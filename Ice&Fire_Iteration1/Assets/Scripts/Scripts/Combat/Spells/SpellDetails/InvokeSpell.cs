using UnityEngine;

public class InvokeSpell : SpellEffect {
    public GameObject prefab;
    public int        maxActiveSummons = 1;

    public override void Register(Spell aSpell) {
        base.Register(aSpell);
        ObjectPool.RegisterPrefab(prefab.name, maxActiveSummons);
    }

    public override void RgstCast() {
        p_Spell._Caster.GetFromPool(prefab.name, transform.position, transform.rotation);
    }
}
