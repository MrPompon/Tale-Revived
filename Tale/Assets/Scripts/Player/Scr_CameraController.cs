using UnityEngine;
using System.Collections;

public class Scr_CameraController : MonoBehaviour {

	// Use this for initialization


    public Transform target;
    [System.Serializable]
    public class CameraPositionSettings
    {
        public Vector3 targetPosOffset = new Vector3(0, 3.4f, 0);
        public float m_lookSmooth = 100;
        public float m_distanceFromTarget = -8;
        public float m_zoomSpeed = 10;
        public float m_maxZoom = -2;
        public float minZoom = -15;

        public float m_CameraReturnTime;
    }
    [System.Serializable]
    public class AimingSettings
    {
        public float xRotation = -20;
        public float yRotation = -180;
        public float maxRotation = 25;
        public float minRotation = -25;
        public float vOrbitSmooth = 150;
        public float hOrbitSmooth = -150;
    }
    [System.Serializable]
    public class InputSettings
    {
        public string AIM_HORIZONTAL_SNAP = "AimHorizontalSnap";
        public string AIM_HORIZONTAL = "Mouse X";
        public string AIM_VERTICAL = "Mouse Y";
        public string ZOOM = "Fire2";
    }
    public CameraPositionSettings m_cameraPositionSettings = new CameraPositionSettings();
    public AimingSettings m_aimSettings = new AimingSettings();
    public InputSettings m_inputSettings = new InputSettings();

    Vector2 m_CameraRefVelocity = Vector2.zero;
    Vector2 m_DefaultCameraRotation;
    Vector3 m_targetPos = Vector3.zero;
    Vector3 m_destination = Vector3.zero;
    scr_PSM playerStateManager;

    scr_playerController m_charController;
    float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput;

    private float cameraReturnCounter;
    private bool m_returnCamera;
    private bool m_CameraIsOrbiting;



	void Start ()
    {
        SetCameraTarget(target);
        playerStateManager = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_PSM>();
       // m_previousPlayerState = GameObject.FindGameObjectWithTag("Player").GetComponent<scr_PSM>();

        m_targetPos = target.position + m_cameraPositionSettings.targetPosOffset;
        m_destination = Quaternion.Euler(m_aimSettings.xRotation, m_aimSettings.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * m_cameraPositionSettings.m_distanceFromTarget;
        m_destination += m_targetPos;
        transform.position = m_destination;
        m_DefaultCameraRotation = new Vector2(m_aimSettings.xRotation, m_aimSettings.yRotation);
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetInput();
        BowCamera();
        // If Player is running and not looking for certain amount of time.
        if(vOrbitInput == 0 && hOrbitInput == 0 && playerStateManager.GetPlayerPose(true) == scr_PSM.PlayerPose.pose_running)  
        {
            cameraReturnCounter += Time.deltaTime;
            if(cameraReturnCounter > m_cameraPositionSettings.m_CameraReturnTime)  
            {
                m_returnCamera = true;
                m_CameraIsOrbiting = false;
            }
        }
        //if player is looking
        else if (vOrbitInput != 0 && hOrbitInput != 0)
        {
            cameraReturnCounter = 0;
            OrbitTarget();
        }
        //if player is starting to move forward and player angle != camera angle.
        else if(playerStateManager.GetPlayerPose(true) == scr_PSM.PlayerPose.pose_idle && transform.rotation != m_charController.transform.rotation)
        {
        }

	}
    public bool IsOrbiting()
    {
        return m_CameraIsOrbiting;
    }
    public Quaternion GetRotation()
    {
        return transform.rotation;
    }
    public void SetCameraTarget(Transform t)
    {
        target = t;
        if(target != null)
        {
            if (target.GetComponent<scr_playerController>())
            {
                m_charController = target.GetComponent<scr_playerController>();
            }
            else
            {
                Debug.LogError("The camers target needs a characterController");
            }
        }
        else
        {
            Debug.LogError("Your camera needs a target");
        }
    }
    void GetInput()
    {
        vOrbitInput = Input.GetAxis(m_inputSettings.AIM_VERTICAL);
        hOrbitInput = Input.GetAxis(m_inputSettings.AIM_HORIZONTAL);
        zoomInput = Input.GetAxisRaw(m_inputSettings.ZOOM);
    }
    void LateUpdate()
    {
        MoveToTarget();
        LookAtTarget();
    }
    void MoveToTarget()
    {
        m_targetPos = target.position + m_cameraPositionSettings.targetPosOffset;
        if(!m_returnCamera)
        {
            m_destination = Quaternion.Euler(m_aimSettings.xRotation, m_aimSettings.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * m_cameraPositionSettings.m_distanceFromTarget; 
        }
        else if(m_returnCamera)
        {

           m_aimSettings.xRotation = Mathf.SmoothDamp(m_aimSettings.xRotation, m_DefaultCameraRotation.x, ref m_CameraRefVelocity.x, m_cameraPositionSettings.m_CameraReturnTime / 2.5f);
           m_aimSettings.yRotation = Mathf.SmoothDamp(m_aimSettings.yRotation, m_DefaultCameraRotation.y, ref m_CameraRefVelocity.y, m_cameraPositionSettings.m_CameraReturnTime / 2.5f);
           // m_destination = Quaternion.Euler(target.eulerAngles.x, target.eulerAngles.y + target.eulerAngles.z, 0) * -Vector3.forward * m_cameraPositionSettings.m_distanceFromTarget;
            m_destination = Quaternion.Euler(m_aimSettings.xRotation, m_aimSettings.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * m_cameraPositionSettings.m_distanceFromTarget; 
        }
        m_destination += m_targetPos;
        transform.position = m_destination;

    }
    void LookAtTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(m_targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_cameraPositionSettings.m_lookSmooth * Time.deltaTime);
    }
    void OrbitTarget()
    {
        m_CameraIsOrbiting = true;
        m_returnCamera = false;
        m_aimSettings.xRotation += vOrbitInput * m_aimSettings.vOrbitSmooth * Time.deltaTime;
        m_aimSettings.yRotation += -hOrbitInput * m_aimSettings.hOrbitSmooth * Time.deltaTime;

        if(m_aimSettings.xRotation > m_aimSettings.maxRotation)
        {
            m_aimSettings.xRotation = m_aimSettings.maxRotation;
        }
        else if (m_aimSettings.xRotation < m_aimSettings.minRotation)
        {
            m_aimSettings.xRotation = m_aimSettings.minRotation;
        }

    }
    void BowCamera()
    {
        if(zoomInput != 0)
        {
            m_cameraPositionSettings.m_distanceFromTarget += zoomInput * m_cameraPositionSettings.m_zoomSpeed * Time.deltaTime;
        }
        else if(zoomInput == 0  )
        {
            m_cameraPositionSettings.m_distanceFromTarget -= 1 * m_cameraPositionSettings.m_zoomSpeed * Time.deltaTime;
        }


        if(m_cameraPositionSettings.m_distanceFromTarget > m_cameraPositionSettings.m_maxZoom)
        {
            m_cameraPositionSettings.m_distanceFromTarget = m_cameraPositionSettings.m_maxZoom;
        }
        if (m_cameraPositionSettings.m_distanceFromTarget < m_cameraPositionSettings.minZoom)
        {
            m_cameraPositionSettings.m_distanceFromTarget = m_cameraPositionSettings.minZoom;
        }
    }
}
