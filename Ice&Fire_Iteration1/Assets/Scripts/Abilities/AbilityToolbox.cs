using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Mirror;
using Spells;



public class AbilityToolbox : MonoBehaviour {
    private Entity        p_Actor;
    private Ability       p_ActiveAbility;
    private List<Ability> p_AbilityList = new List<Ability>();
    public  Transform     _AbilityPostRender;
    public  int           _Selector = -0;

    [Header("Energy")] public float         _MaxEnergy = 5f;
    [Header("Energy")] public float         _CurrentEnergy;
    [Header("Energy")] public float         _EnergyRegen = 0.5f;
    public                    Ability       _SelectedAbility;
    public                    List<Ability> _ActiveAbilities;

    public delegate void ToolEvent(Ability currentlyEquipped);
    public bool _IsAnchoring  { get; set; }
    public bool _IsDeflecting { get; set; }



    private void Start() {
        p_Actor = GetComponent<Entity>();
        
    }

    /// <summary>
    /// 
    /// </summary>
    public void Action() {
        if (p_ActiveAbility) { p_ActiveAbility.Action(); }
    }


    /// <summary>
    /// 
    /// </summary>
    public void PostActivity(Ability act) {
        act._Actor = p_Actor;
        act.Init();
        act.Action();

        this.p_ActiveAbility = act;
        // _IsAnchoring = act._IsAnchored;
        _IsDeflecting = act._Block;
    }


    /// <summary>
    /// Cancel the player ability cast post initiation.
    /// </summary>
    /// <summary>
    /// Cancel the player ability cast post initiation.
    /// </summary>
    public void Cancel() {
        if (p_ActiveAbility) {
            p_ActiveAbility = null;
            _Selector = 0;
        }
    }


    /// <summary>
    /// Casting check. Is the entity in the process of initiating an ability?
    /// </summary>
    /// <returns></returns>
    public bool IsActing() { return p_ActiveAbility != null; }


    public void Select(int abilityIndex) {
        if (p_AbilityList.Capacity <= abilityIndex) { return; }
        if (!p_AbilityList[abilityIndex]) { return; }
        p_ActiveAbility = p_AbilityList[abilityIndex];
    }


    public void Release() {
        if (p_ActiveAbility) { p_ActiveAbility.Release(); }
    }


    public void Hold() {
        if (p_ActiveAbility) { p_ActiveAbility.Postpone(); }
    }


    /// <summary>
    /// 
    /// </summary>
    public void PostEffect() {}
}
