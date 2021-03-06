using UnityEngine;
using System.Collections;

public class scr_ThirdPersonCharacter : MonoBehaviour
{
	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 12f;
	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
	[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	[SerializeField] float m_MoveSpeedMultiplier = 1f;
	[SerializeField] float m_AnimSpeedMultiplier = 1f;
	[SerializeField] float m_GroundCheckDistance = 0.1f;

	Rigidbody m_Rigidbody;
	Animator m_Animator;
	bool m_IsGrounded;
	float m_OrigGroundCheckDistance;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;
	Vector3 m_GroundNormal;
	float m_CapsuleHeight;
	Vector3 m_CapsuleCenter;
	CapsuleCollider m_Capsule;
	bool m_Crouching;
    public float m_InAirMovementSpeed;
    scr_PSM m_PSM;
    float autoTurnThreshold = 10;
    float autoTurnspeed = 20;
    bool isAiming;
    bool isSliding = false;
    Vector3 currentLookPosition;
    private GameObject m_Player;
    public float m_InAirMaxSpeed;
    scr_AudioManager m_AudioManager;
    scr_ThirdPersonUserControl m_TPUC;
    public float m_slidingSpeedMultiplier;
    private float m_defaultSlidingSpeedMultiplier;
    private float m_defaultMovingTurnSpeed;
    private bool m_playJumpLandingSound = false;
    private bool isClimbing;
    private float m_Left;

	void Start()
	{
		m_Animator = GetComponent<Animator>();
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();
		m_CapsuleHeight = m_Capsule.height;
		m_CapsuleCenter = m_Capsule.center;
        m_Player = this.gameObject;
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		m_OrigGroundCheckDistance = m_GroundCheckDistance;
        m_PSM = GetComponent<scr_PSM>();
        m_AudioManager = Camera.main.GetComponent<scr_AudioManager>();
        m_defaultSlidingSpeedMultiplier = m_MoveSpeedMultiplier;
        m_defaultMovingTurnSpeed = m_MovingTurnSpeed;
        m_TPUC = gameObject.GetComponent<scr_ThirdPersonUserControl>();
	}


	public void Move(Vector3 move, bool crouch, bool jump, bool sliding,bool climbing, Vector3 lookPos, bool aim)
	{

		// convert the world relative moveInput vector into a local-relative
		// turn amount and forward amount required to head in the desired
		// direction.

		if (move.magnitude > 1f) move.Normalize();
		move = transform.InverseTransformDirection(move);
		CheckGroundStatus();
		move = Vector3.ProjectOnPlane(move, m_GroundNormal);
		m_TurnAmount = Mathf.Atan2(move.x, move.z);
		m_ForwardAmount = move.z;
        m_Left = move.x;
        this.isAiming = aim;
        this.isSliding = sliding;
        this.currentLookPosition = lookPos;
        this.isClimbing = climbing;
        if(!isAiming && m_PSM.GetPlayerState(true) == scr_PSM.Playerstate.state_grounded )
        {
            ApplyExtraTurnRotation();
        }
        else if(isAiming)
        {
           // m_Player.transform.forward = Camera.main.transform.forward;
            //
            m_Player.transform.forward = new Vector3(Camera.main.transform.forward.x, m_Player.transform.forward.y, Camera.main.transform.forward.z);
        }
            Sliding();
		// control and velocity handling is different when grounded and airborne:
		if (m_IsGrounded) 
		{
			HandleGroundedMovement(crouch, jump);
            m_PSM.SetPlayerState(scr_PSM.Playerstate.state_grounded);
            m_PSM.SetPlayerPose(scr_PSM.PlayerPose.pose_running);
		}
		else if(m_PSM.GetPlayerPose(true) != scr_PSM.PlayerPose.pose_wallclimbing)
		{
			HandleAirborneMovement();
           m_PSM.SetPlayerState(scr_PSM.Playerstate.state_airborne);
		}
		ScaleCapsuleForCrouching(crouch);
		PreventStandingInLowHeadroom();
        JumpHandler();
		// send input and other state parameters to the animator
		UpdateAnimator(move);
	}
    void JumpHandler()
    {
        if (Input.GetButton("Jump") && m_IsGrounded == true)
        {
            m_Rigidbody.AddForce(new Vector3(0, m_JumpPower, 0), ForceMode.Impulse);
            m_Rigidbody.useGravity = true;
        }
    }
	void ScaleCapsuleForCrouching(bool crouch)
	{
		if (m_IsGrounded && crouch)
		{
			if (m_Crouching) return;
			m_Capsule.height = m_Capsule.height / 2f;
			m_Capsule.center = m_Capsule.center / 2f;
			m_Crouching = true;
		}
		else
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
				return;
			}
			m_Capsule.height = m_CapsuleHeight;
			m_Capsule.center = m_CapsuleCenter;
			m_Crouching = false;
		}
	}
    void Sliding()
    {
        if (m_IsGrounded && isSliding)
        {
            m_Capsule.height = m_Capsule.height / 2f;
            m_Capsule.center = m_Capsule.center / 2f;
            //Set animation to sliding also!
            m_ForwardAmount = 1;
            m_MoveSpeedMultiplier = m_slidingSpeedMultiplier;
            m_MovingTurnSpeed = 0;
            
        }
        else
        {
            m_MoveSpeedMultiplier = m_defaultSlidingSpeedMultiplier;
            m_MovingTurnSpeed = m_defaultMovingTurnSpeed;
        }


    }
	void PreventStandingInLowHeadroom()
	{
		// prevent standing up in crouch-only zones
		if (!m_Crouching)
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, ~0, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
			}
		}
	}
    void TurnTowardsCameraForward()
    {
        if (Mathf.Abs(m_ForwardAmount) < .01f)
        {
            Vector3 lookDelta = transform.InverseTransformDirection(currentLookPosition - transform.position);
            float lookAngle = Mathf.Atan2(lookDelta.x, lookDelta.z) * Mathf.Rad2Deg;

            if (Mathf.Abs(lookAngle) > autoTurnThreshold)
            {
                m_TurnAmount += lookAngle * autoTurnspeed * 0.001f;
            }
        }
    }

	void UpdateAnimator(Vector3 move)
	{
		// update the animator parameters
		m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
		m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
		//m_Animator.SetBool("Crouch", m_Crouching);
		m_Animator.SetBool("OnGround", m_IsGrounded);
        m_Animator.SetBool("Climb", isClimbing);
        m_Animator.SetBool("Aim", isAiming);

        m_Animator.SetFloat("Left", -m_Left);

		if (!m_IsGrounded)
		{
			m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
		}
        if(m_TPUC.GetCanShoot() == true && Input.GetAxisRaw("FireAxis") != 0)
        {
            m_Animator.SetBool("Shoot", true);
        }
        else
        {
            m_Animator.SetBool("Shoot", false);
        }
		// calculate which leg is behind, so as to leave that leg trailing in the jump animation
		// (This code is reliant on the specific run cycle offset in our animations,
		// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
		float runCycle =
			Mathf.Repeat(
				m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);

		float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
		if (m_IsGrounded)
		{
		//	m_Animator.SetFloat("JumpLeg", jumpLeg);
		}

		// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
		// which affects the movement speed because of the root motion.
		if (m_IsGrounded && move.magnitude > 0)
		{
			m_Animator.speed = m_AnimSpeedMultiplier;
            
		}
		else
		{
			// don't use that while airborne
			m_Animator.speed = 1;
		}
        if(m_IsGrounded && m_ForwardAmount >=0.2)
        {
            PlayRunningSounds();
        }
	}
    void PlayRunningSounds()
    {
        m_AudioManager.RandomizeSfx(0.3f, m_AudioManager.GetFootStepSounds());
    }
	void HandleAirborneMovement()
	{
		// apply extra gravity from multiplier:
        // dont allow the player to rotate with vertical - while in air.
		Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        if(v != 0 && Mathf.Abs(m_Rigidbody.velocity.z) < m_InAirMaxSpeed)
        {
            m_Rigidbody.AddRelativeForce(new Vector3(0, 0,  v* m_InAirMovementSpeed));
        }
        if(h != 0 && Mathf.Abs(m_Rigidbody.velocity.x) < m_InAirMaxSpeed)
        {
            m_Rigidbody.AddRelativeForce(new Vector3(h * m_InAirMovementSpeed, 0, 0));
        }
        m_Rigidbody.AddRelativeForce(extraGravityForce, ForceMode.Acceleration);
		m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
	}


	void HandleGroundedMovement(bool crouch, bool jump)
	{
		// check whether conditions are right to allow a jump:
		if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
		{
			// jump!
			m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
			m_IsGrounded = false;
			m_Animator.applyRootMotion = false;
			m_GroundCheckDistance = 0.1f;
          //  m_AudioManager.RandomizeSfx(0,m_AudioManager.GetFootStepSounds());

		}
	}

	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
	}


    public void OnAnimatorMove()
    {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (m_IsGrounded && Time.deltaTime > 0)
        {
            Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = v;
        }
    }


	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
		{
			m_GroundNormal = hitInfo.normal;
			m_IsGrounded = true;
			m_Animator.applyRootMotion = true;
       
            if(m_playJumpLandingSound)
            {
                m_AudioManager.RandomizeSfx(0,m_AudioManager.GetJumpLandingSounds());
                m_playJumpLandingSound = false;
            }

		}
		else
		{
            m_playJumpLandingSound = true;
			m_IsGrounded = false;
			m_GroundNormal = Vector3.up;
			//m_Animator.applyRootMotion = false;
            //if(isAiming )
            //{
            //    m_Rigidbody.velocity = Vector3.zero;
            //    m_ForwardAmount = 0;
            //}
		}
	}
}
