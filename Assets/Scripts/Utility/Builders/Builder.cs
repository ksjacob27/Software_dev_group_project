using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Build Factory class to aggregate, format, and instantiate objects from a remote data source.
/// </summary>
public class Builder {
    protected               string            p_BuildFile;
    public                  string            _BuildTitle;
    private static readonly string            p_SPELL_DATA_DIR_ = $"{Application.persistentDataPath}/SpellToolBoxes";
    private static readonly string            p_BUILD_DIR_      = $"{Application.persistentDataPath}/Builds/";
    public                  List<SpellScript> scripts;
    public                  Character         _Character;

    
    /// <summary>
    /// Constructors
    /// </summary>
    public Builder() {}
    public Builder(string fileName) { p_BuildFile = fileName; }

    
    public static Builder LoadDataFromString(string toolBoxJson) {
        BuildTable build = JsonUtility.FromJson<BuildTable>(toolBoxJson);
        Builder builder = new Builder {
            _BuildTitle = build._Name,
            scripts = build.SpellToolDataAggregate()
        };

        return builder;
    }
    

    /// <summary>
    /// Retrieve data from a save stored locally in json format.
    /// </summary>
    public void LoadDataFromJsonFile() {
        string  buildPath = System.IO.File.ReadAllText($"{p_BUILD_DIR_}{p_BuildFile}");
        Builder build     = new Builder();
        build = LoadDataFromString(buildPath);
        scripts = build.GetSpellsFromBuild();
    }
    

    /// <summary>
    ///  Retrieve the spell list from the builder
    /// </summary>
    /// <returns></returns>
    public List<SpellScript> GetSpellsFromBuild() {
        return scripts;
    }
    
    
    /*
    /// <summary>
    /// Retrieve the list of abilities from the builder
    /// </summary>
    /// <returns></returns>
    public List<Ability> GetAbilitiesFromBuild() {
        return _AbilitiesBuilder;
    }*/
    
    
    /// <summary>
    /// Retrieve the PlayerAvatar from the builder
    /// </summary>
    /// <returns></returns>
    public Character GetCharacterFromBuild() {
        return _Character;
    }
    

    /// <summary>
    /// Delete the build file containing the saved abilities.
    /// </summary>
    public void DeleteSpells() {
        if (p_BuildFile == null) { return; }
        Debug.Log($"Build Manager Deleting: {p_SPELL_DATA_DIR_}/{p_BuildFile}");
        System.IO.File.Delete($"{p_SPELL_DATA_DIR_}/{p_BuildFile}");
    }


    /// <summary>
    /// Constructs a readable snapshot of the build data under the selected table column. 
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
        BuildTable BuildColumn = new BuildTable(_BuildTitle, _Character.char_id, scripts);
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
    // }s
}
