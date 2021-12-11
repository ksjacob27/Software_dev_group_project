using System;
using UnityEngine;
using UnityEngine.AI;



/* Tutorial: https://kybernetik.com.au/animancer/docs/introduction/features */
public abstract class CharacterSkeleton : MonoBehaviour {


    public Animator animator;
    // public    NavMeshAgent               agent;
    public    Alliance                   alliance;
    public    Health                     health;
    protected AnimatorOverrideController overrideController;
    public    float                      moveSpeed;
    public    CharacterTrait             maxArmor;
    public    CharacterTrait             maxHealth;


    /// <summary>
    /// Underlining skeleton animation controller
    /// </summary>
    public virtual void Initiate() {
        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
    }

    public abstract void ApplyBuff(Health     h, ConditionInventory con);
    public abstract void ApplyDebuff(Health   h, ConditionInventory con);
    public abstract void AfflictDamage(Health a, float              b);
    // public abstract void TakeDamage(float b);
    public abstract void GetFromPool(string a, Vector3 b, Quaternion c);
}
