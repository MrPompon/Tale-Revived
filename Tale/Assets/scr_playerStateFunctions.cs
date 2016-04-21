using UnityEngine;
using System.Collections;

public class scr_playerStateFunctions : MonoBehaviour {

	// Use this for initialization
    private GameObject m_player;
    scr_attachRopeTo pAttach;
    DontGoThroughThings DGTT;
    scr_ThirdPersonUserControl pInput;
    Rigidbody pRigidbody;
    scr_ThirdPersonCharacter pMove;
	void Start () {
        m_player = GameObject.FindGameObjectWithTag("Player");
        if (m_player == null)
        {
            print("Couldnt Find Player");
            Destroy(this);
            return;
        }
        pRigidbody = m_player.GetComponent<Rigidbody>();
        pAttach = m_player.GetComponent<scr_attachRopeTo>();
        DGTT = m_player.GetComponent<DontGoThroughThings>();
        pInput = m_player.GetComponent<scr_ThirdPersonUserControl>();
        pMove = m_player.GetComponent<scr_ThirdPersonCharacter>();
	}
    public void SetTethered(Collider p_attachArrow, Collider p_arrowAttachCollider)
    {
        //pAttach.enabled = true;
        if(Vector3.Distance(p_attachArrow.transform.position, this.transform.position) <= pAttach.GetMaxTetherLength())
        {
            pAttach.SetTeatherObject(p_attachArrow.gameObject.transform);
            pAttach.SetAttachedArrowsHitTransform(p_arrowAttachCollider.transform);
            pAttach.SetRoperenderingpoints(m_player.transform, p_attachArrow.transform);
            pAttach.SetAmITethered(true);
            print(pAttach.GetMaxTetherLength());
            print(Vector3.Distance(p_attachArrow.transform.position, this.transform.position)); 
        }
        else
        {
            print("SNAP, MAX TETHER LENGTH IS SHORTER THEN TRIED TO ATTACH-LENGTH");
        }
    }
    public void DeattachTether()
    {
        //pAttach.enabled = false;
        pAttach.SetAmITethered(false);
        pAttach.RemoveTetherObject();              
    }
    public void DeattachJump()
    {
        pAttach.JumpAtDetach();        
    }
    public void SetSwinging()
    {
        ////pInput.enabled = false;
        //pAttach.SetWantsToSwing(true);
        //pInput.currentlyDisabled = true;
        //pMove.enabled = false;
        DGTT.enabled = true;
       
        ////FixRigidbodyConstraints(true, true, true);
        ////pRigidbody.useGravity = false;
        //pRigidbody.constraints = RigidbodyConstraints.None;
        pRigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
        pRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //SetRigidBody(1, 0.1f, 0.05f, false, false);
      
        //print("AttachedAgain");
    }
    public void SetRunning()
    {
        pAttach.SetAmITethered(false);
        //pAttach.SetWantsToSwing(false);
        //pRigidbody.useGravity = true;
        pInput.enabled = true;
        pInput.currentlyDisabled = false;
        pMove.enabled = true;
        //DGTT.enabled = false;

        //FixRigidbodyConstraints(true, true, true);
        pRigidbody.constraints = RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationZ;
        pRigidbody.interpolation = RigidbodyInterpolation.None;
        pRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        pRigidbody.isKinematic = false;

        SetRigidBody(1, 0f, 0.05f, true, false);
        print("RunningAgain");
    }
    void SetRigidBody(float mass, float drag, float angDrag, bool useGravity, bool isKinematic)
    {
        pRigidbody.mass = mass;
        pRigidbody.drag = drag;
        pRigidbody.angularDrag = angDrag;
        pRigidbody.useGravity = useGravity;
        pRigidbody.isKinematic = isKinematic;
    }
    void FixRigidbodyConstraints(bool xLocked, bool yLocked, bool ZLocked) //check if working not confirmed
    {
        if (xLocked)
        {
            pRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
        }
        if (yLocked)
        {
            pRigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
        }
        if (ZLocked)
        {
            pRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
        }
      
    }
    public void SetClimbing()
    {

    }
 
}
