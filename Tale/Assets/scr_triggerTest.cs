using UnityEngine;
using System.Collections;

public class scr_triggerTest : MonoBehaviour {
    scr_triggerReciever m_reciever;
	// Use this for initialization
	void Start () {
        m_reciever = GetComponent<scr_triggerReciever>();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_reciever.GetTriggerDuration() > 0)
        {
            print("thing active");
        }
	}
}
