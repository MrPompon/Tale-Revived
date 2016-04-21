using UnityEngine;
using System.Collections;

public class scr_pullable_object : MonoBehaviour {

	// Use this for initialization
    public float mass; //should alter pulling speed
    private bool isAttached;
    public GameObject testPlayer;
    public LineRenderer m_line;
    float distanceFromPlayer;
    public float closestPullRange;//move to player //should be changed depending of size of object
    Vector3 directionToPlayer;
	void Start () {
        m_line.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        //should probably be velocity of player and called from player
        GetDirectionToPlayer();
        AtAttach();
        Pull(directionToPlayer,10);
        //Push(testPlayer.transform.forward, 20);
        

	}
    public float GetMass()
    {
        return mass;
    }
    public void Pull(Vector3 p_dir,float pullforce)
    {
        if (distanceFromPlayer >= closestPullRange)
        {
            this.transform.Translate((p_dir * pullforce/mass) * Time.deltaTime,Space.World);
        }
    }
    public void Push(Vector3 p_dir, float pushforce)
    {
        this.transform.Translate((p_dir * pushforce/mass)*Time.deltaTime,Space.World);
    }
    void GetDirectionToPlayer()
    {
        Vector3 heading= testPlayer.transform.position - this.transform.position;
        distanceFromPlayer = Vector3.Distance(this.transform.position, testPlayer.transform.position);
        directionToPlayer = heading / distanceFromPlayer;
        
    }
    void AtAttach()
    {
        closestPullRange = this.transform.GetComponent<BoxCollider>().bounds.size.x;
    }
}
