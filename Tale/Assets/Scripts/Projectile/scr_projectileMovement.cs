using UnityEngine;
using System.Collections;

public class scr_projectileMovement : MonoBehaviour
{

    // Use this for initialization
    Rigidbody m_rgd;
    Collider m_collider;
    public LayerMask obstacleLayer;
    public LayerMask vurnerableLayer;
    public LayerMask livingLayer;
    [SerializeField]
    private float arrowStuckDuration, orginArrowSoarDuration;
    private float currentSoarDuration;
    public int arrowDamage;
    RaycastHit hit;
    private GameObject projectileOriginator;
    DontGoThroughThings DGTT;
    [SerializeField]
    private GameObject playerRopeAttachPoint;
    private GameObject player;
    private scr_attachRopeTo playerAttach;
    [HideInInspector]
    public GameObject singletonHandler;
    scr_playerStateFunctions PSF;
    LineRenderer m_lineRenderer;
    bool shouldBeDestroyed;
    bool inAir;
    bool ropeIsAttachedToArrow;
    void Start()
    {
        m_collider = GetComponent<Collider>();
        m_rgd = GetComponent<Rigidbody>();
        DGTT = GetComponent<DontGoThroughThings>();
        singletonHandler = GameObject.FindGameObjectWithTag("SingletonHandler");
        PSF = singletonHandler.GetComponent<scr_playerStateFunctions>();
        player = GameObject.FindGameObjectWithTag("Player");
        m_lineRenderer = GetComponent<LineRenderer>();
        currentSoarDuration = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        playerAttach = player.GetComponent<scr_attachRopeTo>();
        shouldBeDestroyed = false;
    }

    void Update()
    {
        if (m_rgd)
        {
            if (m_rgd.velocity.magnitude > 0.1)
            {
                Quaternion dirQ = Quaternion.LookRotation(m_rgd.velocity);
                Quaternion slerp = Quaternion.Slerp(transform.rotation, dirQ, m_rgd.velocity.magnitude * 3.0f * Time.deltaTime);
                m_rgd.MoveRotation(slerp);
            }
        }
        if (m_lineRenderer)
        {
            RenderRopeLine();
        }
        if (m_collider)//if still in air
        {
            currentSoarDuration += Time.deltaTime;
            if (currentSoarDuration > orginArrowSoarDuration)
            {
                SetShouldBeDestroyed();
            }
        }
   
    }
    public void SetRopeAttachedToArrow(bool trueOrFalse)
    {
        ropeIsAttachedToArrow = trueOrFalse;
    }
    void LateUpdate()
    {
        if (shouldBeDestroyed)
        {
            Destroy(this.gameObject);
        }
    }
    public void SetProjectileOriginator(GameObject p_originator)
    {
        projectileOriginator = p_originator;
    }
    public void AddVelocity(Vector3 dir, float velocity)
    {
        m_rgd.AddForce(dir * velocity);
    }
    public void OnProjectileSpawn()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAttach = player.GetComponent<scr_attachRopeTo>();
        m_rgd = GetComponent<Rigidbody>();
        inAir = true;
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    void OnTriggerEnter(Collider colli)
    {
        if ((obstacleLayer.value & 1 << colli.gameObject.layer) == 1 << colli.gameObject.layer)
        {
            Destroy(this.gameObject);
        }
        else if ((vurnerableLayer.value & 1 << colli.gameObject.layer) == 1 << colli.gameObject.layer && !colli.isTrigger)
        {
            MakeArrowIntoProp(colli.transform);
            //attach to thing
            if (ropeIsAttachedToArrow)
            {
                PSF.SetTethered(this.transform.GetComponent<Collider>(), colli);
            }

            //StartCoroutine(DestroyProjectile(arrowStuckDuration));
            Invoke("DestroyProjectile", arrowStuckDuration);
        }
        else if ((livingLayer.value & 1 << colli.gameObject.layer) == 1 << colli.gameObject.layer && !colli.isTrigger)
        {
            scr_healthManager hitHealth;
            if (transform.parent != null)
            {
                hitHealth = colli.gameObject.transform.parent.transform.GetComponent<scr_healthManager>();
            }
            else
            {
                hitHealth = colli.gameObject.transform.GetComponent<scr_healthManager>();
            }
            hitHealth.DealDamage(arrowDamage);
            MakeArrowIntoProp(colli.transform);
            Invoke("DestroyProjectile", arrowStuckDuration);
            //StartCoroutine(DestroyProjectile(arrowStuckDuration));
        }
    }
    void SetShouldBeDestroyed()
    {
        shouldBeDestroyed = true;
    }
    void MakeArrowIntoProp(Transform p_targetParent)
    {
        this.transform.parent = p_targetParent;
        if (m_rgd != null)
        {
            m_rgd.velocity = new Vector3(0, 0, 0);
            Destroy(m_rgd);
        }
        DGTT.enabled = false;
        inAir = false;
        m_lineRenderer.enabled = false;
        Destroy(m_collider);
        if (p_targetParent != null)
        {
            this.transform.parent = p_targetParent;
            if (m_rgd != null)
            {
                m_rgd.velocity = new Vector3(0, 0, 0);
                Destroy(m_rgd);
            }

            Destroy(m_collider);
        }
    }
    void DestroyProjectile()
    {
        //if player is attached to this, deattach player 
        if (this.transform != null)
        {
            SetShouldBeDestroyed();
            if (playerAttach.GetTetherObject().GetInstanceID() != null)
            {
            if (playerAttach.GetTetherObject().GetInstanceID() == this.transform.GetInstanceID())
            {
                //PSF.SetRunning();
                PSF.SetRunning();
                PSF.DeattachTether();
                print("DEAATTACHING ROPE, SINCE THE ARROW THAT IM STUCK ON IS NOW GAWN");
            }
            }
        }
    }
    void RayCastingCollision()
    {
        Debug.DrawRay(transform.position, transform.forward);

        if (Physics.Raycast(transform.position, transform.forward, out hit, vurnerableLayer))
        {
            MakeArrowIntoProp(hit.transform);
            Invoke("DestroyProjectile", arrowStuckDuration);
            //StartCoroutine(DestroyProjectile(arrowStuckDuration));
        }
    }
    void RenderRopeLine()
    {
        m_lineRenderer.SetPosition(0, this.transform.position);
        m_lineRenderer.SetPosition(1, player.transform.position);
    }
}
