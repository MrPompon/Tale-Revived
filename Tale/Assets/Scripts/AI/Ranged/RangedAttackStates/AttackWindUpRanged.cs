using UnityEngine;
using System.Collections;

public class AttackWindUpRanged : IAttackStatesRanged
{
    private readonly StatePatternEnemyRanged enemy;
    private readonly AttackStateRanged attackState;

    private float attackWindUpDuration;
    public void Start()
    {

    }
     public AttackWindUpRanged(StatePatternEnemyRanged statePatternEnemy,AttackStateRanged p_attackState)
    {
        enemy = statePatternEnemy;
        attackState = p_attackState;
    }
    public void UpdateState()
    {
        // Debug.Log("YO IM WINDING UP MY ATTACK(AI)");
        WindUpAttack();
    }
    public void ToAttackWindUp()
    {
        Debug.Log("Already winding up(AI)");
    }
    public void ToAttackActive()
    {
        attackState.currentAttackState = attackState.ongoingAttackState;
        attackWindUpDuration = 0;
    }
    public void ToAttackDownTime()
    {
        attackState.currentAttackState = attackState.downtimeAttackState;
        Debug.Log("Time for me to reload(RangedAI)?");
    }
    private void WindUpAttack()
    {
        if (enemy.isReloaded)
        {
            attackWindUpDuration += Time.deltaTime;
            if (attackWindUpDuration >= enemy.attackWindUpDuration)
            {
                ToAimState();
            }
        }
        else
        {
            ToAttackDownTime();
        }
    }
    public void ToAimState()
    {
        //if enemy is to close run away, (and then aim again)not done here 
        if (enemy.IsPlayerInsideComfortZone())
        {
            enemy.currentState = enemy.retreatState;
        }
        else
        {
            attackState.currentAttackState = attackState.aimState;
        }
    
    }
    public void OnTriggerStay(Collider colli)
    {

    }
}
