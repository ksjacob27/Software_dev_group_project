using UnityEngine;
using TMPro;
using UnityEngine.UI;



/*
 * Attempt to offer a World Of Warcraft style actionbar abilities template.
 * (need to broaden to include physical abilities, and consumables. 
 */
public class SpellTooltip : MonoBehaviour {
    public bool     castable;
    public Image    thumbnail;
    public Image    selectedHighlight;
    public TMP_Text manaCostText;

    
    
    public void EmplaceAction(SpellScript script, bool canCast) {
        this.thumbnail.sprite = script._SpellSprite;
        this.manaCostText.text = script.ManaCost.ToString();
        castable = canCast;
    }

    
    public void Selected(bool state) {
        selectedHighlight.gameObject.SetActive(state);
    }
}
