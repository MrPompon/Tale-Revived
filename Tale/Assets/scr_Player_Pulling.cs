using UnityEngine;
using System.Collections;

public class scr_Player_Pulling : MonoBehaviour {

   // public GameObject currentlyAttachedObject;
    // Use this for initialization
    private float attachedMass; //should alter pulling speed
    private bool isAttached;
    float distanceFromPlayer;
    [SerializeField]
    private float closestPullRange;//move to player //should be changed depending of size of object
    [HideInInspector]
    public float pushForce;
    public LayerMask pushableLayer;
    public void PullRope(Transform p_attachedObject, float p_pullforce)
    {
        distanceFromPlayer = Vector3.Distance(this.transform.position, p_attachedObject.transform.position);
        float tCollSize = p_attachedObject.transform.GetComponent<Collider>().bounds.size.x;
        closestPullRange =tCollSize +tCollSize/4;
        if (distanceFromPlayer > closestPullRange)
        {
            p_attachedObject.transform.Translate((-GetDirectionTo(p_attachedObject) * p_pullforce) * Time.deltaTime, Space.World);
        }
    }
    public void Push(Transform p_attachedObject,Vector3 p_dir, float p_pushforce)
    {
        p_attachedObject.transform.Translate((GetDirectionTo(p_attachedObject) * p_pushforce) * Time.deltaTime, Space.World);
    }
    Vector3 GetDirectionTo(Transform target)
    {
        Vector3 heading = target.transform.position - this.transform.position;
        distanceFromPlayer = Vector3.Distance(this.transform.position, target.transform.position);
        Vector3 directionToAttached = heading / distanceFromPlayer;
        return directionToAttached;
    }
    //void OnCollisionStay(Collision colli)
    //{
    //    //add some input cooldown or smth   
    //      if((pushableLayer.value & 1<<colli.gameObject.layer) == 1<<colli.gameObject.layer){
    //        Push(colli.transform, GetDirectionTo(colli.transform), pushForce);
    //        print("Pushing");
           
    //    }
    //}
}
