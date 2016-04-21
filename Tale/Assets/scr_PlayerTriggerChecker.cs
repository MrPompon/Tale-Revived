using UnityEngine;
using System.Collections;

public class scr_PlayerTriggerChecker : MonoBehaviour {

	// Use this for initialization
    public GameObject player;
    public int ropeLayer;
    scr_playerClimbing player_climbing;
    public string name_grabRopeButton;
    public string name_jumpButton;
	void Start () {
        player_climbing = player.GetComponent<scr_playerClimbing>();
        ropeLayer = LayerMask.NameToLayer("RopeLayer");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerStay(Collider colli)
    {
        if (colli.gameObject.layer == ropeLayer)
        {
            player_climbing.UpdateJoints(colli.transform);

            if (Input.GetButton(name_grabRopeButton))
            {
                player_climbing.AttachToRope();
            }
            if (Input.GetButtonDown(name_jumpButton))
            {
                player_climbing.Deattach();
            }

            /*switch (m_playerState)
            {
                case playerstate.state_airborne:
                    HandleRopeCollisionAirBorne();
                    break;
                case playerstate.state_grounded:
                    HandleRopeCollisionGrounded();
                    break;
            }*/
        }
    }
}
