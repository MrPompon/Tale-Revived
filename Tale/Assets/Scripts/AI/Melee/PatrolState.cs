using UnityEngine;
using System.Collections;

public class PatrolState : IEnemyState{
    private readonly StatePatternEnemy enemy;
    private int nextWayPoint;
    public void Start()
    {

    }
    public PatrolState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        BetterRay();
        Patrol();
    }
    public void OnTriggerEnter(Collider colli)
    {
        if (colli.gameObject.CompareTag("Player"))
        {
            ToAlertState();
        }
    }
    public void OnTriggerStay(Collider colli)
    {

    }
    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
    }
    public void ToPatrolState()
    {
        Debug.Log("Unable to go to same state(AI)");
    }
    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
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
            if (Physics.Raycast(enemy.eyes.transform.position + enemy.eyes.transform.up, direction.normalized, out hit, enemy.m_sphereCol.radius))
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
    private void Patrol()
    {
        enemy.meshRendererFlag.material.color = Color.green;
        enemy.navMeshAgent.destination = enemy.waypoints[nextWayPoint].position;
        enemy.navMeshAgent.Resume();

        if (enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending)
        {
            nextWayPoint = (nextWayPoint + 1) % enemy.waypoints.Length; //goes back to zero when gone to last
        }
    }
}
