using UnityEngine;
using System.Collections;
[RequireComponent(typeof(scr_PSM))]
public class scr_playerClimbing : MonoBehaviour {

	// Use this for initialization
    public float climbingSpeed;
    public int ropeLayer;
    private GameObject jointParent;
    public Transform previousJoint, currentJoint, nextJoint;
    scr_PSM m_PSM;
    string currentJointName;
    public float pullForce;
    public ForceMode pullPushForceMode;
    [SerializeField]
    public string s_holdOntoRope, s_pullRope, s_pushRope;
    private bool currentlyClimbing;
    public string name_grabRopeButton;
    public string name_jumpButton;
    Rigidbody m_rgd;
    CapsuleCollider m_capsuleCollider;
    scr_CharacterMovement m_movementScript;
    bool currentlyAttached;
	void Start () {
        ropeLayer = LayerMask.NameToLayer("RopeLayer");
        m_PSM = GetComponent<scr_PSM>();
        currentlyClimbing = false;
        m_rgd = GetComponent<Rigidbody>();
        m_capsuleCollider = GetComponent<CapsuleCollider>();
        m_movementScript = GetComponent<scr_CharacterMovement>();
        currentlyAttached = false;
	}
	
	// Update is called once per frame
	void Update () {
       /* switch (m_ropeState)
        {
            case ropeState.ropestate_climbing:
                break;
            case ropeState.ropestate_skimming:
                break;
            case ropeState.ropestate_swinging:
                break;
        }*/
        //float hInput = Input.GetAxis("Horizontal");
        
        float vInput=Input.GetAxis("Vertical");
        if (currentlyAttached)
        {
            if (vInput > 0)
            {
                print("forward");
                TraverseForward(vInput);
                m_PSM.SetPlayerPose(scr_PSM.PlayerPose.pose_climbing);
            }
            else if (vInput < 0)
            {
                print("backward");
                TraverseBackward(vInput);
                m_PSM.SetPlayerPose(scr_PSM.PlayerPose.pose_climbing);
            }
        }
  
        if (Input.GetButton(s_pullRope))
        {
            PullRope();
            m_PSM.SetRopeState(scr_PSM.RopeState.ropestate_pulling);
        }
        if (Input.GetButton(s_pushRope))
        {
            PushRope();
        }
        if (Input.GetButton(s_holdOntoRope))
        {
            HoldOntoRope();
            m_PSM.SetPlayerPose(scr_PSM.PlayerPose.pose_climbing);
            m_PSM.SetRopeState(scr_PSM.RopeState.ropestate_hanging);
        }
        else if (Input.GetButtonUp(s_holdOntoRope))
        {
            m_PSM.SetRopeState(scr_PSM.RopeState.ropestate_none);
        }
	}
  
    public void AttachToRope()
    {
        print("Attaching to rope");
        m_PSM.SetPlayerPose(scr_PSM.PlayerPose.pose_climbing);
        m_rgd.useGravity = false;
        //m_capsuleCollider.enabled = false;
        m_rgd.isKinematic = true;
        m_movementScript.enabled = false;//temporary fix for movement turnoff
        currentlyAttached = true;
    }
    public void Deattach()
    {
        print("Deattaching from rope");
        m_PSM.SetPlayerPose(scr_PSM.PlayerPose.pose_standing);
        m_rgd.useGravity = true;
        //m_capsuleCollider.enabled = true;
        m_rgd.isKinematic = false;
        m_movementScript.enabled = true;
        currentlyAttached = false; 
    }
    public void UpdateJoints(Transform newJoint){
        if (newJoint != currentJoint)
        {
            int jointNumber= int.Parse(StringManip(newJoint));
            jointParent = newJoint.transform.parent.gameObject;
            currentJoint = newJoint;
            nextJoint = jointParent.transform.FindChild("Joint_" + (jointNumber + 1).ToString());
            previousJoint = jointParent.transform.FindChild("Joint_" + (jointNumber - 1).ToString());
        }
        else if (currentJoint == null)
        {
            int jointNumber = int.Parse(StringManip(newJoint));
            jointParent = newJoint.transform.parent.gameObject;
            currentJoint = newJoint;
            nextJoint = jointParent.transform.FindChild("Joint_" + (jointNumber + 1).ToString());
            previousJoint = jointParent.transform.FindChild("Joint_" + (jointNumber - 1).ToString());
        }
    }
    void HandleRopeCollisionAirBorne()
    {
        //set climbRopeAirBorne 
    }
    void HandleRopeCollisionGrounded()
    {
        //set holding rope on input? climb on input?
    }
    string StringManip(Transform getNameFromTransform)
    {
         string name = getNameFromTransform.name;
         int foundS1= name.IndexOf("_");
         name = name.Substring(foundS1+1, name.Length - 1 - foundS1);
         //print(name);
         return name;
    }
    void TraverseForward(float inputStr){
        if (nextJoint != null)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, nextJoint.position, climbingSpeed*inputStr*Time.deltaTime);//get axis later, +=movement
        }
    }
    void TraverseBackward(float inputStr){
        if (previousJoint != null)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, previousJoint.position, climbingSpeed*-inputStr*Time.deltaTime);
        }
        
    }
    void PullRope()
    {
        currentJoint.GetComponent<Rigidbody>().AddForce(-Vector3.up * pullForce,pullPushForceMode); //get players backwards
    }
    void PushRope()
    {
        currentJoint.GetComponent<Rigidbody>().AddForce(Vector3.up * pullForce, pullPushForceMode);
    }
    void HoldOntoRope()
    {
        this.transform.position = currentJoint.transform.position;
        //this.transform.rotation = currentJoint.transform.rotation;
    }

    void JumpOffRope()
    {
        //let go somehow or add normal script and remove this script?
    }
}
