using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class ToolBarUI : MonoBehaviour {
    [Header("Components")]
    public List<SpellTooltip> equippedScripts = new List<SpellTooltip>();
    public SpellToolbox toolbox;
    public Transform    rootPanel;
    public GameObject   spellTooltipIcon;

    [Header("Texts")]
    public Transform rootSpellPanel;
    public TMP_Text  currentManaText;
    public TMP_Text  manaRegenText;
    public TMP_Text  selectedText;
    public TMP_Text  castingText;

    private bool Allocated;


    
    public void Init() {
        Allocated = true;
        for (int i = 0; i < toolbox.spellCount; i++) {
            SpellTooltip script = Instantiate(spellTooltipIcon, rootSpellPanel).GetComponent<SpellTooltip>();
            script.gameObject.SetActive(false);
            equippedScripts.Add(script);
        }
    }

    private void Update() {
        if (!Allocated)
            return;

        if (toolbox.IsCasting()) {
            castingText.gameObject.SetActive(true);
            castingText.text = toolbox.activeSpell.name;
        }
        else {
            castingText.gameObject.SetActive(false);
        }
        currentManaText.text = toolbox._CurrentMana.ToString("0.0");
        manaRegenText.text = toolbox._ManaRegen.ToString("0.0") + "/sec";

        for (int i = 0; i < equippedScripts.Count; i++) {
            if (toolbox.scripts.Count > i) {
                equippedScripts[i].gameObject.SetActive(true);
                equippedScripts[i].EmplaceAction(toolbox.scripts[i], toolbox.scripts[i].ManaCost < toolbox._CurrentMana);
                if (toolbox.currentIndex == i) {
                    selectedText.gameObject.SetActive(true);
                    selectedText.text = toolbox.scripts[i].name;
                    equippedScripts[i].Selected(true);
                }
                else {
                    equippedScripts[i].Selected(false);
                }
            }
            else {
                equippedScripts[i].gameObject.SetActive(false);
            }
        }
        if (toolbox.currentIndex < 0) {
            selectedText.gameObject.SetActive(false);
        }
    }
}
