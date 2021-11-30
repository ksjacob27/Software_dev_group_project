using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;



[CanBeNull]
public abstract class Commander<T, V, E> {
    /// <summary> </summary>
    /// <params name="..."></params>
    /// <returns></returns>
    public abstract void Execute(T tAction, V vAction, E eAction);


    public virtual void Move(Transform entityTrans) { }
}



/// <summary> </summary>
public class FirePrimary : Commander<Ray, RaycastHit, Player> {
    public override void Execute(Ray entityRay, RaycastHit raycast, Player player) { FirePrimaryWeapon(entityRay, raycast, player); }


    private void FirePrimaryWeapon(Ray ray, RaycastHit raycast, Player player) { Debug.Log("Fire Primary recorded from: " + player.tag); }
}



/// <summary> </summary>
public class FireSecondary : Commander<Ray, RaycastHit, Player> {
    public override void Execute(Ray entityRay, RaycastHit raycast, Player player) { FireSecondaryWeapon(entityRay, raycast, player); }


    private void FireSecondaryWeapon(Ray ray, RaycastHit raycastHit, Player player) { Debug.Log("Fire Secondary recorded from: " + player.tag); }
}



/// <summary> </summary>
public class UseUtilityPrimary : Commander<Ray, RaycastHit, Player> {
    public override void Execute(Ray entityRay, RaycastHit raycast, Player player) { UsePrimaryUtility(entityRay, raycast, player); }


    private void UsePrimaryUtility(Ray ray, RaycastHit raycastHit, Player player) { Debug.Log("Primary Utility use recorded from: " + player.tag); }
}



/// <summary> </summary>
public class UseUtilitySecondary : Commander<Ray, RaycastHit, Player> {
    public override void Execute(Ray entityRay, RaycastHit raycast, Player player) { UseSecondaryUtility(entityRay, raycast, player); }


    private void UseSecondaryUtility(Ray ray, RaycastHit raycastHit, Player player) { Debug.Log("Secondary Utility use recorded from: " + player.tag); }
}



/// <summary>Abilities</summary>
///
/// <summary>Entity's Ultimate Ability</summary>
public class UseAbilityUltimate : Commander<Ray, RaycastHit, Player> {
    public override void Execute(Ray entityRay, RaycastHit raycast, Player player) { UseUltimateUtility(entityRay, raycast, player); }


    private void UseUltimateUtility(Ray ray, RaycastHit raycastHit, Player player) { Debug.Log("Ultimate Utility use recorded from: " + player.tag); }
}


/// <summary>Entity's Second Slot Ability</summary>
public class UseAbilityTwo : Commander<Ray, RaycastHit, Player> {
    public override void Execute(Ray entityRay, RaycastHit raycast, Player player) { }
}


/// <summary>Entity's Third Slot Ability</summary>
public class UseAbilityThree : Commander<Ray, RaycastHit, Player> {
    public override void Execute(Ray entityRay, RaycastHit raycast, Player player) { }
}



/// <summary>Utility Items</summary>
///
/// <summary>Entity's First Slot Utility</summary>
public class UseUtilitySlotOne : Commander<Ray, RaycastHit, Player> {
    public override void Execute(Ray entityRay, RaycastHit raycast, Player player) { }
}


/// <summary>Entity's Second Slot Utility</summary>
public class UseUtilitySlotTwo : Commander<Ray, RaycastHit, Player> {
    public override void Execute(Ray entityRay, RaycastHit raycast, Player player) { }
}


/// <summary>Entity's Third Slot Utility</summary>
public class UseUtilitySlotThree : Commander<Ray, RaycastHit, Player> {
    public override void Execute(Ray entityRay, RaycastHit raycast, Player player) { }
}



/// <summary>Entity's Movement</summary>
public class MoveEntity : Commander<Transform, Vector3, Vector3[]> {
    public override void Execute(Transform T, Vector3 V, Vector3[] E) { MoveActor(T, V, E); }


    private void MoveActor(Transform tAction, Vector3 heading, Vector3[] directionMovement) {
        Vector3 direction = new Vector3(Input.GetAxis("HorizontalKey"), 0, Input.GetAxis("VerticalKey"));

        tAction.forward = heading;
        tAction.position += directionMovement[0];
        tAction.position += directionMovement[1];

        // p_Agent.SetDestination(hit.point);
    }
}



public struct NothingStuct {
}


/// <summary> </summary>
public class DoNothing : Commander<NothingStuct, NothingStuct, Player> {
    public override void Execute(NothingStuct null1, NothingStuct null2, Player null3) { return; }
}




public class PlayerRangeAbility : MonoBehaviour {
    private int _damageModifier = 10;
    private float _fireInterval = 0.10f;
    private float _range = 100f;
    private int _MaxDamage;
    private int _MinDamage;

    private Ray _lineOfFire = new Ray();
    private RaycastHit _hitRegister;
    private LineRenderer _dischargeTrail;
    private ParticleSystem _abilityEffects;
    private Light _dischargeLight;
    private AudioSource _dischargeReport;

    public float _dischargeDuration = 0.5f;
    public float timer;


    void Awake() {
        _dischargeLight = GetComponent<Light>();
        _dischargeReport = GetComponent<AudioSource>();
        _dischargeTrail = GetComponent<LineRenderer>();
        _abilityEffects = GetComponent<ParticleSystem>();
    }



    private void Update() {
        timer += Time.deltaTime;
        if (Input.GetButton("PrimaryFire")) {
            if (timer >= _fireInterval && Time.timeScale != 0) {

            }
        }
    }
}
