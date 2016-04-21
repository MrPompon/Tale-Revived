using UnityEngine;
using System.Collections;

public class AttackWindUp : IAttackStates {
    private readonly StatePatternEnemy enemy;
    private readonly AttackState attackState;

    private float attackWindUpDuration;
    public void Start()
    {

    }
     public AttackWindUp(StatePatternEnemy statePatternEnemy,AttackState p_attackState)
    {
        enemy = statePatternEnemy;
        attackState = p_attackState;
    }
    public void UpdateState()
    {
        Debug.Log("YO IM WINDING UP MY ATTACK(AI)");
        WindUpAttack();
    }
    public void ToAttackWindUp()
    {
        Debug.Log("Already winding up(AI)");
    }
    public void ToAttackActive()
    {
        attackState.currentAttackState = attackState.ongoingAttackState;
        Debug.Log("asdasdasdasdsad");
        attackWindUpDuration = 0;
    }
    public void ToAttackDownTime()
    {
        Debug.Log("Got stunned during windup(AI)?");
    }
    private void WindUpAttack()
    {
        attackWindUpDuration += Time.deltaTime;
        if (attackWindUpDuration >= enemy.attackWindUpDuration)
        {
            ToAttackActive(); 
        }
    }
}
