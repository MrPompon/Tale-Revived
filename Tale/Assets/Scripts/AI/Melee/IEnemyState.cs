using UnityEngine;
using System.Collections;

public interface IEnemyState {
    void Start();
    void UpdateState();
    void OnTriggerEnter(Collider colli);
    void OnTriggerStay(Collider colli);
    void ToAlertState();
    void ToPatrolState();
    void ToChaseState();
    void ToAttackState();
    void ToRetreatState();
}
