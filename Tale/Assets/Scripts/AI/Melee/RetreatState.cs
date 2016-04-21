using UnityEngine;
using System.Collections;

public class RetreatState : IEnemyState
{
    private readonly StatePatternEnemy enemy;
    public void Start()
    {

    }
    public RetreatState(StatePatternEnemy statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }
    public void UpdateState()
    {

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
