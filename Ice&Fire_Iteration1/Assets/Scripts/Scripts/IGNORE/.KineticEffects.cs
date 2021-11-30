using System.Collections;
using UnityEngine;
using Mirror;




public class KineticEffect : NetworkBehaviour {
    public Ability       p_Ability;
    public KineticEffect p_Parent;


    public delegate void      LaunchAction();
    public event LaunchAction OnAction;
    public event LaunchAction OnWindUp;
    public event LaunchAction OnLaunch;
    public event LaunchAction OnFinish;

    public delegate void      HitAnyAction(GameObject other);
    public event HitAnyAction OnHitAny;

    public delegate void         HitHealthAction(Health target);
    public event HitHealthAction OnHitHealth;


    /// <summary>
    /// Called by the act that this effect is on when the act is instantiated,
    /// Registers callbacks for all act events.
    /// If this effect has a p_Parent effect, events are registered to the p_Parent instead.
    /// </summary>
    /// <param name="act"></param>
    public virtual void Register(Ability act) {
        p_Ability = act;
        OnAction += () => {};
        OnWindUp += () => {};
        OnLaunch += () => {};
        OnFinish += () => {};
        OnHitAny += (GameObject other) => {};
        OnHitHealth += (Health  t) => {};

        if (p_Parent) {
            Debug.Log("Registered OnAction to p_Parent effect");
            p_Parent.OnAction += RgstAct;
            p_Parent.OnWindUp += RgstWindUp;
            p_Parent.OnLaunch += RgstLaunch;
            p_Parent.OnHitAny += RgstHitAny;
            p_Parent.OnHitHealth += RgstHitHealth;
            p_Parent.OnFinish += RgstCleanUp;
        }
        else {
            act.OnAction += RgstAct;
            act.OnPostpone += RgstWindUp;
            act.OnRelease += RgstLaunch;
            act.OnAnyHitBy += RgstHitAny;
            act.OnHitHealth += RgstHitHealth;
            act.OnFinish += RgstCleanUp;
        }
    }

    public virtual void RgstAct() {
        OnAction?.Invoke();
    }
    public virtual void RgstWindUp() {
        OnWindUp?.Invoke();
    }
    public virtual void RgstLaunch() {
        OnLaunch?.Invoke();
    }
    public virtual void RgstHitAny(GameObject other) {
        OnHitAny?.Invoke(other);
    }
    public virtual void RgstHitHealth(Health target) {
        OnHitHealth?.Invoke(target);
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
