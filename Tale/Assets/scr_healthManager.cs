using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class scr_healthManager : MonoBehaviour {

	// Use this for initialization
    [SerializeField]
    private int m_health;
    private bool m_alive;
    [SerializeField]
    private bool fragmentAtDeath;
    public Transform destructFrom;
    private Animator m_animator;
    private NavMeshAgent m_navMesh;
    private StatePatternEnemy m_patternEnemy;
    private StatePatternEnemyRanged m_patternEnemyRanged;

    [SerializeField]string[] destructorPartsNames;
    List<Transform> destructorTransforms;

    public SkinnedMeshRenderer m_skinnedRenderer;
    CapsuleCollider m_capsuleCollider;

    [SerializeField] float destructorDrag;
    [SerializeField]float destructPartLifeTime;
    private bool isDestroyed=false;
	void Start () {
        destructorTransforms = new List<Transform>();
        m_patternEnemy = GetComponent<StatePatternEnemy>();
        m_patternEnemyRanged = GetComponent<StatePatternEnemyRanged>();
        m_animator = GetComponent<Animator>();
        m_navMesh = GetComponent<NavMeshAgent>();
        m_capsuleCollider = GetComponent<CapsuleCollider>();
        if (m_skinnedRenderer == null)
        {
            print("skinned renderer is null, assign in scene(AI) hpscript");
        }
        if (m_health > 0)
        {
            m_alive = true;
        }
        else
        {
            m_alive = false;
            Debug.Log("Spawned with 0 health, instadeath");
        }
	}
    void Update()
    {
        if (isDestroyed)
        {
            destructPartLifeTime -= Time.deltaTime;
            if (destructPartLifeTime <= 0)
            {
                
                foreach (Transform trans in destructorTransforms)
                {
                    if (trans != null)
                    {
                        Destroy(trans.gameObject);
                    }

                }
                Destroy(this.gameObject);
            }
        }
    }
    public int GetHealth()
    {
        return m_health;
    }

    public bool GetIsAlive()
    {
        return m_alive;
    }

    public void SetHealth(int p_healthAmountToSet)
    {
        m_health = p_healthAmountToSet;
        CheckIfAlive();
    }
 
    public void DealDamage(int p_damage)
    {
        m_health -= p_damage;
        CheckIfAlive();
    }

    void CheckIfAlive()
    {
        if (m_health <= 0)
        {
            m_alive = false;
            if (fragmentAtDeath)
            {
                DestructTransform();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
    void DestructTransform()
    {
        DisableAtDestruct();
  
         foreach (Transform trans in destructorTransforms)
         {
             if (trans != null)
             {
                 if (trans.parent != null)
                 {
                     trans.SetParent(null);
                 }
                 if (trans.GetComponent<Rigidbody>() == null)
                 {
                     Rigidbody p_rigid = trans.gameObject.AddComponent<Rigidbody>();
                     p_rigid.drag = destructorDrag;
                 }
                 if (trans.GetComponent<Collider>() == null)
                 {
                     SphereCollider p_coll= trans.gameObject.AddComponent<SphereCollider>();
                     p_coll.radius = 0.1f; // TEMP 
                 }
             }
         }
    }
    void DisableAtDestruct()
    {
        if (m_patternEnemy != null)
        {
            m_patternEnemy.enabled = false;
        }
        if (m_patternEnemyRanged != null)
        {
            m_patternEnemyRanged.enabled = false;
        }
        m_animator.Stop();
        m_animator.enabled = false;
        //m_navMesh.Stop();
        m_navMesh.enabled = false;
        m_capsuleCollider.enabled = false;
        WithForLoop(transform);
        m_skinnedRenderer.updateWhenOffscreen = true;
        isDestroyed = true;
    }
    void WithForeachLoop(Transform p_transf)
    {
        foreach (Transform child in transform){
            WithForLoop(child);
            WithForeachLoop(child);
            //print("Foreach loop: " + child);
        }
     
    }

    void WithForLoop(Transform p_transf)
    {
        int children = p_transf.childCount;
        for (int i = 0; i < children; ++i)
        {
            //print("For loop: " + p_transf.GetChild(i));
            WithForLoop(p_transf.GetChild(i));
            destructorTransforms.Add(ComparePartToString(p_transf.GetChild(i),destructorPartsNames));
  
        }
    }
    Transform ComparePartToString(Transform p_transform, string[] p_string)
    {
        foreach(string stringLine in p_string){
            if (stringLine == p_transform.name)
            {
                p_transform.gameObject.layer = LayerMask.NameToLayer("Default");
                return p_transform;
            }
        }
        return null;
    }
    // THINGS TO MOVE THIGHS, COLLARBONES, FOREARM, ELBOWSOCKET, TROOPERHEAD, UPPERARM, RIBCAGE
}
