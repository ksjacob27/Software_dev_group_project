using System;
using UnityEngine;
using UnityEngine.UIElements;



// [CreateAssetMenu(fileName = "NewAbility", menuName = "ScriptableObjects/Abilities", order = 0)]
[CreateAssetMenu(fileName = "NewCast", menuName = "ScriptableObjects/ActionAbility", order = 0)]
public class AbilityScript : ScriptableObject {

    public int        _AbilityID;
    public GameObject _AbilityPrefab;
    public Sprite     _AbilitySprite;
    public Schools    _Type;
    public Alliance[] _NotHarmWhom;
    
    public float _Damage;
    public float _Healing;
    public float _ManaCost;
    public float _EnergyCost;

    public string _Title;
    public string _detail;

}
