using UnityEngine;
using System.Collections;

public class AttackAimTowardsRange : IAttackStatesRanged{

    // Use this for initialization
    private readonly StatePatternEnemyRanged enemy;
    private readonly AttackStateRanged attackState;
    private float downTime;
    public void Start()
    {
        downTime = 0;
    }
    public AttackAimTowardsRange(StatePatternEnemyRanged statePatternEnemy, AttackStateRanged p_attackState)
    {
        enemy = statePatternEnemy;
        attackState = p_attackState;
    }
    void ToChaseState()
    {
        attackState.currentAttackState = attackState.windUpAttackState;
        enemy.currentState = enemy.detectedState;
        downTime = 0;
    }
    public void UpdateState()
    {
       // Look();
        BetterRay();
        AimTowards(enemy.chaseTarget.transform.position);
    }
    public void ToAttackWindUp()
    {
        attackState.currentAttackState = attackState.windUpAttackState;
        downTime = 0;
        enemy.isReloaded = true;
    }
    public void ToAttackActive()
    {
        attackState.currentAttackState = attackState.ongoingAttackState;
    }
    public void ToAttackDownTime()
    {

    }
    public void ToAimState()
    {

    }

    void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
        attackState.currentAttackState = attackState.windUpAttackState;
    }
    private void Look()
    {
        RaycastHit hit;
        Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;
        if (Physics.Raycast(enemy.eyes.transform.position, enemyToTarget, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
            ToAttackActive();
        }
        else
        {
            ToAlertState();
            return;
        }

    }

    public void OnTriggerStay(Collider other)
    {

    }
    //public void OnTriggerStay(Collider other)
    //{
    //    // If the player has entered the trigger sphere...
        
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("asdsadas");
    //        // By default the player is not in sight.
    //        // Create a vector from the enemy to the player and store the angle between it and forward.
    //        Vector3 direction = other.transform.position - enemy.eyes.transform.position;
    //        float angle = Vector3.Angle(direction, enemy.eyes.transform.forward);

    //        // If the angle between forward and where the player is, is less than half the angle of view...
    //        if (angle < enemy.FOV_angle * 0.5f)
    //        {
    //            RaycastHit hit;

    //            // ... and if a raycast towards the player hits something...
    //            if (Physics.Raycast(enemy.eyes.transform.position + enemy.eyes.transform.up, direction.normalized, out hit, enemy.m_sphereCol.radius*2))
    //            {
    //                // ... and if the raycast hits the player...
    //                if (hit.collider.gameObject.CompareTag("Player"))
    //                {
    //                    // ... the player is in sight.
    //                    ToAttackActive();

    //                    // Set the last global sighting is the players current position.
    //                    //lastPlayerSighting.position = player.transform.position;
    //                }
    //            }
    //            else
    //            {
    //                ToAlertState();
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

                // ... and if a raycast towards the player hits something...
                if (Physics.Raycast(enemy.eyes.transform.position + enemy.eyes.transform.up, direction.normalized, out hit, enemy.m_sphereCol.radius * 2))
                {
                    // ... and if the raycast hits the player...
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        // ... the player is in sight.
                        ToAttackActive();

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
    void AimTowards(Vector3 p_pos)
    {
     //if aiming on player//should miss sometimes tho fire
        //Debug.Log("Aiming at pos"+ p_pos);
        //Debug.Log(enemy.GetDirectionTo(enemy.chaseTarget));
        enemy.RotateToward();
        //ToAttackActive();
    }

}
