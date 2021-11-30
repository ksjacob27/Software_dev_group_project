using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



[Serializable] public class BuildTable {
    public string              _Name;
    public int                 _CharacterId;
    public Tuple<int[], int[]> _Index;



    public BuildTable(string name, int characterID, List<Ability> abilities, List<SpellScript> spells) {
        _Name = name;
        _CharacterId = characterID;
        _Index = new Tuple<int[], int[]>(abilities.Select((able) => able._AbilityID).ToArray(), spells.Select((spell) => spell._SpellID).ToArray());
    }

    
    public Character CharacterDataAggregate() {
        Character[] characters = Resources.LoadAll<Character>("Characters");
        return Array.Find(characters, (cha) => cha.id == _CharacterId);
    }


    public List<Ability> AbilityToolDataAggregate() {
        Ability[] abilities = Resources.LoadAll<Ability>("Ability");

        return _Index.Item1.Select(t => Array.Find(abilities, (ab) => ab._AbilityID == t)).Where(able => able).ToList();
    }


    public List<SpellScript> SpellToolDataAggregate() {
        SpellScript[]     spells = Resources.LoadAll<SpellScript>("SpellScript");
        List<SpellScript> box    = spells.Select((t, i) => Array.Find(spells, (spell) => spell._SpellID == _Index.Item2[i])).Where(script => script).ToList();
        return box;
    }
}
