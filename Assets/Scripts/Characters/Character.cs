using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Character type selection.
/// </summary>
[CreateAssetMenu(fileName = "NewCharacter", menuName = "ScriptableObjects/Character", order = 1)]
public class Character : ScriptableObject {
    public int    char_id;
    public string char_Name;
    public string userName;

    public Schools[]  specialties;      
    public GameObject charPrefab;
}
