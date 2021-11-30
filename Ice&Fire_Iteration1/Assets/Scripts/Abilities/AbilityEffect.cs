using System.Collections;
using UnityEngine;
using Mirror;



public class AbilityEffect : NetworkBehaviour {
    public Ability       p_Ability;
    public AbilityEffect p_Parent;


    public delegate void    AbilityAction();
    public event AbilityAction OnAbility;
    public event AbilityAction OnHold;
    public event AbilityAction OnRelease;
    public event AbilityAction OnFinish;

    public delegate void    HitAnyAbility(GameObject other);
    public event HitAnyAbility OnHitAny;

    public delegate void       HitHealthAbility(Health target);
    public event HitHealthAbility OnHitHealth;

    
    /// <summary>
    /// Called by the ability that this effect is on when the ability is instantiated,
    /// Registers callbacks for all ability events.
    /// If this effect has a p_Parent effect, events are registered to the p_Parent instead.
    /// </summary>
    /// <param name="ability"></param>
    public virtual void Register(Ability ability) {
        this.p_Ability = ability;
        OnAbility += () => {};
        OnHold += () => {};
        OnRelease += () => {};
        OnFinish += () => {};
        OnHitAny += (GameObject other) => {};
        OnHitHealth += (Health  t) => {};

        if (p_Parent) {
            Debug.Log("Registered OnAbility to p_Parent effect");
            p_Parent.OnAbility += RgstAbility;
            p_Parent.OnHold += RgstPostpone;
            p_Parent.OnRelease += RgstReleased;
            p_Parent.OnHitAny += RgstHitAny;
            p_Parent.OnHitHealth += RgstHitHealth;
            p_Parent.OnFinish += RgstCleanUp;
        }
        else {
            ability.OnAction += RgstAbility;
            ability.OnPostpone += RgstPostpone;
            ability.OnRelease += RgstReleased;
            ability.OnAnyHitBy += RgstHitAny;
            ability.OnHitHealth += RgstHitHealth;
            ability.OnFinish += RgstCleanUp;
        }
    }

    public virtual void RgstAbility() {
        OnAbility();
    }
    
    public virtual void RgstPostpone() {
        OnHold();
    }
    
    public virtual void RgstReleased() {
        OnRelease();
    }
    
    public virtual void RgstHitAny(GameObject other) {
        OnHitAny(other);
    }
    
    public virtual void RgstHitHealth(Health target) {
        OnHitHealth(target);
    }

    public virtual void RgstCleanUp() {
        OnFinish();
    }

    public virtual bool RgstDone() {
        return true;
    }

    protected IEnumerator WaitThenDo(float duration, System.Action action) {
        yield return new WaitForSeconds(duration);
        action();
    }

}
