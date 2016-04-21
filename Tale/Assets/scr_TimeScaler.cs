using UnityEngine;
using System.Collections;

public class scr_TimeScaler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Time.timeScale = 2.0f;
            print("DoubleTime");
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Time.timeScale = 0.5f;
            print("HalfTime");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Time.timeScale = 1.0f;
            print("NormalTime");
        }
	}
}
