using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



/// <summary>
/// Class to template the data retrieved from a database or file.
/// </summary>
[Serializable] public class BuildTable {
    public string _Name;
    public int    _CharacterId;
    public int[]  _Index;



    public BuildTable(string name, int characterID, IEnumerable<SpellScript> spells) {
        _Name = name;
        _CharacterId = characterID;
        _Index = spells.Select((spell) => spell._SpellID).ToArray();
    }


    public Character CharacterDataAggregate() {
        Character[] characters = Resources.LoadAll<Character>("Characters");
        return Array.Find(characters, (cha) => cha.char_id == _CharacterId);
    }


    public List<SpellScript> SpellToolDataAggregate() {
        SpellScript[]     spells = Resources.LoadAll<SpellScript>("SpellScript");
        List<SpellScript> box    = spells.Select((t, i) => Array.Find(spells, (spell) => spell._SpellID == _Index[i])).Where(script => script).ToList();
        return box;
    }
}
