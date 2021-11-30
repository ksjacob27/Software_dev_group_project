using System.Collections.Generic;
using UnityEngine;



namespace Spells {
    /// <summary>
    /// Takes care of the aiming for a spell.
    /// </summary>
    public abstract class Aimer {
        protected readonly Ability ChargedAbility;

        
        
        protected Aimer(Ability aimedSpell) {
            Debug.Assert(aimedSpell != null);
            ChargedAbility = aimedSpell;
        }

        
        /// <summary>Rotation to target</summary>
        protected Quaternion TargetRotation { get; set; }

        
        /// <summary>Sets the TargetRotation according to the players input</summary>
        /// <param name="snapAngle"> The angle variation, in degrees, from the player's mouseclick index to a possible target</param>
        /// <param name="mouseInput"> The player's mouse index.</param>
        /// <param name="currentPos"> The player's movement trajectory.</param>
        public abstract bool DoPlayerAimAssist(float snapAngle, Vector3 mouseInput, Vector3 currentPos);

        
        /// <summary>Robot target aiming</summary>
        /// <param name="targets"> The list of targets to consider</param>
        /// <returns> Whether a valid target was found for the spell </returns>
        public abstract bool DoEnemyAim(List<Player> targets);
    }
}
