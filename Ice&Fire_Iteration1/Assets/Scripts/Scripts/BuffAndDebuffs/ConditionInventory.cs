using System.Collections;
using UnityEngine;



public abstract class ConditionInventory : ScriptableObject {
    public abstract Condition GetCondition(Health tHealth, EntitySkeleton origin);
}


public class ConditionInventory<T, Typename> : ConditionInventory 
    where T : EffectTemplate 
    where Typename : Condition<T>, new() {
    public T _BuffData;
    public override Condition GetCondition(Health tHealth, EntitySkeleton origin) {
        return new Typename { _BuffData = this._BuffData, _OtherHealth = tHealth };
    }
}
