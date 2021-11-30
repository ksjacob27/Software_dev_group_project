// using System.Collections.Generic;
// using UnityEngine;
//
//
//
//
// public class SpellsBuilder {
//     private                 string      p_BuildFile;
//     public                  List<Spell> _ListSpells       = new List<Spell>();
//     private static readonly string      p_SPELL_DATA_DIR_ = $"{Application.persistentDataPath}/SpellToolBoxes";
//
//
//
//     // --------------------------------------------Constructors-------------------------------------------- \\
//     public SpellsBuilder() {}
//     public SpellsBuilder(string fileName) {
//         this.p_BuildFile = fileName;
//     }
//
//
//
//     // --------------------------------------------Getters-------------------------------------------- \\
//     public static SpellsBuilder LoadDataFromString(string toolboxJson) {
//         ToolData      toolbox = JsonUtility.FromJson<ToolData>(toolboxJson);
//         SpellsBuilder spells  = new SpellsBuilder {
//             p_BuildFile = toolbox._Name,
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
