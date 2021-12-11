using System;
using UnityEngine;
using UnityEngine.AI;



public class EnemyController : CharacterSkeleton, IPoolableObject {

    protected NavMeshAgent agent;
    protected Vector2      smoothDeltaPosition = Vector2.zero;
    protected Vector2      velocity            = Vector2.zero;



    private void Start() {
        base.Initiate();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }


    private void OnEnable() {
        if (agent) { return; }
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(transform.position, out closestHit, 100f, 1)) {
            transform.position = closestHit.position;
            agent = gameObject.GetComponent<NavMeshAgent>();
            agent.baseOffset = 0f;
        }
    }


    public override void ApplyBuff(Health   h, ConditionInventory con) { throw new NotImplementedException(); }
    public override void ApplyDebuff(Health h, ConditionInventory con) { throw new NotImplementedException(); }
    public override void AfflictDamage(Health other, float amount) {
        other.OnDamageTaken(amount, this);
    }


    // public override void TakeDamage(float impact) {
    //     
    // }

    public void Initialize() {}



    protected virtual void Update() {}



    public override void GetFromPool(string a, Vector3 b, Quaternion c) { throw new NotImplementedException(); }
}
