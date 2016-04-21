using UnityEngine;
using System.Collections;

public class scr_checkpointBehaviour : MonoBehaviour {
    private GameObject m_player;
    private GameObject m_LatestCheckPoint;

	// Use this for initialization
	void Start () 
    {
        m_player = this.gameObject;
        m_LatestCheckPoint = null;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    void OnTriggerEnter(Collider colli)
    {
        if(colli.gameObject.CompareTag("checkpoint"))
        {
            Debug.Log("Checkpoint Reached");
            m_LatestCheckPoint = colli.gameObject;
        }
        else if(colli.gameObject.CompareTag("death"))
        {
            transform.position = m_LatestCheckPoint.transform.position;
        }
    }
}
