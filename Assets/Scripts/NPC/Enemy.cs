using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;



/// <remarks>https://ebookreading.net/view/book/EB9781783553655_68.html</remarks>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Enemy : EnemyController {
    
    public enum PlayerState {
        Idle,
        Died,
    }

    public enum State {
        Idle,
        Wander,
        Hunting,
        ChasingPlayer,
        ChasingEnemy,
        Attacking,
        Fleeing,
        Dead,
    }

    [Header("Components")]
    private State       rootState;
    public List<State> states;
    public Enemy[]     Allies;
    public Rigidbody   rigbody;

    public bool IsSpawner = false;
    public bool IsHunting;
    public bool IsAttacking;
    public bool IsFleeing;
    
    public int  aggressionScale;
    public bool ranged;
    public bool melee;
    public int  attackPreference;

    [Header("Actions")]
    public GameObject activeTargetObj;
    public SpellScript baseSpellAttack;
    public float       aggressionRange;
    public float       attackRange;
    public float       maxAttackRange;
    public float       minAttackRange;
    

    [Header("Movement")]
    public Transform activeAttackTarget;
    public PlayerHealth targetPlayerHealth;
    public Health       nonPlayerTargetHealth;
    public float        noticeRange   = 150f;
    public float        trackingRange = 20.0f;
    public float        chaseRange    = 20.0f;
    public float        rotSpeed      = 8.0f;
    public float        FOVAngle;



    public virtual void Start() {
        // root = State.Idle;
    }
    
    

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>https://learning.oreilly.com/library/view/unity-game-development/</remarks>
    /// <returns></returns>
    private NodeState DieState() {
        Debug.Log("Die: Enter");
        Destroy(this.gameObject);
        return NodeState.SUCCESS;
    }



    protected bool NoTarget() {
        return (activeAttackTarget == null);
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>https://learning.oreilly.com/library/view/unity-game-development/</remarks>
    /// <returns></returns>
    protected float GetDistanceToTarget() {
        return (transform.position - activeAttackTarget.transform.position).magnitude;
    }

    protected Vector3 GetTargetPosition() {
        return transform.position - activeAttackTarget.transform.position;
    }
    
    /*public KeyValuePair<float, Avatar> Was for multiplayer */
    protected float GetPlayerDistance() {
        Vector3 player = AvatarController.Instance.transform.position;
        return Vector3.Distance(transform.position, player);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float AttackTargetPruning() {
        float bestScore = float.MaxValue;
        foreach (Enemy enemy in GameManager.Instance.Enemies.Where(enemy => enemy.alliance != alliance)) {
            Debug.Assert(enemy != null);
            float newScore = 0f;
            // distance check
            float distanceToTarget = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToTarget > noticeRange) continue;
            newScore += distanceToTarget;

            float angleDifference = Mathf.Abs(Vector3.SignedAngle(transform.forward, enemy.transform.position - activeAttackTarget.position, Vector3.up));
            if (angleDifference > FOVAngle) { continue; }
            newScore += angleDifference;

            // evaluate scoring
            if (newScore <= bestScore) {
                bestScore = newScore;
                // Maybe Vector3 over Transform?
                activeAttackTarget = enemy.transform;
            }
        }

        return Vector3.Distance(transform.position, activeAttackTarget.transform.position);
    }


    /// <summary>
    /// 
    /// </summary>
    private void Evaluate() {
        // Find out the name of the function we want to call
        string methodName = rootState.ToString() + "State";

        // Searches this class for a function with the name of 
        // state + State (for example: idleState)
        System.Reflection.MethodInfo info = GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }


    public void OverriddenDefense() {}


    protected void RotateTowardsTarget() {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(activeAttackTarget.position - transform.position), rotSpeed * Time.deltaTime);
    }


    private void MoveTowardsPlayer(float moveSpeed) {}
    public override void GetFromPool(string a, Vector3 b, Quaternion c) { throw new NotImplementedException(); }
}
