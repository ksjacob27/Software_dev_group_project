using System.Collections;
using UnityEngine;



public abstract class Condition {
    public abstract EffectTemplate GetCondition();

    public delegate void            DropConditionEvent();
    public event DropConditionEvent OnDropBuffCondition;
    public event DropConditionEvent OnDropDebuffCondition;

    public delegate void           AddConditionEvent();
    public event AddConditionEvent OnAddBuffCondition;
    public event AddConditionEvent OnAddDebuffCondition;


    public virtual void AddBuffCondition()   { OnAddBuffCondition += () => {}; }
    public virtual void AddDebuffCondition() { OnAddDebuffCondition += () => {}; }

    public virtual void DropBuffCondition()   { OnDropBuffCondition?.Invoke(); }
    public virtual void DropDebuffCondition() { OnDropDebuffCondition?.Invoke(); }

    public abstract void DoT();



    public IEnumerator DebuffEffectOvertime(float duration) {
        float dotTick = duration / 32f; // 0.32 seconds World of Warcraft design (adding diminishing)     
        float now     = Time.fixedTime;
        float end     = now + duration;

        float last = 0f;
        while (now < end) {
            now = Time.fixedTime;
            if (now > last + dotTick) {

                DoT();

                last = now;
            }
            yield return null;
        }

        DropDebuffCondition();
    }
}


public abstract class Condition<T> : Condition where T : EffectTemplate {
    public T      _BuffData;
    public Health _OtherHealth;
    public Entity _OriginalEntity;

    public override EffectTemplate GetCondition() {
        return _BuffData;
    }
}
