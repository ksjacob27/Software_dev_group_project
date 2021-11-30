using System.Collections.Generic;
using UnityEngine;



public class Builder {
    protected               string            p_BuildFile;
    public                  string            _BuildTitle;
    private static readonly string            p_Ability_Data_Dir_ = $"{Application.persistentDataPath}/AbilityToolBoxes";
    private static readonly string            p_SPELL_DATA_DIR_   = $"{Application.persistentDataPath}/SpellToolBoxes";
    private static readonly string            p_BUILD_DIR_        = $"{Application.persistentDataPath}/Builds/";
    private                 List<Ability>     _AbilitiesBuilder;
    private                 List<SpellScript> _SpellsBuilder;
    private                 Character         _Character;

    public Builder() {}
    public Builder(string fileName) {
        p_BuildFile = fileName;
    }

    public static Builder LoadDataFromString(string toolBoxJson) {
        BuildTable build   = JsonUtility.FromJson<BuildTable>(toolBoxJson);
        Builder    builder = new Builder {
            _BuildTitle = build._Name,
            _AbilitiesBuilder = build.AbilityToolDataAggregate(),
            _SpellsBuilder = build.SpellToolDataAggregate()
        };

        return builder;
    }

    public void LoadDataFromJsonFile() {
        string  buildPath = System.IO.File.ReadAllText($"{p_BUILD_DIR_}{p_BuildFile}");
        Builder build     = new Builder();
        build = LoadDataFromString(buildPath);
        _AbilitiesBuilder = build.GetAbilitiesFromBuild();
        _SpellsBuilder = build.GetSpellsFromBuild();
    }

    public List<SpellScript> GetSpellsFromBuild() {
        return _SpellsBuilder;
    }

    public List<Ability> GetAbilitiesFromBuild() {
        return _AbilitiesBuilder;
    }

    public Character GetCharacterFromBuild() {
        return _Character;
    }

    public void DeleteSpells() {
        if (p_BuildFile == null) { return; }
        Debug.Log($"Build Manager Deleting: {p_SPELL_DATA_DIR_}/{p_BuildFile}");
        System.IO.File.Delete($"{p_SPELL_DATA_DIR_}/{p_BuildFile}");
    }

    public void DeleteAbilities() {
        if (p_BuildFile == null) { return; }
        Debug.Log($"Build Manager Deleting: {p_Ability_Data_Dir_}/{p_BuildFile}");
        System.IO.File.Delete($"{p_Ability_Data_Dir_}/{p_BuildFile}");
    }

    public override string ToString() {
        BuildTable BuildColumn = new BuildTable(_BuildTitle, _Character.id, _AbilitiesBuilder, _SpellsBuilder);
        return JsonUtility.ToJson(BuildColumn);
    }
    //
    // private class AbilitiesBuilder : Builder {
    //     public List<Ability> _ListAbilities = new List<Ability>();
    //     
    //     
    //     // --------------------------------------------Constructors-------------------------------------------- \\
    //     public AbilitiesBuilder() {}
    //
    //
    //     // --------------------------------------------Getters-------------------------------------------- \\
    //     public static AbilitiesBuilder LoadDataFromString(string toolboxJson) {
    //         ToolData toolbox = JsonUtility.FromJson<ToolData>(toolboxJson);
    //         AbilitiesBuilder abilities = new AbilitiesBuilder {
    //             // p_BuildFile = toolbox._Name,
    //             _ListAbilities = toolbox.AbilityToolDataAggregate()
    //         };
    //         return abilities;
    //     }
    //
    //
    //     // public static AbilitiesBuilder[] loadLocalToolAbilities() {}
    //
    //
    //     public void Delete() {
    //         if (p_BuildFile == null) { return; }
    //         Debug.Log($"Build Manager Deleting: {p_Ability_Data_Dir_}/{p_BuildFile}");
    //         System.IO.File.Delete($"{p_Ability_Data_Dir_}/{p_BuildFile}");
    //     }
    //
    // }
    //
    //
    // private class SpellsBuilder: Builder {
    //     public                  List<Spell> _ListSpells       = new List<Spell>();
    //
    //     
    //     
    //     // --------------------------------------------Constructors-------------------------------------------- \\
    //     public SpellsBuilder() {}
    //
    //
    //
    //     // --------------------------------------------Getters-------------------------------------------- \\
    //     public static SpellsBuilder LoadDataFromString(string toolboxJson) {
    //         ToolData toolbox = JsonUtility.FromJson<ToolData>(toolboxJson);
    //         SpellsBuilder spells = new SpellsBuilder {
    //             _ListSpells = toolbox.SpellToolDataAggregate()
    //         };
    //         return spells;
    //     }
    //
    //
    //     // public static SpellsBuilder[] loadLocalToolAbilities() {
    //     //     NotImplementedException();
    //     // }
    //
    //
    //     public void Delete() {
    //         if (p_BuildFile == null) { return; }
    //         Debug.Log($"Build Manager Deleting: {p_SPELL_DATA_DIR_}/{p_BuildFile}");
    //         System.IO.File.Delete($"{p_SPELL_DATA_DIR_}/{p_BuildFile}");
    //     }
    //
    // }

}
