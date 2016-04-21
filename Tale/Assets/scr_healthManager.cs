using UnityEngine;
using System.Collections;

public class scr_healthManager : MonoBehaviour {

	// Use this for initialization
    [SerializeField]
    private int m_health;
    private bool m_alive;

	void Start () {
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
            Destroy(this.gameObject);
        }
    }
 
}
