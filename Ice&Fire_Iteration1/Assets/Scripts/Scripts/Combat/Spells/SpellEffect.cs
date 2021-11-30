using System.Collections;
using UnityEngine;
using Mirror;



public class SpellEffect : NetworkBehaviour {
    public Spell       p_Spell;
    public SpellEffect p_Parent;


    public delegate void    CastAction();
    public event CastAction OnCast;
    public event CastAction OnHold;
    public event CastAction OnRelease;
    public event CastAction OnFinish;

    public delegate void    HitAnyCast(GameObject other);
    public event HitAnyCast OnHitAny;

    public delegate void       HitHealthCast(Health target);
    public event HitHealthCast OnHitHealth;

    
    /// <summary>
    /// Called by the spell that this effect is on when the spell is instantiated,
    /// Registers callbacks for all spell events.
    /// If this effect has a p_Parent effect, events are registered to the p_Parent instead.
    /// </summary>
    /// <param name="spell"></param>
    public virtual void Register(Spell spell) {
        this.p_Spell = spell;
        OnCast += () => {};
        OnHold += () => {};
        OnRelease += () => {};
        OnFinish += () => {};
        OnHitAny += (GameObject other) => {};
        OnHitHealth += (Health  t) => {};

        if (p_Parent) {
            Debug.Log("Registered OnCast to p_Parent effect");
            p_Parent.OnCast += RgstCast;
            p_Parent.OnHold += RgstPostpone;
            p_Parent.OnRelease += RgstReleased;
            p_Parent.OnHitAny += RgstHitAny;
            p_Parent.OnHitHealth += RgstHitHealth;
            p_Parent.OnFinish += RgstCleanUp;
        }
        else {
            spell.OnCast += RgstCast;
            spell.OnPostpone += RgstPostpone;
            spell.OnRelease += RgstReleased;
            spell.OnAnyHitBy += RgstHitAny;
            spell.OnHitHealth += RgstHitHealth;
            spell.OnFinish += RgstCleanUp;
        }
    }

    public virtual void RgstCast() {
        OnCast();
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
