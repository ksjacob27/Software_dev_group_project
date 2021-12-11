using System;
using UnityEngine;



public class Golem : Enemy {
    public  Allegiance type = Allegiance.Golem;
    public  GameObject GolemPrefab;
    public  EnemyType  rank;
    private Node       root;

    [Header("Attacking")]
    // public Action MeleeAttack;
    public Spell mainAttack;
    public Spell secondaryAttack;



    public void Awake() {
        health.maxHealth = maxHealth.CurrentValue;
        health.maxArmor = maxArmor.CurrentValue;
        rigbody = GetComponent<Rigidbody>();
        rigbody.useGravity = true;
        rigbody.freezeRotation = true;
    }


    public override void Start() {
        Initialize();
        mainAttack.Initialize();
        secondaryAttack.Initialize();
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        moveSpeed = 8.0f;
        root = new Selector(new Node[] { new Node(FollowState), new Node(AttackState), new Node(IdleState) });
    }


    protected override void Update() {
        if (health.IsDead || IsSpawner || IsAttacking) { return; }
        root.Evaluate();
    }


    /// <remarks>https://ebookreading.net/view/book/EB9781783553655.html</remarks>
    /// <returns></returns>
    private NodeState IdleState() {
        //OnEnter
        Debug.Log("Idle: Enter");

        return NodeState.SUCCESS;

    }


    /// <remarks>https://learning.oreilly.com/library/view/unity-game-development/</remarks>
    /// <returns></returns>
    private /*IEnumerator*/ NodeState FollowState() {
        Debug.Log("Follow: Enter");
        if (NoTarget() || (GetDistanceToTarget() > noticeRange)) { return NodeState.FAILURE; }

        transform.position = Vector3.MoveTowards(transform.position, activeAttackTarget.position, Time.deltaTime * moveSpeed);
        RotateTowardsTarget();
        float forwardSpeed = Vector3.Project(agent.desiredVelocity, transform.right).magnitude;
        animator.SetBool("Walk Forward", forwardSpeed > 0);
        Debug.Log("Follow: Exit");
        // Evaluate();
        return NodeState.SUCCESS;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <remarks>https://learning.oreilly.com/library/view/unity-game-development/</remarks>
    /// <returns></returns>
    private NodeState AttackState() {
        if (!NoTarget()) {
            float distance = GetTargetPosition().sqrMagnitude;

            if (distance > maxAttackRange) {
                agent.SetDestination(new Ray(activeAttackTarget.transform.position, GetTargetPosition()).GetPoint(maxAttackRange));
            }
            else if (distance < minAttackRange) {
                // move away if range preferred.
                if (attackPreference == 2) {
                    // Vector3 gapPosition = activeAttackTarget.transform.position.normalized * -3f;
                    agent.SetDestination(new Ray(activeAttackTarget.transform.position, GetTargetPosition()).GetPoint(minAttackRange));
                }
            }
            else {
                IsAttacking = true;
                agent.SetDestination(transform.position);
                mainAttack.transform.LookAt(activeAttackTarget.transform.position);
                mainAttack.Cast();
            }
        }
        else {
            AttackTargetPruning();
            // activeAttackTarget = alliance.FindClosestAlly(transform.position, aggressionRange).transform;
            return NodeState.FAILURE;
        }

        return NodeState.SUCCESS;
    }

}
