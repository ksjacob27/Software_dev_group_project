using UnityEngine;



namespace Spells {
    public interface IProjectileMovement {
        void OnInvoke(Vector3 direction);

        IProjectileMovement Copy();

        Quaternion Rotate(double distanceSinceInvoke);

        Vector3 TranslateForward(double distanceSinceInvoke);
    }

}
