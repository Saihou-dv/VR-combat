using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.AI;
namespace RootMotion.Dynamics {

	/// <summary>
	/// This is just a commented template for creating new Puppet Behaviours.
	/// </summary>
	[AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/Behaviours/CustomPuppetBehaviour")]
	public class CustomPuppetBehaviour : BehaviourBase {
		
		public LayerMask CollisionLayers;
		
		private int collisions;
		private float timeStep = 0.2f;
		private float Collisontimer = 0f;
		private int[] CollisionLayerints;
		public Animator AnimControl;
		public Text impulseinfo;
		public EnemyHP enemyhp;
		[Range(1, 30)] public int maxCollisons;
		public UnityEvent noHpevent;
		bool playedsound = false;
		[Header("PuppetMaster Controls")]
		public Rigidbody Hips;
		bool checkedifdead = false;
		float unpinnedMuscleWeight = 0;
		[Header("Animation")]
		public Animator puppetanim;
		float stunnedthresshold = 3f;
		float stunnedtimer;
	    bool stuncalled = false;
		public NavMeshAgent agent;
		[Range(4, 12)]
		public float knockoutDmg;
		private RaycastHit hit;
		public LayerMask floor;
		[Header("Audio")]
		public AudioClip impactclip;
		public AudioClip Knockout;
		public UnityEvent Fallen;
		[Header("Remapping Pin")]
		public bool isremapped = true;
		public bool isstill = true;
		
		[Range(0.01f,1)]
		public float stillthresshold;
		private float stillduration = 1f;
		private float stilltime = 0;

		[Header("SlowMo")]
		private float orginalfixedDeltatime;


		// Just for demonstrating the intended use of sub-behaviours. 
		// Sub-behaviours take care of behaviour code reusability.
		// While there can be only one active Puppet Behaviour at a time, that active behaviour can use multiple independent and reusable sub-behaviours simultaneously.
		// For example SubBehaviourCOM is responsible for calculating everything about the center of mass and can be used by any behaviour or even other sub-behaviours that need CoM information.
		public SubBehaviourCOM centerOfMass;

		// Used by SubBehaviourCOM
		public LayerMask groundLayers;

		// Just for demonstrating the intended use of PuppetEvents
		public PuppetEvent onLoseBalance;

		// A PuppetEvent will be called when the balance angle exceeds this point.
		public float loseBalanceAngle = 60f;
		
		protected override void OnInitiate() {
			// Initiate something. This is called only once by the PuppetMaster in Start().
			CollisionLayerints = CollisionLayers.MaskToNumbers();
			// Initiating sub-behaviours. SubBehaviourCOM will update automatically once it has been initiated
			centerOfMass.Initiate(this as BehaviourBase, groundLayers);
		}

        private void Awake()
        {
			enemyhp = GetComponentInParent<EnemyHP>();
			orginalfixedDeltatime = Time.fixedDeltaTime;
        }
        protected override void OnActivate() {
			// When this becomes the active behaviour. There can only be one active behaviour. 
			// Switching behaviours is done by the behaviours themselves, using PuppetEvents.
			// Each behaviour should know when it is no longer required and which behaviours to switch to in each case.
		}
		
		public override void OnReactivate() {
			// Called when the PuppetMaster has been deactivated (by parenting it to an inactive hierarchy or calling SetActive(false)).
		}

		protected override void OnDeactivate() {
			// Called when this behaviour is exited. OnActivate is the place for resetting variables to defaults though.
		}

		private bool IsOnBack()
		{
			if (Physics.Raycast(Hips.position, transform.TransformDirection(Vector3.forward), out hit, 1, floor))
			{
				return false;


			}
			return true;
		}
        
			void RemapPuppet()
			{
            if (enemyhp.Enemy_HP > 0) 
			{
				Vector3 up = Vector3.up;
				Vector3 forward = Vector3.ProjectOnPlane(Hips.transform.forward, up).normalized;
				if (forward == Vector3.zero) forward = puppetMaster.targetRoot.forward;
				Physics.Raycast(Hips.position, transform.TransformDirection(Vector3.down), out hit, 1, floor);
				Vector3 hippos = new Vector3(Hips.position.x, hit.point.y, Hips.position.z);
				Quaternion flatRotation = Quaternion.LookRotation(forward, up);
				puppetMaster.targetRoot.rotation = flatRotation;
				puppetMaster.Teleport(hippos, flatRotation, false);
				puppetMaster.pinWeight = 1f;
				puppetMaster.state = PuppetMaster.State.Alive;
				isremapped = true;
			}
			
			}
		protected override void OnFixedUpdate(float deltaTime) {
			// Everything happening in the fixed time step.

			// Example of using PuppetEvents
			if (centerOfMass.angle > loseBalanceAngle) {
				// If the angle between Vector3.up and the vector from the center of pressure to the center of mass > loseBalanceangle, lose balance (maybe switch to another behaviour).
				onLoseBalance.Trigger(puppetMaster);
			}
			

            if (collisions > 0)
            {
				Collisontimer -= Time.deltaTime;
				if(Collisontimer <= 0)
                {
					collisions = 0;
					//AnimControl.ResetTrigger("Punched");
				}
            }
			/*
            if (stuncalled)
            {
				stunnedtimer += Time.fixedDeltaTime;
				Debug.Log(stunnedtimer);
				if (stunnedtimer>=stunnedthresshold)
                {
					
					stunnedtimer = 0;
					puppetanim.ResetTrigger("Stunned");
					stuncalled = false;
					agent.enabled = true;
                }
            }*/
	
            if (!isremapped && puppetMaster.state == (PuppetMaster.State.Dead))
            {
				
				foreach (var muscle in puppetMaster.muscles)
                {
                    if (muscle.rigidbody.velocity.magnitude > stillthresshold)
                    {
						
						isstill = false;
						break;
                    }
					isstill = true;
                }

                if (isstill)
                {
					stilltime += deltaTime;
                    if (stilltime >= stillduration)
                    {
						RemapPuppet();
                    }
                }
                else
                {
					stilltime = 0;
                }
            }

			

		}
		IEnumerator ResumeTimeAfterDelay(float duration)
		{
			yield return new WaitForSecondsRealtime(duration); // unaffected by timescale
			Time.timeScale = 1f;
			Time.fixedDeltaTime = orginalfixedDeltatime;
			
		}
		protected override void OnLateUpdate(float deltaTime) {
			// Everything happening in LateUpdate().
		}

		protected override void OnMuscleHitBehaviour(MuscleHit hit) {
			if (!enabled) return;

			// If the muscle has been hit via code using MuscleCollisionBroadcaster.Hit(float unPin, Vector3 force, Vector3 position);
			// This is used for shooting based on raycasting instead of physical collisions.
		}

		private void UnPinAndRecover()
        {
			isremapped = false;
			puppetMaster.state = PuppetMaster.State.Dead;
			puppetMaster.pinWeight = unpinnedMuscleWeight;
		
		}
		protected override void OnMuscleCollisionBehaviour(MuscleCollision m) {
			if (!enabled) return;
			if (collisions > maxCollisons) return;
			// If the muscle has collided with something that is on the PuppetMaster's collision layers.
			
			for(int i =0; i<CollisionLayerints.Length; i++)
            {	
				if(CollisionLayerints[i] == m.collision.gameObject.layer)
                {
					Transform current = m.collision.collider.transform;
					HandPhysics hand = null;

					while (current != null)
					{
						hand = current.GetComponent<HandPhysics>();
						if (hand != null) break;
						current = current.parent;
					}

                    if (hand != null)
                    {
						
						AudioSource.PlayClipAtPoint(impactclip, m.collision.contacts[0].point, hand.speed / 10f);

						//AnimControl.SetTrigger("Punched");
						//Rigidbody dmg = hand.GetComponentInChildren<Rigidbody>();
						float damage = m.collision.relativeVelocity.magnitude;

                        //Debug.Log(damage);
                        if (damage > knockoutDmg && enemyhp.Enemy_HP>0)
                        {
							UnPinAndRecover();
                        }
						enemyhp.TakeDamage(damage);
                        

					}







                    if (!checkedifdead)
                    {
						if (enemyhp.Enemy_HP <= 0)
						{
							checkedifdead = true;
							if (!playedsound)
							{

								AudioSource.PlayClipAtPoint(Knockout, m.collision.contacts[0].point);
								playedsound = true;
								Time.timeScale = 0.2f;
								Time.fixedDeltaTime = orginalfixedDeltatime * Time.timeScale;
								StartCoroutine(ResumeTimeAfterDelay(3));
							}
							noHpevent.Invoke();
							puppetMaster.state = PuppetMaster.State.Dead;
							puppetMaster.muscleWeight = 2;
							Hips.mass = 50f;
							//puppetMaster.Invoke("Freeze", 1);
							//puppetMaster.muscles[i].rigidbody.AddForce(10f*m.collision.transform.TransformDirection(Vector3.forward), ForceMode.Impulse);
						}
					}
					
					if (collisions == 0)
						Collisontimer = timeStep;

					collisions++;
                }
            }
			
		}
	}
}
