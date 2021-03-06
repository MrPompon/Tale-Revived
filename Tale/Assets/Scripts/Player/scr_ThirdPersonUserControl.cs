using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class scr_ThirdPersonUserControl : MonoBehaviour
{
    private scr_ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
    //standard assets ^
    //our assets 
    public GameObject m_arrowPrefab;
    public bool WalkByDefault = false;
    private scr_CharacterMovement charMove;
    private Transform cam;
    private Vector3 camForward;
    private Vector3 move;
    public bool aim;
    private bool m_sliding;
    public float aimingWheight;
    public bool lookInCameraDirection;
    Vector3 lookPosition;

    Animator anim;
    private GameObject m_player;

    private Transform m_arrowSpawnpoint;
    private Rigidbody m_rgd;
    //These variables need to be detailed and set specifically for ALVA
    //  public Transform spine;
    public float aimingZ = 213.46f;
    public float aimingX = -65.93f;
    public float aimingY = 20.1f;
    public float point = 30;

    public float m_projectileSpeed;

    public float m_ReloadTime;
    private float m_reloadCounter;
    [SerializeField]
    private float maxBowLoadupDuration;
    private float currentArrowForce;
    [SerializeField]
    private float bowAccumulationMultiplier;
    public bool currentlyDisabled;

    [SerializeField]
    public Vector3 normalOffset = new Vector3(0, 0, -1);

    [SerializeField]
    public Vector3 aimingOffset = new Vector3(0, -0.3f, 0.7f);
    private bool m_isAxisInUse;
    private bool arrowIsLoaded;
    private scr_AudioManager m_audioManager;



    private bool m_Climbing;
    private GameObject m_ClimableObject;
    public float m_ClimbingLenght;
    public float m_MaxClimbingAngle;  // The angle of the top of the object that the player wants to climb
    public float m_ClimbingSpeed;
   // private bool m_ViableToClimb = false;

    bool ropeAttachedToArrow;
    private scr_PSM m_psm;

    public float m_WallClimbingSpeed;
    private void Start()
    {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
            cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )
        m_Character = GetComponent<scr_ThirdPersonCharacter>();
        m_arrowSpawnpoint = GameObject.FindGameObjectWithTag("arrowSpawnPoint").transform;
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_rgd = gameObject.GetComponent<Rigidbody>();
        m_audioManager = Camera.main.GetComponent<scr_AudioManager>();
        m_psm = m_player.GetComponent<scr_PSM>();

    }
    public bool GetCanShoot()
    {
        if (m_reloadCounter > m_ReloadTime)
            return true;
        else
            return false;
    }
    void CheckClimbViability()
    {
        //Raycast below feet of player to see when done,
        //raycast top of climable object?? If lvl designer doesnt use it well.

        RaycastHit hit;
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y , transform.position.z), transform.forward);
        Debug.DrawRay(new Vector3(transform.position.x , transform.position.y , transform.position.z), transform.forward);
        Debug.DrawRay(transform.position, transform.up,Color.red);

       

        if(Physics.Raycast(ray, out hit) && m_psm.GetPlayerState(true) == scr_PSM.Playerstate.state_airborne && m_rgd.velocity.y < 0 )
        {
            if (hit.collider.tag == "climbable")
            {
                if (Vector3.Distance(hit.point, transform.position) < 2)
                {
                    if(hit.collider.gameObject.transform.position.y + hit.collider.bounds.extents.y - transform.position.y < m_ClimbingLenght)
                    {
                        ClimbObject(hit.collider.gameObject,hit);
                    }
                }
            }
        }
    }
    void ClimbObject(GameObject obj, RaycastHit hit)
    {
        m_psm.SetPlayerPose(scr_PSM.PlayerPose.pose_wallclimbing);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x,
                    obj.transform.position.y + obj.GetComponent<Collider>().bounds.extents.y - 0.5f, transform.position.z), m_ClimbingSpeed);
        m_rgd.velocity = new Vector3(0, 0, 0);
      //  transform.rotation = new Quaternion(hit.normal.x, hit.normal.y,hit.normal.z, transform.rotation.w);

    }
    void ClimbingJump()
    {
        if(m_psm.GetPlayerPose(true) == scr_PSM.PlayerPose.pose_wallclimbing && CrossPlatformInputManager.GetButtonDown("Jump"))
        {
           var localVel = transform.InverseTransformDirection(m_rgd.velocity);
           localVel.z = 1;
           localVel.y = 7;

           m_rgd.velocity = transform.TransformDirection(localVel);
           m_psm.SetPlayerPose(scr_PSM.PlayerPose.pose_running);
        }
    }
    void WallClimbing()
    {
        if(m_psm.GetPlayerPose(true) == scr_PSM.PlayerPose.pose_wallclimbing )
        {
            
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

            var localVel = transform.InverseTransformDirection(m_rgd.velocity);
           // localVel.z = m_WallClimbingSpeed * v;
            localVel.x = m_WallClimbingSpeed * h;

            m_rgd.velocity = transform.TransformDirection(localVel);

        }
    }
    bool GetClimbingStatus()
    {
        if(m_psm.GetPlayerPose(true) == scr_PSM.PlayerPose.pose_wallclimbing)
        {
            m_Climbing = true;
            m_rgd.useGravity = false;
        }
        else
        {
            m_rgd.useGravity = true;
            m_Climbing = false;
        }
        return m_Climbing;
    }
    private void Update()
    {
        CheckClimbViability();
        ClimbingJump();
        WallClimbing();


        if (Input.GetButtonDown("ChangeEquippedArrow"))
        {
            print("ChangeEquippedArrow" + ropeAttachedToArrow);

            if (ropeAttachedToArrow == true)
            {
                ropeAttachedToArrow = false;
            }
            else
            {
                ropeAttachedToArrow = true; 
            }
        }
        if (!m_Jump)
        {
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }
        if(Input.GetKey(KeyCode.T))
        {
            m_psm.SetPlayerPose(scr_PSM.PlayerPose.pose_wallclimbing);
        }
        else
        {
            m_sliding = false;
        }


        aim = Input.GetAxisRaw("AimAxis")>0;  //Controller
      //  aim = Input.GetMouseButton(1);

        if (aim)
        {
            if(Input.GetMouseButtonDown(1))
                m_audioManager.PlayDrawBow();
            if (Input.GetMouseButton(0) && m_reloadCounter > m_ReloadTime)
            print("AIM");
            if (Input.GetAxisRaw("FireAxis")>0) //load the bow
            {
                arrowIsLoaded = true;
                if (currentArrowForce < maxBowLoadupDuration)
                {
                    currentArrowForce += Time.deltaTime;
                }
            }
            else
            {
                m_reloadCounter += Time.deltaTime; // nothing happens
            }
            if (arrowIsLoaded)
            {
                if (Input.GetAxisRaw("FireAxis") == 0 && m_reloadCounter > m_ReloadTime)
                {
                    if (ropeAttachedToArrow)
                    {
                        //   anim.SetTrigger("Fire");
                        GameObject arrow = (GameObject)Instantiate(m_arrowPrefab, m_arrowSpawnpoint.position, m_player.GetComponent<Transform>().rotation);
                        scr_projectileMovement projMovement = arrow.GetComponent<scr_projectileMovement>();
                        projMovement.SetRopeAttachedToArrow(ropeAttachedToArrow);
                        projMovement.OnProjectileSpawn();
                        projMovement.SetProjectileOriginator(this.gameObject);
                        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
                        //projMovement.AddVelocity(ray.direction, m_projectileSpeed + m_rgd.velocity);
                        Rigidbody arrowRgd = (Rigidbody)arrow.GetComponent<Rigidbody>();
                        arrowRgd.AddForce((ray.direction * (m_projectileSpeed + (currentArrowForce * bowAccumulationMultiplier))), ForceMode.Impulse);
                        m_reloadCounter = 0;
                        currentArrowForce = 0;
                        arrowIsLoaded = false;
                    }
                    else
                    {
                        GameObject arrow = (GameObject)Instantiate(m_arrowPrefab, m_arrowSpawnpoint.position, m_player.GetComponent<Transform>().rotation);
                        scr_projectileMovement projMovement = arrow.GetComponent<scr_projectileMovement>();
                        projMovement.SetRopeAttachedToArrow(ropeAttachedToArrow);
                        projMovement.OnProjectileSpawn();
                        projMovement.SetProjectileOriginator(this.gameObject);
                        projMovement.GetComponent<LineRenderer>().enabled = false;
                        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
                        //projMovement.AddVelocity(ray.direction, m_projectileSpeed + m_rgd.velocity);
                        Rigidbody arrowRgd = (Rigidbody)arrow.GetComponent<Rigidbody>();
                        arrowRgd.AddForce((ray.direction * (m_projectileSpeed + (currentArrowForce * bowAccumulationMultiplier))), ForceMode.Impulse);
                        m_reloadCounter = 0;
                        currentArrowForce = 0;
                        arrowIsLoaded = false;
                        m_audioManager.PlayBowShoot();
                    }
                }

            }
        }
        else
        {
            m_reloadCounter += Time.deltaTime;
        }
    }
    void LateUpdate()
    {
        aimingWheight = Mathf.MoveTowards(aimingWheight, (aim) ? 1.0f : 0.0f, Time.deltaTime * 5);

        Vector3 normalState = normalOffset;
        Vector3 aiminngState = aimingOffset;

        Vector3 pos = Vector3.Lerp(normalState, aiminngState, aimingWheight);
        cam.transform.localPosition = pos;

        if (aim)
        {
            Vector3 eulerAngleOffset = Vector3.zero;

            eulerAngleOffset = new Vector3(aimingX, aimingY, aimingZ);

            Ray ray = new Ray(cam.position, cam.forward);

            Vector3 lookPosition = ray.GetPoint(point);
            //  spine.LookAt(lookPosition);
            //spine.Rotate(eulerAngleOffset, Space.Self);
        }
    }
    public Vector3 GetNormalCameraOffset()
    {
        return normalOffset;
    }
    public Vector3 GetAimCameraOffset()
    {
        return aimingOffset;
    }
    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        // read inputs
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        bool crouch = Input.GetKey(KeyCode.C);
        if (!aim) // Om vi inte aimar.
        {
            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
        }
        else // om vi aimar.
        {
           // move = Vector3.zero; // n�r man aimar s� stannar man
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v * m_CamForward + h * m_Cam.right;
            Vector3 dir = lookPosition - transform.position;  //direktionen man tittar, 
            dir.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

           // anim.SetFloat("Forward", v); // This should probably be for the aiming states.
            //anim.SetFloat("Turn", h);

        }
   
#if !MOBILE_INPUT
		// walk speed multiplier
	    if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

        // pass all parameters to the character control script
        if (!currentlyDisabled)
        {
            m_Character.Move(m_Move, crouch, m_Jump,m_sliding,GetClimbingStatus(), lookPosition, aim);
            m_Jump = false;
        }
      
    }
}
