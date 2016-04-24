using UnityEngine;
using System.Collections;

public class StatePatternEnemy : MonoBehaviour {

	// Use this for initialization
    public float searchTurnSpeed = 120.0f;
    public float searchDuration = 4.0f;
    public Transform[] waypoints;
    public Transform eyes;//raypoint start
    [HideInInspector]
    public float sightRange;
    public Vector3 offset = new Vector3(0, 0.5f, 0); //lookoffset
    Vector3 prevFramePos;

    public float attackRange = 4.0f;
    public float attackWindUpDuration = 0.4f;
    public float attackDuration = 1.0f;
    public float attackDownDuration = 1.4f;

    public Transform swingSimulationPoint;
    public Transform[] weaponWayPoints;
    public LayerMask playerLayer;
    [HideInInspector]public LineRenderer weaponSwingRenderer;
    public float FOV_angle;
    public SphereCollider m_sphereCol;
    public bool drawGizmos;

    [HideInInspector]public Animator m_animator;
    public float animClipTimeLeft; 

    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;
    [HideInInspector]public Transform chaseTarget;
    [HideInInspector]public IEnemyState currentState;
    [HideInInspector]public AlertState alertState;
    [HideInInspector]public ChaseState chaseState;
    [HideInInspector]public PatrolState patrolState;
    [HideInInspector]public AttackState attackState;
    [HideInInspector]public RetreatState retreatState;
    [HideInInspector]public NavMeshAgent navMeshAgent;
    private void Awake()
    {
        prevFramePos = this.transform.position;
        chaseState = new ChaseState(this);
        alertState = new AlertState(this);
        patrolState = new PatrolState(this);
        attackState = new AttackState(this);
        retreatState = new RetreatState(this);

        attackState.Start(); // note to self call start on states that it is needed
        
        navMeshAgent = GetComponent<NavMeshAgent>();
        weaponSwingRenderer = GetComponent<LineRenderer>();
        m_sphereCol = GetComponent<SphereCollider>();
        m_animator = GetComponent<Animator>();
    }
	void Start () {
        chaseTarget = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = patrolState;
        sightRange = m_sphereCol.radius;
	}
	
	// Update is called once per frame
	void Update () {
        currentState.UpdateState();
        UpdateAnimator();
	}
    void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.blue;
            if (waypoints.Length > 0)
            {
                for (int i = 0; i < waypoints.Length; i++)
                {
                    Gizmos.DrawCube(waypoints[i].position, new Vector3(1, 1, 1));
                }
            }
            Gizmos.color = Color.red;
            if (weaponWayPoints.Length > 0)
            {
                for (int i = 0; i < weaponWayPoints.Length-1; i++)
                {
                    Gizmos.DrawCube(weaponWayPoints[i].position, new Vector3(0.5f, 0.5f, 0.5f));
                    if (weaponWayPoints.Length > 1)
                    {
                        if (i < weaponWayPoints.Length + 1)
                        {
                            Gizmos.DrawLine(weaponWayPoints[i].position, weaponWayPoints[i + 1].position);
                        }
                    }
                }
            }
        }
    }
    private void UpdateAnimator(){
        //http://docs.unity3d.com/Manual/nav-CouplingAnimationAndNavigation.html

        Vector3 worldDeltaPosition = transform.position-prevFramePos  ;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot (transform.right, worldDeltaPosition);
        float dy = Vector3.Dot (transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2 (dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
        smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.5f && navMeshAgent.remainingDistance > navMeshAgent.radius;

        // Update animation parameters
        m_animator.SetBool("shouldMove", shouldMove);
        m_animator.SetFloat("Forward", velocity.y);
        m_animator.SetFloat("Turn", navMeshAgent.velocity.x);
        //print(velocity);
        prevFramePos = this.transform.position;
        UpdateClipLength();
    }
    IEnumerator UpdateClipLength()
    {
        yield return new WaitForEndOfFrame();
        print("current clip length = " + m_animator.GetCurrentAnimatorStateInfo(0).length);
        animClipTimeLeft = m_animator.GetCurrentAnimatorStateInfo(0).length;

    }
    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }
}
