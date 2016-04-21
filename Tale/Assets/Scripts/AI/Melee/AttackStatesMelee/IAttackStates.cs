using UnityEngine;
using System.Collections;

public interface IAttackStates{
    void Start();
    void UpdateState();
    void ToAttackWindUp();
    void ToAttackActive();
    void ToAttackDownTime();
}
