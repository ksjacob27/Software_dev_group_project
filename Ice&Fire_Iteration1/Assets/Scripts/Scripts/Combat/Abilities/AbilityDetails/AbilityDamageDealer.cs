using System;
using System.Collections.Generic;
using UnityEngine;


public class AbilityDamageDealer : AbilityEffect {
    public  float            _Damage;
    public  bool             _CleaverDamage = false;
    private List<GameObject> p_Impacted     = new List<GameObject>();



    public override void RgstHitHealth(Health impacted) {
        if (_CleaverDamage) {
            if (!p_Impacted.Contains(impacted.gameObject)) {
                p_Ability._Actor.DealDamage(impacted, _Damage);
            }
        }
        else {
            p_Ability._Actor.DealDamage(impacted, _Damage);
        }
        
        p_Impacted.Add(impacted.gameObject);
    }


}
