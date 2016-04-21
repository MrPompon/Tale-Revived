using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class scr_attachRopeTo : MonoBehaviour
{

    // Use this for initialization
    Collider m_collider;
    Vector3 velocity;
    Vector3 acceleration;
    public float maxVelocity;
    public float minVelocity;
    public Transform tetherObject;
    public bool amITethered;
    Vector3 prevPosition;
    Vector3 prevVelocity;
    Vector3 newVelocity;
    Transform thisOffsetTransform;
    public float speedMultiplier;
    public float reelInMultiplier;
    public bool autoShortenRope;
    public bool triesToReachDesired;
    public float desiredLength;
    public float maxTetherLength;
    public float tetherLength;
    [SerializeField]
    private float swingSpeed;
    public float downwardAcc;
     LineRenderer m_lineRenderer;
     Rigidbody m_rgd;
     [HideInInspector]
     public float vInput=0, hInput=0;
    [SerializeField]
     private float playerHeigthTetherOffset;
    scr_playerStateFunctions PSF;
    [SerializeField]float detachJumpPowerUpwardForce, detachJumpPowerForwardForce;
    //rope pulling stuff
    scr_Player_Pulling m_pullRope;
    [SerializeField]
    private float m_ropePullForce;
    private float distToGround;
    public LayerMask groundLayer;
    private bool isGrounded;
    private Transform ropeRenderpoint1, ropeRenderpoint2;
    private Transform attachedArrowsHitTransform;
    void Start()
    {
        m_collider = GetComponent<Collider>();
        m_lineRenderer = GetComponent<LineRenderer>();
        m_rgd = GetComponent<Rigidbody>();
        m_pullRope = GetComponent<scr_Player_Pulling>();
        PSF = GameObject.FindGameObjectWithTag("SingletonHandler").GetComponent<scr_playerStateFunctions>();
        distToGround = m_collider.bounds.extents.y;
        ropeRenderpoint1 = this.transform;
        ropeRenderpoint2 = tetherObject;
    }

    // Update is called once per frame
    void Update()
    {
        //GamePad.SetVibration(0, 2, 3);
        isGrounded = IsGrounded();
        if (amITethered)
        {
            TetherInput();
            //extend rope when moving with it and not swinging    <------ add when this check exists
            if (isGrounded)
            {
                if (tetherObject != null && !Input.GetButton("ReelInRope"))
                {

                    tetherLength = Vector3.Distance(this.transform.position, tetherObject.transform.position);
                    // do slidy slide or pull object
                    //when grounded works add restriction when falling with rope
                    print("HUGGA HUGGA"); // do slidy thing? this no work well 
                }
                if (Input.GetButton("ReelInRope"))
                {
                    Pullrope();
                }
            }
        }
    }
    public float GetMaxTetherLength()
    {
        return maxTetherLength;
    }
    public Transform GetTetherObject()
    {
        return tetherObject; 
    }
    void TetherInput()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        
        if (Input.GetButton("PullRope"))
        {
            Pullrope();
        }
        if (Input.GetButton("ReelInRope"))
        {
            if (!isGrounded)
            {
                if (tetherLength > 2)
                {
                    tetherLength -= Time.deltaTime * reelInMultiplier;
                }
            }
        }
        else if (Input.GetButton("ReelOutRope"))
        {
            if (tetherLength < maxTetherLength)
            {
                tetherLength += Time.deltaTime * reelInMultiplier;
            }
        }
        //if not grounded, if input ANY direction Input, 
        if (Input.GetButtonDown("Jump"))//&& !isGrounded
        {
            if (!isGrounded)
            {
                PSF.SetRunning();
                PSF.DeattachTether();
                PSF.DeattachJump();
            }
        }
    }
    public void SetRoperenderingpoints(Transform from, Transform to)
    {
        ropeRenderpoint1 = from;
        ropeRenderpoint2 = to;
    }
    void LateUpdate()
    {
        if (amITethered)
        {
            UpdateLineRenderer();
        }
    }
    void FixedUpdate()
    {
        print(IsGrounded());
        if (!amITethered)
        {
            m_lineRenderer.enabled = false;
        }
        else if (amITethered && m_lineRenderer.enabled == false)
        {
            m_lineRenderer.enabled = true;
        }
   
        if (!isGrounded)
        {
            if (amITethered)
            {
                HandleMoveInput(vInput, hInput);

            }
        }
            NewVelocity();
            if (!isGrounded && amITethered)
            {
                this.transform.Translate(prevVelocity * speedMultiplier * Time.deltaTime, Space.World);
                m_rgd.velocity = prevVelocity * speedMultiplier * Time.deltaTime;

                TetherRestriction();
            }
            
            PrevVelocity();  //always update velocity thingies 
            PrevPosition();
            if (!isGrounded)
            {
                if (amITethered)
                {
                    if (autoShortenRope)
                    {
                        ShrinkRope();
                    }
                    if (triesToReachDesired)
                    {
                        ResizeTowardsDesired();
                    }
                }
            }
    }
    public void SetAttachedArrowsHitTransform(Transform p_transform)
    {
        attachedArrowsHitTransform = p_transform;
    }
    public void SetTeatherObject(Transform p_transform)
    {
        tetherLength = Vector3.Distance(this.transform.position, p_transform.position) -playerHeigthTetherOffset;
        tetherObject = p_transform;
        
    }
    public void RemoveTetherObject()
    {
        tetherObject = null;
    }
    void ResizeTowardsDesired()
    {
        if (desiredLength < tetherLength)
        {
            tetherLength -= Time.deltaTime*reelInMultiplier;
        }
        else if (desiredLength > tetherLength)
        {
            tetherLength += Time.deltaTime*reelInMultiplier;
        }
    }
    public void SetAmITethered(bool trueOrFalse)
    {
        amITethered = trueOrFalse;
    }
    void ShrinkRope(){
        tetherLength =Vector3.Distance(this.transform.position,tetherObject.transform.position);
    }
    void FlipForces()
    {
        Vector3 newVel=new Vector3(-GetComponent<Rigidbody>().velocity.x,-GetComponent<Rigidbody>().velocity.y, -GetComponent<Rigidbody>().velocity.z);
        GetComponent<Rigidbody>().velocity =VelocityMaintainer( newVel) ;
        
    }
    void TetherRestriction()
    {
        if (amITethered)
        {
            if (Vector3.Distance(this.transform.position, tetherObject.transform.position) > tetherLength)
            {
                // we're past the end of our rope
                // pull the avatar back in.
                //this.transform.Translate(prevVelocity); 
                this.transform.position =tetherObject.transform.position+ (this.transform.position - tetherObject.transform.position).normalized * tetherLength;
                //FlipForces();
            }//if vel.y<0 when teather restriction mean reset gravity?
        }
    }
    void PrevVelocity()
    {
        prevVelocity=VelocityMaintainer(this.transform.position- prevPosition);
    }
    void PrevPosition()
    {
        prevPosition = this.transform.position;
    }
    void NewVelocity()
    {
        newVelocity=VelocityMaintainer( prevPosition-this.transform.position);
    }
    void NonTetheredAirMovement()
    {
        print("MovingThroughAir");

    }
    void UpdateLineRenderer()
    {
        if (ropeRenderpoint1 != null && ropeRenderpoint2 != null)
        {
            if (amITethered)
            {
                m_lineRenderer.SetPosition(0, ropeRenderpoint1.position);
                m_lineRenderer.SetPosition(1, ropeRenderpoint2.position);
            }
        }
    }
    void HandleMoveInput(float vInput, float hInput)
    {
        this.transform.Translate(new Vector3(hInput * swingSpeed,0 , vInput * swingSpeed) * Time.deltaTime);
        if (amITethered)
        {//fake gravity 
           // this.transform.Translate(new Vector3(0, downwardAcc, 0) * Time.deltaTime, Space.World);
        }
    }
    public void JumpAtDetach()
    {
        //depending on input?
        m_rgd.AddForce(new Vector3(0, detachJumpPowerUpwardForce, detachJumpPowerForwardForce)+m_rgd.velocity, ForceMode.Impulse);//MAYBE ADD forward force aswell(probably) note maybe just add current vel+* some offset multi
    }
    void GetInput()
    {
        acceleration.x = Input.GetAxis("Horizontal");
        acceleration.y = Input.GetAxis("Vertical");
    }
    Vector3 VelocityMaintainer(Vector3 p_velocity)
    {
        //check max
        Vector3 newVelocity = p_velocity;
        if (p_velocity.x > maxVelocity)
        {
            newVelocity = new Vector3(maxVelocity, newVelocity.y, newVelocity.z);
        }
        if (p_velocity.y > maxVelocity)
        {
            newVelocity = new Vector3(newVelocity.x, maxVelocity,newVelocity.z);
        }
        if (p_velocity.z > maxVelocity)
        {
            newVelocity = new Vector3(newVelocity.x, newVelocity.y, maxVelocity);
        }
        //check min
        if (p_velocity.x < minVelocity)
        {
            newVelocity = new Vector3(minVelocity, newVelocity.y, newVelocity.z);
        }
        if (p_velocity.y < minVelocity)
        {
            newVelocity = new Vector3(newVelocity.x, minVelocity, newVelocity.z);
        }
        if (p_velocity.z < minVelocity)
        {
            newVelocity = new Vector3(newVelocity.x, newVelocity.y, minVelocity);
        }
       // Debug.Log(newVelocity);
        return newVelocity;
    }
    private void Pullrope()
    {
        if (attachedArrowsHitTransform != null)
        {
            m_pullRope.PullRope(attachedArrowsHitTransform, m_ropePullForce);
            print("hnnnnnnnnnng");
        }
    }
    public bool IsGrounded()
     {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f, groundLayer);
    }
}
