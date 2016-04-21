using UnityEngine;
using System.Collections;

public class scr_generic_simpleMove : MonoBehaviour {

	// Use this for initialization
    public Transform goal;
	void Start () {
	}
	
	// Update is called once per frame
    void Update()
    {
        //Vector3 heading = goal.transform.position - this.transform.position;
        //float distanceFromGoal = Vector3.Distance(this.transform.position, goal.transform.position);
        //Vector3 directionToGoal = heading / distanceFromGoal;
        //if (distanceFromGoal > 0.01)
        //{
            
        //}
        this.transform.Translate(Vector3.forward * 4 * Time.deltaTime);
        //this.transform.position = goal.transform.position;
	}
}
