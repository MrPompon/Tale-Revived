using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AttackOnGoingRanged : IAttackStatesRanged
{

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
 

    private readonly StatePatternEnemyRanged enemy;
    private readonly AttackStateRanged attackState;
    private List<Transform> hitObjects;

    private int cWP;
    public void Start()
    {
        cWP = 0;
        hitObjects = new List<Transform>();
    }
    public AttackOnGoingRanged(StatePatternEnemyRanged statePatternEnemy,AttackStateRanged p_attackState)
    {
        enemy = statePatternEnemy;
        attackState = p_attackState;
    }
    public void UpdateState()
    {
        Fire();
        
    }
    public void ToAttackWindUp()
    {
        attackState.currentAttackState = attackState.windUpAttackState;
    }
    public void ToAttackActive()
    {

    }
    void Fire()
    {
        if (!enemy.IsPlayerInsideComfortZone())
        {
            Vector3 directionToPlayerWithOffset=enemy.GetDirectionTo(enemy.chaseTarget);
            directionToPlayerWithOffset= new Vector3(directionToPlayerWithOffset.x,directionToPlayerWithOffset.y+0.10f,directionToPlayerWithOffset.z);
            SpawnProjectile(enemy.transform.position,directionToPlayerWithOffset,enemy.projectileSpeed);
            Debug.Log("lets say i jsut fired");
            enemy.isReloaded = false;
            ToAttackWindUp();
        }
        else
        {
            ToAttackWindUp();
            enemy.currentState = enemy.retreatState;
        }
    }
    public void ToAimState()
    {

    }
    public void OnTriggerStay(Collider colli)
    {

    }
    public void ToAttackDownTime()
    {
        attackState.currentAttackState = attackState.downtimeAttackState;

    }
    public void SpawnProjectile(Vector3 p_pos, Vector3 p_dir, float velocity)
    {
        GameObject newProj= enemy.SpawnProjectile();
        newProj.transform.position = p_pos;
        //newProj.transform.rotation =Quaternion.Euler( enemy.transform.rotation.x,enemy.transform.rotation.y,70);
        scr_AIProj scr_newProj = newProj.GetComponent<scr_AIProj>();
        scr_newProj.OnProjectileSpawn();
        scr_newProj.AddVelocity(p_dir, velocity);
        //newProj.transform.LookAt(newProj.transform.position + enemy.chaseTarget.position);

       
      

    }
    void ResetState()
    {
  
    }
}
