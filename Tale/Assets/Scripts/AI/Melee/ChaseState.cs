using UnityEngine;
using System.Collections;

public class ChaseState : IEnemyState
{
    private readonly StatePatternEnemy enemy;

    public void Start()
    {

    }
    public ChaseState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        BetterRay();
        Chase();
    }
    public void OnTriggerEnter(Collider colli)
    {

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

    }
    public void ToChaseState()
    {

    }
    public void ToAttackState()
    {
        enemy.currentState = enemy.attackState;
    }
    public void ToRetreatState()
    {

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

                    // Set the last global sighting is the players current position.
                    //lastPlayerSighting.position = player.transform.position;
                }
            }
            else
            {
                ToAlertState();
            }
        }
    }
    private void Chase()
    {
        enemy.meshRendererFlag.material.color = Color.red;
        enemy.navMeshAgent.destination = enemy.chaseTarget.position;
        enemy.navMeshAgent.Resume();
        if (Vector3.Distance(enemy.transform.position, enemy.chaseTarget.position) < enemy.attackRange)
        {
            ToAttackState();
        }
    }
}
