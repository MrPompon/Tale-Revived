using UnityEngine;
using System.Collections;

public class AttackDownTime : IAttackStates {

	// Use this for initialization
    private readonly StatePatternEnemy enemy;
    private readonly AttackState attackState;
    private float downTime;
    public void Start()
    {
        downTime = 0;
    }
    public AttackDownTime(StatePatternEnemy statePatternEnemy,AttackState p_attackState)
    {
        enemy = statePatternEnemy;
        attackState = p_attackState;
    }
    void ToChaseState()
    {
        attackState.currentAttackState = attackState.windUpAttackState;
        enemy.currentState = enemy.chaseState;
        downTime = 0;
    }
    public void UpdateState()
    {
        ChannelDownTime();
    }
    public void ToAttackWindUp()
    {

    }
    public void ToAttackActive()
    {

    }
    public void ToAttackDownTime()
    {

    }
    void ChannelDownTime()
    {
        Debug.Log("dammmn tired after that swing yo(AI)");
        downTime += Time.deltaTime;
        if (downTime > enemy.attackDownDuration)
        {
            ToChaseState();
        }
    }
}
