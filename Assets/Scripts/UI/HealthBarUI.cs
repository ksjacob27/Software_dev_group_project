using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class HealthBarUI : MonoBehaviour {
    private PlayerAvatar p_Player;
    private PlayerHealth _Health;
    
// public PlayerHealth Health {
    //     get { return _Health; }
    //     set {
    //         _Health = value;
            // _Health.OnStatusApplied += (Effect status) => {
            //     ConditionTemplate data = Effect.();
            //     Debug.Log(data.name + " Added");
            //     var chip = GameObject.Instantiate(statusChipPrefab, statusEffectParent).GetComponent<StatusChip>();
            //     chip.SetSprite(data.sprite);
            //     currentStatuses.Add(data.name, chip);
            // };

            // _Health.OnStatusRemoved += (Status status) => {
            //     StatusData data = status.GetData();
            //     Debug.Log(data.name + " Removed");
            //     var chip = currentStatuses[data.name];
            //     Destroy(chip.gameObject);
            //     currentStatuses.Remove(data.name);            
            // };
        // }
    // }
    // private Dictionary<string, DebuffChip> p_Debuffs = new Dictionary<string, DebuffChip>();
    // private Dictionary<string, BuffChip>   p_Buffs   = new Dictionary<string, BuffChip>();
    
    public Image    _ArmorBarImage;
    public Image    _HealthBarImage;
    public TMP_Text _PercentHealth;
    public TMP_Text _PercentArmor;
    // public GameObject 

    public PlayerHealth Health {
        get { return _Health; }
        set { _Health = value; }
    }


    void Update() {
        if (!Health) { return; }
        string currentHealth = _Health.currentHealth.ToString("F0");
        string maxHealth     = _Health.maxHealth.ToString("F0");
        _PercentHealth.text = $"{currentHealth} / {maxHealth}";
        _HealthBarImage.fillAmount = _Health.currentHealth / _Health.maxHealth;

        _PercentArmor.text = _Health.armor.ToString("F0");
        _ArmorBarImage.fillAmount = _Health.maxArmor > 0 ? _Health.armor / _Health.maxArmor : 0f;
    }    
}
