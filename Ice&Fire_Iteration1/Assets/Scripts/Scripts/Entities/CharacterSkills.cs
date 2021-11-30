using System;



[Serializable] public class CharacterSkills {
    public float _InitValue;
    public float _LevelModifier;
    public float _Multiplier;
    
    private float p_StatBonus;

    public float CurrentValue {
        get { return (_InitValue + _LevelModifier + p_StatBonus) * _Multiplier; }
    }
}
