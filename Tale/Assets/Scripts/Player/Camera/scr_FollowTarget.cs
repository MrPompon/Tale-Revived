using UnityEngine;
public abstract class scr_FollowTarget : MonoBehaviour {

    [SerializeField] public Transform target;
    [SerializeField] private bool autoTargetPlayer = true;

	// Use this for initialization
	virtual protected void Start () 
    {
        if(autoTargetPlayer)
        {
            FindTargetPlayer();
        }
	
	}
    void FixedUpdate()
    {
        if (autoTargetPlayer && (target == null || !target.gameObject.activeSelf))
        {
            FindTargetPlayer();
        }

        if (target != null && (target.GetComponent<Rigidbody>() != null && !target.GetComponent<Rigidbody>().isKinematic))
        {
            Follow(Time.deltaTime);
        }
    }
    protected abstract void Follow(float deltatime);
	
	// Update is called once per frame
	void Update () 
    {
        
	}
    public void FindTargetPlayer()
    {
        if(target == null)
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag("Player");
            if(targetObj)
            {
                SeTarget(targetObj.transform);
            }
        }
        
    }

    public virtual void SeTarget(Transform newTransform)
    {
        target = newTransform;
    }
    public Transform Target { get { return this.target; } }
}
