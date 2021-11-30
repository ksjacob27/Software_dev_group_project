using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Mirror;
using Spells;

public abstract class SpellToolbox : MonoBehaviour {
    private Entity      _Actor;
    public  Transform   _AbilityPostRender;
    public  Spell       _ActiveSpell;
    public  int         _Selector;
    public  SpellScript _Selected;

    [Header("Mana")] public float _MaxMana = 5f;
    [Header("Mana")] public float _CurrentMana;
    [Header("Mana")] public float _ManaRegen = 0.5f;
    public                  float _SDelay;

    public SpellScript       _SelectedSpell;
    public List<SpellScript> _SpellList;

    public delegate void ToolEvent(Ability currentlyEquipped);
    public          bool _IsAnchoring  { get; set; }
    public          bool _IsDeflecting { get; set; }



    private void Start() {
        _Actor = GetComponent<Entity>();
    }

    private void Update() {
        if (_ActiveSpell && _ActiveSpell.Done()) {
            _ActiveSpell.Despawn();
            _IsAnchoring = false;
            _IsDeflecting = false;
            _ActiveSpell = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void PostEffect() {}


    public void PostCast(Spell act) {
        act.Init();
        act.Cast();

        act._Caster = _Actor;
        _ActiveSpell = act;
        _IsAnchoring = act._Anchored;
        _IsDeflecting = act._DeflectDuration;
    }





    public void Cast() {
        if (_ActiveSpell) { _ActiveSpell.Cast(); }
    }

    /// <summary>
    /// Casting check. Is the entity in the process of initiating an ability?
    /// </summary>
    /// <returns></returns>
    public bool IsCasting() { return _SpellList != null; }


    /// <summary>
    /// Cancel the player ability cast post initiation.
    /// </summary>
    public void Cancel() {
        if (_ActiveSpell) {
            _ActiveSpell = null;
            _SelectedSpell = null;
        }
    }


    public void Select(int spellIndex) {
        if (_SpellList.Capacity <= spellIndex) { return; }
        if (!_SpellList[spellIndex]) { return; }
        _Selected = _SpellList[spellIndex];
    }


    public void Release() {
        if (_ActiveSpell) { _ActiveSpell.Release(); }
    }


    public void Postpone() {
        if (_ActiveSpell) { _ActiveSpell.Postpone(); }
    }
}
