using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewCharacter", menuName = "ScriptableObjects/Character", order = 1)]
public class Character : ScriptableObject {
    public int    id;
    public string _ActiveName;
    public string _ActiveDescription;

    public string    _PassiveName;
    public string    _PassiveDescription;
    public Schools[] _Types;

    public GameObject EntityPrefab;
}
