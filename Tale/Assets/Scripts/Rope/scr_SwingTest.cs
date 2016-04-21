using UnityEngine;
using System.Collections;

public class scr_SwingTest : MonoBehaviour {

	// Use this for initialization
    //movement
    Vector3 currentPos;
    Vector3 previousPos;
    Rigidbody m_rgd;
    Vector3 acceleration;
    Vector3 lastVelocity;
    [SerializeField]
    float maxVelocity, minVelocity;
    //teather
    [SerializeField]
    float maximumTeatherLength;
    public Transform aPoint;
    float distanceFromPlayer;
    Vector3 directionToPlayer;
    bool amITethered;
    Vector3 tetherPoint;
    float tetherLength;
    public Transform playerBody;

    void GetDirectionToTarget()
    {
        Vector3 heading = aPoint.position - playerBody.transform.position;
        distanceFromPlayer = Vector3.Distance(playerBody.transform.position, aPoint.transform.position);
        directionToPlayer = heading / distanceFromPlayer;

    }
	void Start () {
        currentPos = playerBody.transform.position;
        previousPos = currentPos;
        m_rgd = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        CalculateMovement();
        CalculateTeather(aPoint.position);
        DrawCrap();
        //CollisionStuff(this.transform.position, m_rgd.velocity);
        
	}
    void LateUpdate()
    {
        previousPos = playerBody.transform.position;//do at end
        lastVelocity = m_rgd.velocity;
    }
    Vector3 VelocityMaintainer()
    {
        //check max
        Vector3 newVelocity=m_rgd.velocity;
        if (m_rgd.velocity.x > maxVelocity)
        {
            newVelocity = new Vector3(maxVelocity, m_rgd.velocity.y, m_rgd.velocity.z);
        }
        if (m_rgd.velocity.y > maxVelocity)
        {
            newVelocity = new Vector3(m_rgd.velocity.x ,maxVelocity, m_rgd.velocity.z);
        }
        if (m_rgd.velocity.z > maxVelocity)
        {
            newVelocity = new Vector3(m_rgd.velocity.x, m_rgd.velocity.y, maxVelocity);
        } 
        //check min
        if (m_rgd.velocity.x < minVelocity)
        {
            newVelocity = new Vector3(minVelocity, m_rgd.velocity.y, m_rgd.velocity.z);
        }
        if (m_rgd.velocity.y < minVelocity)
        {
            newVelocity = new Vector3(m_rgd.velocity.x, minVelocity, m_rgd.velocity.z);
        }
        if (m_rgd.velocity.z < minVelocity)
        {
            newVelocity = new Vector3(m_rgd.velocity.x, m_rgd.velocity.x, minVelocity);
        }
        return newVelocity;
    }
    void CalculateMovement()
    { //avatar.position = avatar.position + (avatar.oldvelocity + avatar.velocity) * deltaT / 2.0f
        currentPos = playerBody.transform.position;

        Vector3 velocity = (currentPos - previousPos) / Time.deltaTime;

        acceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;

        m_rgd.velocity = m_rgd.velocity + acceleration * Time.deltaTime;
        m_rgd.velocity=VelocityMaintainer();
        currentPos = playerBody.transform.position + (lastVelocity + m_rgd.velocity) * Time.deltaTime / 2.0f;
        playerBody.transform.position = playerBody.transform.position + (lastVelocity + m_rgd.velocity) * Time.deltaTime / 2.0f;
        
        
        // print(m_rgd.velocity);
    }
    void CalculateTeather(Vector3 teatherPoint)
    {
        float teatherStretchLength = Vector3.Distance(playerBody.transform.position, teatherPoint);
        //testPosition = (testPosition - tetherPoint).Normalized() * tetherLength;}

        if (teatherStretchLength > maximumTeatherLength)
        {
            Vector3 teatherFixPoint;
            teatherFixPoint = ((playerBody.transform.position - teatherPoint).normalized) * maximumTeatherLength;
            //m_rgd.velocity = (previousPos - playerBody.transform.position) / Time.deltaTime;
            playerBody.transform.position = teatherFixPoint+aPoint.position;
            Debug.DrawLine(playerBody.transform.position, teatherFixPoint, Color.red); 
        }
        //print(teatherStretchLength);
    }
    void DrawCrap()
    {
        Debug.DrawLine(playerBody.transform.position, aPoint.transform.position);
       
    }
    void GetPoint()
    {
        RaycastHit wallData;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out wallData, maximumTeatherLength))
        {
            amITethered = true;
            tetherPoint = wallData.point;
            tetherLength = Vector3.Distance(wallData.point, playerBody.position);
        }
    }
    //void CollisionStuff(Vector3 pos, Vector3 vel)
    //{
    //    //Vector testPosition = avatar.position + avatar.velocity * deltaT;
    //    //Vector intersection
    //    //if( RayCast( position, testPosition, out intersection ))
    //    //{
    //    //    // we went through a wall, let's pull the character back,
    //    //    // using the normal of the wall
    //    // // with a 1-unit space for breathing room
    //    // testPosition = intersection + intersection.normal;
    //    //}
    //    Vector3 testPosition = pos + vel * Time.deltaTime;
    //    RaycastHit intersection;
    //    Debug.DrawLine(pos, testPosition, Color.black,2.0f);
    //    if (Physics.Raycast(pos, testPosition, out intersection))
    //    {
    //        testPosition = intersection.point + intersection.normal;
    //        m_rgd.velocity = ((testPosition - this.transform.position) / Time.deltaTime);
    //        VelocityMaintainer();
    //        this.transform.position = testPosition;
    //        print(intersection.transform.name);
    //        print(intersection.point);
    //    }
    //}
}
