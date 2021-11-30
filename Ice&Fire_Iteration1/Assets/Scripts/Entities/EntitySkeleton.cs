using UnityEngine;
using Mirror;

public abstract class EntitySkeleton : NetworkBehaviour {

    protected AnimatorOverrideController p_AnimatorOverrideController;

    public CharacterSkills _AttackSpeed;
    public CharacterSkills _MoveSpeed;
    public CharacterSkills _MaxArmor;
    public CharacterSkills _MaxHealth;

    public Transform _AimPoint;
    public Vector3   _LookPoint;
    public Animator  _Animator;
    public Health    _Health;
    public Alliance  _Alliance;

    // ---------------------------------------------------- Events Definitions ---------------------------------------------------- \\
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public abstract void GetFromPool(string name,   Vector3            position, Quaternion rotation);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <param name="amount"></param>
    public abstract void DealDamage(Health  other,  float              amount);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="health"></param>
    /// <param name="condition"></param>
    public abstract void AddDebuff(Health   health, ConditionInventory condition);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="health"></param>
    /// <param name="condition"></param>
    public abstract void AddBuff(Health     health, ConditionInventory condition);


    public virtual void Init() {
        p_AnimatorOverrideController = new AnimatorOverrideController(_Animator.runtimeAnimatorController);
        _Animator.runtimeAnimatorController = p_AnimatorOverrideController;
    }

    
    // TODO: 
    public virtual void AnimationOverride(string animationName, AnimationClip clip) {
        p_AnimatorOverrideController[animationName] = clip;
    }

    

}
