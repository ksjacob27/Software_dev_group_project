using UnityEngine;



namespace Spells {
    
    public class StraightTypeProjectileMovement : IProjectileMovement {
        private Vector3    m_Direction;
        private Quaternion m_ForwardRotation;



        /// <summary>Initialises projectile with forward direction</summary>
        /// <param name="forwardDirection"></param>
        public void OnInvoke(Vector3 forwardDirection) {
            m_Direction = forwardDirection;
            m_ForwardRotation = Quaternion.LookRotation(forwardDirection);
        }


        /// <summary>Returns the forward direction along the distance towards the projectile destination.</summary>
        /// <param name="distanceSinceInvoke"></param>
        public Vector3 TranslateForward(double distanceSinceInvoke) {
            return m_Direction;
        }


        /// <summary>Returns the new rotation of the projectile for a given traveled distance</summary>
        /// <param name="distanceSinceInvoke"></param>
        public Quaternion Rotate(double distanceSinceInvoke) {
            return m_ForwardRotation;
        }


        /// <summary>Return a copy of the IProjectileMovement object</summary>
        /// <returns name="clonedMovement"></returns>
        public IProjectileMovement Copy() {
            StraightTypeProjectileMovement clonedMovement = new StraightTypeProjectileMovement();
            return clonedMovement;
        }
    }
}
