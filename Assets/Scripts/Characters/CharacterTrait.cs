using System;



/* Tutorial: https://kybernetik.com.au/animancer/docs/introduction/features */
[Serializable]
public class CharacterTrait {

    public float standardVal;
    public float bonus;
    public float activeMultiplier;
    public float diminishedReturns;
    public float CurrentValue { get { return (standardVal + bonus) * activeMultiplier; } }

}
