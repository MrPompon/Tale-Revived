using UnityEngine;
using System.Collections;

public class AlertState : IEnemyState
{
  private readonly StatePatternEnemy enemy;
  private float searchTimer;
    public void Start()
   {

   }
    public AlertState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        BetterRay();
        Search();
    }
    public void OnTriggerEnter(Collider colli)
    {

    }
    public void OnTriggerStay(Collider colli)
    {

    }
    public void ToAlertState()
    {
        Debug.Log("Unable to go to same state(AI)");
    }
    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
        searchTimer = 0f;
    }
    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        searchTimer = 0f;
    }
    public void ToAttackState()
    {
        enemy.currentState = enemy.attackState;
    }
    public void ToRetreatState()
    {
        enemy.currentState = enemy.retreatState;
    }
    void BetterRay()
    {
        Vector3 direction = enemy.chaseTarget.transform.position - enemy.eyes.transform.position;
        float angle = Vector3.Angle(direction, enemy.eyes.transform.forward);

        // If the angle between forward and where the player is, is less than half the angle of view...
        if (angle < enemy.FOV_angle * 0.5f)
        {
            RaycastHit hit;

            // ... and if a raycast towards the player hits something...
            if (Physics.Raycast(enemy.eyes.transform.position + enemy.eyes.transform.up, direction.normalized, out hit, enemy.m_sphereCol.radius * 2))
            {
                // ... and if the raycast hits the player...
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    // ... the player is in sight.
                    enemy.chaseTarget = hit.transform;
                    ToChaseState();
                    // Set the last global sighting is the players current position.
                    //lastPlayerSighting.position = player.transform.position;
                }
            }
        }
    }
    private void Search()
    {
        enemy.meshRendererFlag.material.color = Color.yellow;
        enemy.navMeshAgent.Stop();
        enemy.transform.Rotate(0, enemy.searchTurnSpeed * Time.deltaTime,0); //rotates on this points and it stopped above.
        searchTimer += Time.deltaTime;
        if (searchTimer >= enemy.searchDuration)
        {
            ToPatrolState();
        }
    }
}
