using System.Linq;
using UnityEngine;


namespace Spells {
    
    public class ThrowOnCast {
        [SerializeField][Range(1, 200)] public float m_AttackRange = 100f;
        public                                 float m_Velocity;

    }
}
