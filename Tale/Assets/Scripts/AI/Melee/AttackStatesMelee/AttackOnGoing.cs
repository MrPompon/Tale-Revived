using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AttackOnGoing : IAttackStates {

	// Use this for initialization
    //I have been making a multiplayer brawler, and I go for a variant of solution 2. 
    //What I do is for every weapon, have a set of empty transforms which define a simplified 'edge' for 
    //the weapon, and then while it is being swung, I remember the difference between the positions of each empty and their
    //positions last frame, and use Physics.Linecast to find out if anything got hit in between. This provides a very accurate 
    //measure of where the weapon is going, because it doesn't rely on an object not 'phasing' through because of the animation. The only thing to check here, is to make sure that the weapon
    //doesn't hit something twice in the same swing- maybe when it hits an enemy, make the enemy stop receiving damage for half a second or so.

    //For an even easier implementation, cast a line that covers how far the bullet just traveled since the previous 
    //    fixed update. Record the bullet's position in Start(), for example in a variable named previousPosition. In
    //each FixedUpdate(), run a linecast from previousPosition to the current position. Then store the current position 
    //    in previousPosition. You don't even need to set it to continuous mode for this.
 

    private readonly StatePatternEnemy enemy;
    private readonly AttackState attackState;
    private List<Transform> hitObjects;

    private int cWP;
    public void Start()
    {
        cWP = 0;
        hitObjects = new List<Transform>();
    }
    public AttackOnGoing(StatePatternEnemy statePatternEnemy,AttackState p_attackState)
    {
        enemy = statePatternEnemy;
        attackState = p_attackState;
    }
    public void UpdateState()
    {
        Debug.Log("ATTACKING");
        SimulateAttackRays();
    }
    public void ToAttackWindUp()
    {

    }
    public void ToAttackActive()
    {

    }
    public void ToAttackDownTime()
    {
        attackState.currentAttackState = attackState.downtimeAttackState;

    }
    void SimulateAttackRays()
    {
        if (cWP < enemy.weaponWayPoints.Length-1)
        {

            float distanceBetween = Vector3.Distance(enemy.weaponWayPoints[cWP].position, enemy.weaponWayPoints[cWP + 1].position);
            Vector3 direction = enemy.weaponWayPoints[cWP + 1].position - enemy.weaponWayPoints[cWP].position;
            RaycastHit[] hitTargets = Physics.RaycastAll(enemy.weaponWayPoints[cWP].position, direction, distanceBetween,enemy.playerLayer);
            Debug.DrawLine(enemy.weaponWayPoints[cWP].position, enemy.weaponWayPoints[cWP + 1].position, Color.magenta);

            AddRayHitToList(hitTargets);
            cWP += 1; 
        }
        else
        {
            DealDamageToHitTargets();
        }
    }
    void AddRayHitToList(RaycastHit[] p_hitTargets)
    {
        for (int i = 0; i < p_hitTargets.Length; i++)
        {
            if(!hitObjects.Contains(p_hitTargets[i].transform)){
                hitObjects.Add(p_hitTargets[i].transform);
            }
        }
    }
    void DealDamageToHitTargets()
    {
        
        if (hitObjects.Count > 0)
        {
            foreach (Transform transform in hitObjects)
            {
                Debug.Log(transform.name); //add damage calulation here later<-------------------------
            }
        }
        Debug.Log("I damn missed(AI)");
        ToAttackDownTime();
        ResetState();
    }
    void SimulateAttackRaysWithSwingSpeed()
    {
        
    }
    void ResetState()
    {
        hitObjects.Clear();//do at end
        cWP = 0;
    }
}
