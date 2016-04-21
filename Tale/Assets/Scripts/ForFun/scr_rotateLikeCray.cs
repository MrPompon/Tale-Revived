using UnityEngine;
using System.Collections;

public class scr_rotateLikeCray : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // Rotate the object around its local Y axis at 1 degree per second
        transform.Rotate(Vector3.right, Time.deltaTime*130,Space.World);

        // ...also rotate around the World's Y axis
        transform.Rotate(Vector3.up, Time.deltaTime*130, Space.World);
	}
}
