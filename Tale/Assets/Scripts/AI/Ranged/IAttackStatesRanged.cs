using UnityEngine;
using System.Collections;

public interface IAttackStatesRanged{
    void Start();
    void UpdateState();
    void ToAttackWindUp();
    void ToAttackActive();
    void ToAttackDownTime();
    void ToAimState();
    void OnTriggerStay(Collider colli);
}
