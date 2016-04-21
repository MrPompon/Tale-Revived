using UnityEngine;
using System.Collections;

public class AttackState : IEnemyState
{
    private readonly StatePatternEnemy enemy;
    [HideInInspector]public IAttackStates currentAttackState;
    [HideInInspector]public AttackWindUp windUpAttackState;
    [HideInInspector]public AttackOnGoing ongoingAttackState;
    [HideInInspector]public AttackDownTime downtimeAttackState;

    public void Start()
    {
        Debug.Log("asfas");
        windUpAttackState = new AttackWindUp(enemy, this);
        ongoingAttackState = new AttackOnGoing(enemy, this);
        downtimeAttackState = new AttackDownTime(enemy, this);
        windUpAttackState.Start();
        ongoingAttackState.Start();
        downtimeAttackState.Start();
        currentAttackState = windUpAttackState;

    }
    public AttackState(StatePatternEnemy statePatternEnemy)
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
