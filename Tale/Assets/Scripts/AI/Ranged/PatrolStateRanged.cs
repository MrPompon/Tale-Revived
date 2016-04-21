using UnityEngine;
using System.Collections;

public class PatrolStateRanged : IEnemyState
{
    private readonly StatePatternEnemyRanged enemy;
    private int nextWayPoint;
    public void Start()
    {

    }
    public PatrolStateRanged(StatePatternEnemyRanged statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }

    public void UpdateState()
    {
        //Look();
        Patrol();
    }
    public void OnTriggerEnter(Collider colli)
    {
        if (colli.gameObject.CompareTag("Player"))
        {
            ToAlertState();
        }
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
        enemy.currentState = enemy.detectedState;
    }
    public void ToAttackState()
    {
        enemy.currentState = enemy.attackState;
    }
    public void ToRetreatState()
    {
        enemy.currentState = enemy.retreatState;
    }
    private void Look()
    {
        RaycastHit hit;
        if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.transform.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
            ToChaseState();
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

    public void OnTriggerStay(Collider other)
    {
        // If the player has entered the trigger sphere...
        if (other.gameObject.CompareTag("Player"))
        {
            // By default the player is not in sight.
            // Create a vector from the enemy to the player and store the angle between it and forward.
            Vector3 direction = other.transform.position - enemy.eyes.transform.position;
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
                        enemy.chaseTarget=hit.transform;
                        ToAlertState();

                        // Set the last global sighting is the players current position.
                        //lastPlayerSighting.position = player.transform.position;
                    }
                }
            }
        }
    }
}
