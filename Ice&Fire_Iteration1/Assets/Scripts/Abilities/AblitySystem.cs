using System;


namespace Spells {
    
    public abstract class AbilitySystem {
        public enum AbilityType {
            Physical,
            Mental,
            Poison,
            Water,
            Fire,
            Help,
        }
        
        public TargetFlag[] p_HarmWhom;


    }
    
}