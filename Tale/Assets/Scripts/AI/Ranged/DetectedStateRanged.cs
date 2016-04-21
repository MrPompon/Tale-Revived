using UnityEngine;
using System.Collections;

public class DetectedStateRanged : IEnemyState
{
    private readonly StatePatternEnemyRanged enemy;

    public void Start()
    {

    }
    public DetectedStateRanged(StatePatternEnemyRanged statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        //Look();
        BetterRay();
        Chase();
    }
    public void OnTriggerEnter(Collider colli)
    {

    }
    public void OnTriggerStay(Collider other){

    } 
    //public void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("ASdasdas");
    //    // If the player has entered the trigger sphere...
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        // By default the player is not in sight.
    //        // Create a vector from the enemy to the player and store the angle between it and forward.
    //        Vector3 direction = other.transform.position - enemy.eyes.transform.position;
    //        float angle = Vector3.Angle(direction, enemy.eyes.transform.forward);

    //        // If the angle between forward and where the player is, is less than half the angle of view...
    //        if (angle < enemy.FOV_angle * 0.5f)
    //        {
    //            RaycastHit hit;

    //            // ... and if a raycast towards the player hits something...
    //            if (Physics.Raycast(enemy.eyes.transform.position + enemy.eyes.transform.up, direction.normalized, out hit, enemy.m_sphereCol.radius))
    //            {
    //                // ... and if the raycast hits the player...
    //                if (hit.collider.gameObject.CompareTag("Player"))
    //                {
    //                    // ... the player is in sight.
    //                   enemy.chaseTarget=hit.transform;
    //                    ToChaseState();
    //                    // Set the last global sighting is the players current position.
    //                    //lastPlayerSighting.position = player.transform.position;
    //                }
    //                else
    //                {
    //                    ToAlertState();
    //                }
    //            }
    //        }
    //    }
    //}
    void BetterRay()
    {
        Vector3 direction = enemy.chaseTarget.transform.position - enemy.eyes.transform.position;
        float angle = Vector3.Angle(direction, enemy.eyes.transform.forward);

        // If the angle between forward and where the player is, is less than half the angle of view...
        if (angle < enemy.FOV_angle * 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(enemy.eyes.transform.position + enemy.eyes.transform.up, direction.normalized, out hit, enemy.m_sphereCol.radius * 2))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    ToChaseState();
                }
            }
            else
            {
                ToAlertState();
            }
        }
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
        enemy.navMeshAgent.Stop();
    }
    public void ToRetreatState()
    {

    }
    private void Look()
    {
        RaycastHit hit;
        Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;
        if (Physics.Raycast(enemy.eyes.transform.position, enemyToTarget, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;

        }
        else
        {
            ToAlertState();
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
