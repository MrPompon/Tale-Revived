using UnityEngine;
using System.Collections;

public class AttackDownTimeRanged : IAttackStatesRanged
{

	// Use this for initialization
    private readonly StatePatternEnemyRanged enemy;
    private readonly AttackStateRanged attackState;
    private float downTime;
    public void Start()
    {
        downTime = 0;
    }
    public AttackDownTimeRanged(StatePatternEnemyRanged statePatternEnemy,AttackStateRanged p_attackState)
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
        ChannelDownTime();
    }
    public void ToAttackWindUp()
    {
        attackState.currentAttackState = attackState.windUpAttackState;
        downTime = 0;
        enemy.isReloaded = true;
    }
    public void ToAttackActive()
    {

    }
    public void ToAimState()
    {

    }
    public void OnTriggerStay(Collider colli)
    {

    }
    public void ToAttackDownTime()
    {

    }
    void ChannelDownTime()
    {
        //Debug.Log("Im reloading(ranged AI)");
        downTime += Time.deltaTime;
        if (downTime > enemy.attackDownDuration)
        {
            ToAttackWindUp();
        }
    }
}
