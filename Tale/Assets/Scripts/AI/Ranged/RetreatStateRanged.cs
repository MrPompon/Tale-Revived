using UnityEngine;
using System.Collections;

public class RetreatStateRanged : IEnemyState
{
    private readonly StatePatternEnemyRanged enemy;
    public void Start()
    {

    }
    public RetreatStateRanged(StatePatternEnemyRanged statePatternEnemy)
    {
        enemy = statePatternEnemy;
    }
    public void UpdateState()
    {
       // Debug.Log("Damn you too close for me man");
        //DONT DO IN UPDATE CHECK ONCE THEN MOVE TO THAT POINT <<<<<note self optimize later perhaps 
        if (enemy.IsPlayerInsideComfortZone())//if player is NOT in i safe, and i aim again
        {
            RetreatOnNavmesh(GetPointToRetreatTo());
        }
        else
        {
            ToAttackState();
        }
    }
    void RetreatOnNavmesh(Vector3 toPos){
        enemy.navMeshAgent.SetDestination(toPos);
        enemy.navMeshAgent.Resume();
       // Debug.Log(toPos);
    }
    Vector3 GetPointToRetreatTo()
    {
        NavMeshHit hit;
        Vector3 pointAwayFromPlayer;
        if (NavMesh.SamplePosition(enemy.transform.position-enemy.GetDirectionTo(enemy.chaseTarget)*enemy.retreatLength, out hit, enemy.navMeshAgent.height*2, NavMesh.AllAreas))
        {
           return pointAwayFromPlayer = hit.position;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
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
        enemy.currentState = enemy.attackState;
    }
    public void ToRetreatState()
    {

    }
}
