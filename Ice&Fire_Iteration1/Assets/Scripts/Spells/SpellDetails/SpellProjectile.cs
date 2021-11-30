using System;
using System.Linq;
using Mirror;
using Spells;
using UnityEngine;



[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class SpellProjectile : NetworkBehaviour, IPoolableObject {

    public Entity              _Caster;
    public IProjectileMovement _Mover;
    public Rigidbody           _Rigidbody;
    public SpellEffect         _SpellEffect;
    public float               _DistanceSinceInvoke;
    public Vector3             _Destination;



    // UNUSED
    public void Init()    {}
    public void RpcInit() {}


    public void OnInvoke(Vector3 direction, Vector3 angle, IProjectileMovement objectMover) {
        _Mover = objectMover;
        _Mover.OnInvoke(direction);
        _Rigidbody = GetComponent<Rigidbody>();
        _Destination = angle;


        bool hasTriggerCollider = GetComponents<Collider>().Any(col => col.isTrigger);
        Debug.Assert(hasTriggerCollider, "Object needs a Collider Component that is a trigger");
    }


    private void FixedUpdate() {
        if (_Caster._SpellAttackRange.CurrentValue <= _DistanceSinceInvoke) Destroy();

        _DistanceSinceInvoke += _Caster._SwingSpeed.CurrentValue * Time.deltaTime;
        _Rigidbody.velocity = _Mover.TranslateForward(_DistanceSinceInvoke) * _Caster._SwingSpeed.CurrentValue;
    }


    private void Destroy() {}


    [ServerCallback] private void OnCollisionEnter(Collision other) {
        Health health = other.gameObject.GetComponent<Health>();

        if (health) {
            _SpellEffect.RgstHitHealth(health);
        }

        _SpellEffect.RgstHitAny(other.gameObject);
        _Rigidbody.velocity = Vector3.zero;
        _Rigidbody.angularVelocity = Vector3.zero;
        _Rigidbody.isKinematic = true;
    }
}
