using System;
using UnityEngine;




public class SpellController : MonoBehaviour {
    public                 GameObject p_SpellPrefab;
    public                 Vector3    p_CasterPosition;
    public                 Vector3    p_CasterMomentum;
    public                 RaycastHit p_CastDirection;
    [Range(1, 200)] public float      p_TravelSpeed = 20;
    public                 Rigidbody  spellBody;



    private SpellController(Vector3 p_CasterPosition, Vector3 p_CastDirection, Vector3 p_CasterMomentum, float p_TravelSpeed = 50f) {

        this.spellBody = new Rigidbody();
    }


    public void Awake() {
        spellBody.transform.position = Vector3.MoveTowards(transform.position, p_CastDirection.normal, p_TravelSpeed * Time.deltaTime);

    }
}
