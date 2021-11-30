using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class HealthBar : MonoBehaviour {
    private Player                         p_Player;
    private PlayerHealth                   p_Health;
    private Dictionary<string, DebuffChip> p_Debuffs = new Dictionary<string, DebuffChip>();
    private Dictionary<string, BuffChip>   p_Buffs   = new Dictionary<string, BuffChip>();

    public Image    _HealthBarImage;
    public TMP_Text _PercentHealth;
    // public GameObject 

    public PlayerHealth Health {
        get { return p_Health; }
        set { p_Health = value; }
    }


    void Update() {
        _HealthBarImage.fillAmount = Mathf.Clamp((int)(p_Health._CurrentHealth / p_Health._MaxHealth), 0, 1f);
    }
}
