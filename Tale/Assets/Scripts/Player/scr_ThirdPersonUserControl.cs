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
    bool ropeAttachedToArrow;
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

    }


    private void Update()
    {
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
        aim = Input.GetAxisRaw("AimAxis")>0.2;
        if (aim)
        {
            if(Input.GetMouseButtonDown(1))
            {
                m_audioManager.PlayDrawBow();
            }
            if (Input.GetAxisRaw("FireAxis") == 1 && m_reloadCounter > m_ReloadTime) //load the bow
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
                if (Input.GetAxisRaw("FireAxis") ==0 && m_reloadCounter > m_ReloadTime)
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
                        projMovement.GetComponent<LineRenderer>().enabled=false;
                        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
                        //projMovement.AddVelocity(ray.direction, m_projectileSpeed + m_rgd.velocity);
                        Rigidbody arrowRgd = (Rigidbody)arrow.GetComponent<Rigidbody>();
                        arrowRgd.AddForce((ray.direction * (m_projectileSpeed + (currentArrowForce * bowAccumulationMultiplier))), ForceMode.Impulse);
                        m_reloadCounter = 0;
                        currentArrowForce = 0;
                        arrowIsLoaded = false;
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
            move = Vector3.zero; // när man aimar så stannar man
            camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;

            Vector3 dir = lookPosition - transform.position;  //direktionen man tittar, 
            dir.y = 0;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

            //anim.SetFloat("Forward", v); // This should probably be for the aiming states.
            //anim.SetFloat("Turn", h);

        }
#if !MOBILE_INPUT
		// walk speed multiplier
	    if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

        // pass all parameters to the character control script
        if (!currentlyDisabled)
        {
            m_Character.Move(m_Move, crouch, m_Jump, lookPosition, aim);
            m_Jump = false;
        }
      
    }
}
