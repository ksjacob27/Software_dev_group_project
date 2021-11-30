using System;
using UnityEngine;


public class SpellTargeting : SpellEffect {
    public float     _Range = 150f;
    public LayerMask _Mask;

    public override void RgstCast() {
        Transform heading = p_Spell._Caster._AimPoint;

        if (Physics.Raycast(heading.position, heading.forward, out RaycastHit hit, _Range, _Mask)) {
            this.transform.position = hit.point;
            this.transform.rotation = Quaternion.identity;
            base.RgstCast();
        }
    }
    
}



public class SpellTargetPosition : SpellEffect {
    public override void RgstHitHealth(Health other) {
        other.transform.position = transform.position;
    }
}

