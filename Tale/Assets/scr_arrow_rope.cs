using UnityEngine;
using System.Collections;

public class scr_arrow_rope : MonoBehaviour
{

    // Use this for initialization
    Rigidbody m_rgd;
    Collider m_collider;
    public LayerMask obstacleLayer;
    public LayerMask vurnerableLayer;
    public LayerMask ropeAttachLayer;
    [SerializeField]
    private float arrowStuckDuration;
    public GameObject Player;

    void Start()
    {
        m_collider = GetComponent<Collider>();
        m_rgd = GetComponent<Rigidbody>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    public void AddVelocity(Vector3 dir, float velocity)
    {
        m_rgd.AddForce(dir * velocity);
    }
    public void OnProjectileSpawn()
    {
        m_rgd = GetComponent<Rigidbody>();
    }
    void OnCollisionEnter(Collision colli)
    {
        if ((obstacleLayer.value & 1 << colli.gameObject.layer) == 1 << colli.gameObject.layer)
        {
            Destroy(this.gameObject);
        }
        else if ((vurnerableLayer.value & 1 << colli.gameObject.layer) == 1 << colli.gameObject.layer)
        {
            MakeArrowIntoProp(colli.transform);
            StartCoroutine(DestroyProjectile(arrowStuckDuration));
        }
        else if ((ropeAttachLayer.value & 1 << colli.gameObject.layer) == 1 << colli.gameObject.layer)
        { 
            //raycast between poitns to see if valid distance and no crap in way;
            RaycastBetweenValidPoints(Player.transform.position, colli.contacts[0].point);
        }
    }
    void RaycastBetweenValidPoints(Vector3 fromPoint, Vector3 toPoint)
    {
         RaycastHit hit;
        Vector3 directionTo= toPoint-fromPoint.normalized;
        float distanceTo=Vector3.Distance(toPoint, fromPoint);
                // ... and if a raycast towards the player hits something...
                if (Physics.Raycast(fromPoint, directionTo, out hit,distanceTo))
                {
                    // ... and if the raycast hits the player...
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        // ... the player is in sight.

                        // Set the last global sighting is the players current position.
                        //lastPlayerSighting.position = player.transform.position;
                    }
                    else if (hit.collider == null)
                    {
                        Debug.Log("I hit nathing good");
                    }
                    if (hit.collider)
                    {
                        Debug.Log("hit something");
                    }
                }
}
    void MakeArrowIntoProp(Transform p_targetParent)
    {
        this.transform.parent = p_targetParent;
        m_rgd.velocity = new Vector3(0, 0, 0);
        Destroy(m_rgd);
        Destroy(m_collider);
    }
    IEnumerator DestroyProjectile(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(this.gameObject);

    }
}
