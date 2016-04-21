using UnityEngine;
using System.Collections;

public class AttackStateRanged : IEnemyState
{
    private readonly StatePatternEnemyRanged enemy;
    [HideInInspector]public IAttackStatesRanged currentAttackState;
    [HideInInspector]public AttackWindUpRanged windUpAttackState;
    [HideInInspector]public AttackOnGoingRanged ongoingAttackState;
    [HideInInspector]public AttackDownTimeRanged downtimeAttackState;
    [HideInInspector]public AttackAimTowardsRange aimState;

    public void Start()
    {
        windUpAttackState = new AttackWindUpRanged(enemy, this);
        ongoingAttackState = new AttackOnGoingRanged(enemy, this);
        downtimeAttackState = new AttackDownTimeRanged(enemy, this);
        aimState = new AttackAimTowardsRange(enemy, this);
        windUpAttackState.Start();
        ongoingAttackState.Start();
        downtimeAttackState.Start();
        currentAttackState = windUpAttackState;

    }
    public AttackStateRanged(StatePatternEnemyRanged statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
        currentAttackState.UpdateState();
    }
    public void OnTriggerEnter(Collider colli)
    {

    }
    public void OnTriggerStay(Collider colli)
    {
        currentAttackState.OnTriggerStay(colli);
    }
    public void ToAlertState()
    {

    }
    public void ToPatrolState()
    {

    }
    public void ToChaseState()
    {

    }
    public void ToAttackState()
    {

    }
    public void ToRetreatState()
    {

    }
}
