using UnityEngine;
//using UnityEditor;

public class scr_FreeCameraLook : scr_Pivot {

    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float turnSpeed = 1.5f;
    [SerializeField]
    private float turnsmoothing = .1f;
    [SerializeField]
    private float tiltMax = 75f;
    [SerializeField]
    private float tiltMin = 45f;
    [SerializeField]
    private bool lockCursor = false;


    private float lookAngle;
    private float tiltAngle;

    private const float LookDistance = 100f;

    private float smoothX = 0;
    private float smoothY = 0;
    private float smoothXvelocity = 0;
    private float smoothYvelocity = 0;

    private Vector3 velocity = Vector3.zero;
    //if no input, player moving, bigger than resettimer'
    private bool m_takingInput;
    private bool m_autoFollowPlayer;
    public float m_CameraResetTime;
    private float m_CameraResetCounter;


    private GameObject m_player;
    private Rigidbody m_playerRB;

    private Transform m_pivot;

    private Vector3 normalOffset;
    private Vector3 aimingOffset;
    protected override void Awake()
    {
        //lookAngle needs to be rested with TurnCameraTowardsPlayerForward
        //Otherwise it snaps to the angle as soon as input is taken

        base.Awake();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerRB = m_player.GetComponent<Rigidbody>();

        normalOffset = m_player.GetComponent<scr_ThirdPersonUserControl>().GetNormalCameraOffset();
        aimingOffset = m_player.GetComponent<scr_ThirdPersonUserControl>().GetAimCameraOffset();

        Cursor.visible = lockCursor;
  
        //Add so visible and locked happends togheter
        m_pivot = GetComponentInChildren<scr_Pivot>().GetPivotTF();

        if (lockCursor)
        {
         //   Cursor.lockState = CursorLockMode.Locked;
         //   Cursor.visible = lockCursor;
        }

        cam = GetComponentInChildren<Camera>().transform;
        pivot = cam.parent;

    }

    protected override void Update()
    {
        base.Update();
        HandleRotationMovement();
        MoveCameraPivotCloserToPlayer();
        if(m_takingInput == false)
        {
            m_CameraResetCounter += Time.deltaTime;
            if(m_CameraResetCounter > m_CameraResetTime && m_playerRB.velocity != Vector3.zero && !Input.GetMouseButton(1))
            {
                TurnCameraTowardsPlayerForward();
            }
        }
        else
        {
            m_CameraResetCounter = 0;
        }
        if (lockCursor && Input.GetMouseButtonUp(0))
        {
           // Cursor.visible = !lockCursor;

        }
    }
    void MoveCameraPivotCloserToPlayer()
    {
         float moveOffsetHigh = Mathf.Abs(tiltAngle) / tiltMax;
         //Debug.Log(moveOffsetHigh);
         transform.position = Vector3.MoveTowards(transform.position, new Vector3(m_player.transform.position.x + normalOffset.x,
             m_player.transform.position.y + normalOffset.y, m_player.transform.position.z + normalOffset.z + moveOffsetHigh),1);

    }
    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    protected override void Follow(float deltaTime)
    {
        transform.position = Vector3.Lerp(transform.position , target.position , deltaTime * moveSpeed);
    }
    float offsetX = 0;
    float offsetY = 0;
    void TurnCameraTowardsPlayerForward()
    {
        //lookAngle = Mathf.Lerp(lookAngle, m_player.transform.eulerAngles.y, 10);
        lookAngle = Mathf.MoveTowardsAngle(lookAngle, m_player.transform.localEulerAngles.y, 0.8f);

        //m_forward = m_player.transform.forward;
        //m_wantedPos = m_player.transform.position - m_forward;

        //transform.position = Vector3.SmoothDamp(transform.position, m_wantedPos,ref refVel, smoothVel);
        //transform.LookAt(target.transform);
        //transform.rotation = Quaternion.Lerp(transform.rotation, m_player.transform.rotation, 1);

        //Vector3 forward = m_player.transform.forward;
        //Vector3 needPos = m_player.transform.position - forward;
        //transform.position = Vector3.SmoothDamp(transform.position, needPos, ref velocity, 0.05f);
        //// transform.LookAt(target.transform);
        //transform.rotation = Quaternion.Lerp(transform.rotation, m_player.transform.rotation, 1);


        //if (Mathf.Abs(m_forwardAmount) < .01f)
        //{
        //    Vector3 lookDelta = transform.InverseTransformDirection(currentLookPosition - transform.position);
        //    float lookAngle = Mathf.Atan2(lookDelta.x, lookDelta.z) * Mathf.Rad2Deg;

        //    if (Mathf.Abs(lookAngle) > autoTurnThreshold)
        //    {
        //        m_turnAmount += lookAngle * autoTurnspeed * 0.001f;
        //    }
        //}

    }

    void handleOffsets()
    {
        if (offsetX != 0)
        {
            offsetX = Mathf.MoveTowards(offsetX, 0, Time.deltaTime);
        }

        if (offsetY != 0)
        {
            offsetY = Mathf.MoveTowards(offsetY, 0, Time.deltaTime);
        }
    }


    void HandleRotationMovement()
    {
        handleOffsets();

        float x = Input.GetAxis("Mouse X") + offsetX;
        float y = Input.GetAxis("Mouse Y") + offsetY;
         // float x = Input.GetAxis("xbox_h") + offsetX;
        //float y = Input.GetAxis("xbox_v") + offsetX;
        if(x != 0 ||y != 0)
            m_takingInput = true;
        else
            m_takingInput = false;

       // Debug.Log(m_takingInput);
        if (turnsmoothing > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, x, ref smoothXvelocity, turnsmoothing);
            smoothY = Mathf.SmoothDamp(smoothY, y, ref smoothYvelocity, turnsmoothing);
        }
        else
        {
            smoothX = x;
            smoothY = y;
        }
        lookAngle += smoothX * turnSpeed;

        transform.rotation = Quaternion.Euler(0f, lookAngle, 0);

        tiltAngle -= smoothY * turnSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, -tiltMin, tiltMax);

        pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

    }

}
