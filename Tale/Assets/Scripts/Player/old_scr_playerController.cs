using UnityEngine;
using System.Collections;

namespace playerenums
{
    public enum PlayerState
    {
        Idle, Running, Jumping, Climbing, Aiming,
    };
}
[RequireComponent(typeof(Rigidbody))]
public class scr_playerController : MonoBehaviour {


    [System.Serializable]
    public class MoveSettings
    {
        public float m_ForwardSpeed;
        public float m_RotationSpeed;
        public float m_JumpSpeed;
        public float m_DistanceToGround;

        public LayerMask m_groundLayer;

    }
    [System.Serializable]
    public class PhysSettings
    {
        public float m_DownAccel;
    }
    [System.Serializable]
    public class InputSettings
    {
        public float m_inputDelay;
        public string FORWARD_AXSIS = "Vertical";
        public string TURN_AXSIS = "Horizontal";
        public string JUMP_AXSIS = "Jump";
    }


    public MoveSettings m_moveSettings = new MoveSettings();
    public PhysSettings m_physSeetings = new PhysSettings();
    public InputSettings m_inputSettings = new InputSettings();
    scr_PSM playerStateManager;
    Scr_CameraController m_cameraController;
    Vector3 velocity = Vector3.zero;

    Quaternion m_targetRotation;
    Rigidbody m_rb;

    float forwardInput, turnInput, jumpInput;
    float m_refValue = 0;

    bool Grounded()
    {
        if(Physics.Raycast(transform.position, Vector3.down, m_moveSettings.m_DistanceToGround))
        {
            playerStateManager.SetPlayerState(scr_PSM.Playerstate.state_grounded);
        }
        else
        {
            playerStateManager.SetPlayerState(scr_PSM.Playerstate.state_airborne);
        }
        return Physics.Raycast(transform.position, Vector3.down, m_moveSettings.m_DistanceToGround);
    }
	// Use this for initialization
	void Start () 
    {
        m_targetRotation = transform.rotation;
        playerStateManager = this.gameObject.GetComponent<scr_PSM>();
        m_cameraController = Camera.main.GetComponent<Scr_CameraController>();

        if (GetComponent<Rigidbody>())
        {
            m_rb = GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("The player needs a rigidbody");
        }
        forwardInput = 0; turnInput = 0; jumpInput = 0;
	}
    public Vector3 GetVelocity()
    {
        return m_rb.velocity;
    }

    void GetInput()
    {
        forwardInput = Input.GetAxis(m_inputSettings.FORWARD_AXSIS); 
        turnInput = Input.GetAxis(m_inputSettings.TURN_AXSIS); // ^ + this = interpolated
        jumpInput = Input.GetAxisRaw(m_inputSettings.JUMP_AXSIS); // not interpoaltesds
    }

	// Update is called once per frame
	void Update () 
    {
        GetInput();
        Turn();
	}

    void FixedUpdate()
    {
        Run();
        Jump();

        m_rb.velocity = transform.TransformDirection(velocity);
    }
    public void SetDownAcceleratin(float value)
    {
        m_physSeetings.m_DownAccel = value;
    }
    public float GetDownAcceleration()
    {
        return m_physSeetings.m_DownAccel;
    }
    void Run()
    {
        if(Mathf.Abs(forwardInput) > m_inputSettings.m_inputDelay)
        {
            playerStateManager.SetPlayerPose(scr_PSM.PlayerPose.pose_running);
            velocity.z = m_moveSettings.m_ForwardSpeed * forwardInput;
        }
        else
        {
            playerStateManager.SetPlayerPose(scr_PSM.PlayerPose.pose_idle);
            velocity.z = 0;
        }
    }
    public void SetTargetRotation(Quaternion rotation)
    {
        m_targetRotation = rotation;
    }
    public Quaternion GetTargetRotation()
    {
        return m_targetRotation;
    }
    void TurnCharacterToCameraRotation()
    {
        m_targetRotation.y = Mathf.SmoothDamp(m_targetRotation.y, -m_cameraController.GetRotation().y, ref m_refValue, 0.2f);
    }
    void Turn()
    {
        if(playerStateManager.GetPlayerPose(true) == scr_PSM.PlayerPose.pose_idle)
        {
            //TurnCharacterToCameraRotation();
            //m_targetRotation *= Quaternion.AngleAxis(m_moveSettings.m_RotationSpeed * turnInput * Time.deltaTime, Vector3.up);

        }
        if(playerStateManager.GetPlayerPose(true) == scr_PSM.PlayerPose.pose_running)
        {
            m_targetRotation *= Quaternion.AngleAxis(m_moveSettings.m_RotationSpeed * turnInput * Time.deltaTime, Vector3.up);

        }
        transform.rotation = m_targetRotation;
    }
    void Jump()
    {
        if(jumpInput > 0 && Grounded())
        {
            velocity.y = m_moveSettings.m_JumpSpeed;
        }
        else if(jumpInput == 0 && Grounded())
        {
            velocity.y = 0;
        }
        else   //Gravity
        {
            velocity.y -= m_physSeetings.m_DownAccel;
        }
    }
}
