using UnityEngine;
using System.Collections;

public class scr_CharacterMovement : MonoBehaviour {

    float m_MoveSpeedMultiplier = 1;
    float m_stationaryTurnSpeed = 360;
    float m_movingTurnSpeed = 360;

    bool onGround;

    Animator m_aim;
    Rigidbody m_rb;

    Vector3 m_moveInput;
    float m_turnAmount;
    float m_forwardAmount;
    Vector3 velocity;

   public float m_jumpPower = 10;

    IComparer rayHitComparer;

    float autoTurnThreshold = 10;
    float autoTurnspeed = 20;
    bool isAiming;
    Vector3 currentLookPosition;

	// Use this for initialization
	void Start () 
    {

        SetUpAnimator();
	}   
    public void Move(Vector3 move, bool aim, Vector3 lookPos)
    {
        if(move.magnitude > 1)
        {
            move.Normalize();
        }
        this.m_moveInput = move;
        this.isAiming = aim;
        this.currentLookPosition = lookPos;

        velocity = m_rb.velocity;
        ConvertMoveInput();

        if(!isAiming) // när man inte aimar
        {
            ApplyExtraTurnRotation();

        }
        GroundCheeck();
        JumpHandler();
        UpdateAnimator();
    }
    void SetUpAnimator()
    {
        m_aim = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody>();
        foreach (Animator childAnimator in GetComponentsInChildren<Animator>())
        {
            if (childAnimator != m_aim)
            {
                m_aim.avatar = childAnimator.avatar;
                Destroy(childAnimator);
                break;
            }
        }
    }
    void OnAnimatorMove()
    {
        if(onGround && Time.deltaTime > 0)
        {
            Vector3 v = (m_aim.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;
            v.y = m_rb.velocity.y;
            m_rb.velocity = v;

        }
    }
    void ConvertMoveInput()
    {
        Vector3 m_localMove = transform.InverseTransformDirection(m_moveInput);

        m_turnAmount = Mathf.Atan2(m_localMove.x, m_localMove.z);
        m_forwardAmount = m_localMove.z;
    }
    void UpdateAnimator()
    {
        m_aim.applyRootMotion = true;
        m_aim.SetFloat("Forward", m_forwardAmount, 0.1f, Time.deltaTime);
        m_aim.SetFloat("Turn", m_turnAmount, 0.1f, Time.deltaTime);

        // Säger till animator att spelaren siktar.
       // m_aim.SetBool("Aim", isAiming);

    }
    void ApplyExtraTurnRotation()
    {
        float turnSpeed = Mathf.Lerp(m_stationaryTurnSpeed, m_movingTurnSpeed, m_forwardAmount);
        transform.Rotate(0, m_turnAmount * turnSpeed * Time.deltaTime, 0);
            
    }
    void JumpHandler()
    {
        if(Input.GetButton("Jump") && onGround == true)
        {
            m_rb.AddForce(new Vector3(0, m_jumpPower, 0),ForceMode.Impulse);
            m_rb.useGravity = true;
        }
    }
    void GroundCheeck()
    {
        Ray ray = new Ray(transform.position + Vector3.up * .5f, -Vector3.up);

        RaycastHit[] hits = Physics.RaycastAll(ray, 1.0f);
        rayHitComparer = new RayHitComparer();

        System.Array.Sort(hits, rayHitComparer);

        if (velocity.y < m_jumpPower * .5f) // 
        {
            onGround = false;
            m_rb.useGravity = true;// <-------------------------- tempdisable need a check if in air+ move+ rb.velocity otherwise stuck in air
            foreach (var hit in hits)
            {
                if (!hit.collider.isTrigger)
                {
                    if (velocity.y <= 0)
                    {
                        m_rb.position = Vector3.MoveTowards(m_rb.position, hit.point, Time.deltaTime * 5);
                    }
                    onGround = true;
                    m_rb.useGravity = false;

                    break;
                }
            }

            Debug.DrawRay(transform.position, -transform.up / 2, Color.green);
            if (Physics.Raycast(transform.position, -transform.up, 1.8f))
            {
                Debug.Log("grounded");
                m_rb.useGravity = false;
                Debug.DrawRay(transform.position, -transform.up / 2, Color.green);
                if (Physics.Raycast(transform.position, -transform.up, 0.8f))
                {
                    Debug.Log("grounded");
                }
                else
                {
                    Debug.Log("air");
                }
                //Get distance to ground from player height, if the distance to the ground is bigger than that on ground = false;
                // om det händer gravity = true;


            }
        }
    }
    void TurnTowardsCameraForward()
    {
        if(Mathf.Abs(m_forwardAmount) < .01f)
        {
            Vector3 lookDelta = transform.InverseTransformDirection(currentLookPosition - transform.position);
            float lookAngle = Mathf.Atan2(lookDelta.x, lookDelta.z) * Mathf.Rad2Deg;

            if(Mathf.Abs(lookAngle)> autoTurnThreshold)
            {
                m_turnAmount += lookAngle * autoTurnspeed * 0.001f;
            }
        }
    }
    class RayHitComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
            //this returns < 0 if x < y
            // > 0 if x > y
            // 0 if x = y
        }
    }
}
