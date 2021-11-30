using System;
using System.Collections.Generic;
using UnityEngine;



public class SpellDamageDealer : SpellEffect {
    public  float        _Damage;
    public  bool         _BlastDamage = false;
    private List<GameObject> p_Impacted   = new List<GameObject>();



    public override void RgstHitHealth(Health impacted) {
        if (_BlastDamage) {
            if (!p_Impacted.Contains(impacted.gameObject)) {
                p_Spell._Caster.DealDamage(impacted, _Damage);
            }
        }
        else {
            p_Spell._Caster.DealDamage(impacted, _Damage);
        }
        
        p_Impacted.Add(impacted.gameObject);
    }


}

