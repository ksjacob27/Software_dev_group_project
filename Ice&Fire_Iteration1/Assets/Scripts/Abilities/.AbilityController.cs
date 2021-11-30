using System;
using UnityEngine;



namespace Spells {

    public class AbilityController : MonoBehaviour {
        public                 GameObject p_AbilityPrefab;
        public                 Vector3    p_CurrentPosition;
        public                 Vector3    p_AssetMoveInput;
        public                 RaycastHit p_SwingAngle;
        [Range(1, 200)] public float      p_RevSpeed = 20;
        public                 Rigidbody  abilityBody;



        private AbilityController(Vector3 p_CurrentPosition, Vector3 p_TargetedPosition, Vector3 p_AssetMoveInput, float p_RevSpeed = 20f) {

            this.abilityBody = new Rigidbody();
        }


        public void Awake() {
            abilityBody.transform.position = Vector3.MoveTowards(transform.position, p_SwingAngle.normal, p_RevSpeed * Time.deltaTime);

        }
    }
}
