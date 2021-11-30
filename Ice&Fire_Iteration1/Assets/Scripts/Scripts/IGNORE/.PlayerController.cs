using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;




// [RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour {
    /// <summary> </summary>
    private Transform p_Player;
    private Rigidbody p_Rigidbody;
    // private Health    p_PlayerHealth;


    
    private Camera         p_Camera;
    private NavMeshAgent   p_Agent;
    private CollisionFlags p_CollisionFlags;

    [Range(1, 20)]  private float p_Speed       = 4f;
    [Range(1, 200)] private float p_AttackRange = 100f;
    [Range(1, 200)] private int   p_AttackSpeed = 20;
    private                 bool  p_IsJumping;
    private                 bool  p_IsRunning;
    private                 bool  p_IsWalking;
    private                 bool  p_IsGrounded;
    private                 bool  p_IsMoving;
    private                 bool  p_Enabled;

    private Ray        p_TargetedPosition;
    private Ray        p_TargetedAttackPosition;
    private Ray        p_AttackAngle;
    private RaycastHit p_TargetedMoveCast;
    private RaycastHit p_TargetedAttackCast;

    private Vector3 p_AssetMoveInput;
    private Vector3 p_Velocity;
    private Vector3 p_Current;
    private float   p_InitYAxis;
    private Vector3 p_Forward, p_Right;


    public MoveEntity         _buttonA;
    public MoveEntity         _buttonD;
    public MoveEntity         _buttonS;
    public MoveEntity         _buttonW;
    public UseAbilityUltimate _buttonQ;
    // private AbilityController  p_EquippedAbility;


    public bool  PEnabled  { get { return p_Enabled; }    set { p_Enabled = value; } }
    public bool  PGrounded { get { return p_IsGrounded; } set { p_IsGrounded = value; } }
    public bool  PMoving   { get { return p_IsMoving; }   set { p_IsMoving = value; } }
    public float PSpeed    { get { return p_Speed; }      set { p_Speed = value; } }
    // public int   Health    { get { return this.p_PlayerHealth._GetHealth(); }    set { this.p_PlayerHealth._SetHealth(value); } }
    // public int   MaxHealth { get { return this.p_PlayerHealth._GetMaxHealth(); } set { this.p_PlayerHealth._SetMaxHealth(value); } }



    private void Start() {
        p_Enabled = true;
        p_IsGrounded = true;
        p_IsMoving = false;
        p_IsWalking = false;
        p_IsRunning = false;
        p_IsJumping = false;

        p_Player = this.transform;
        p_Rigidbody = GetComponent<Rigidbody>();
        p_Current = p_Player.position;
        p_Forward = Vector3.Normalize(p_Player.forward);
        p_Right = Quaternion.Euler(new Vector3(0, 90, 0)) * p_Forward;
        // p_PlayerHealth = gameObject.AddComponent<Health>();

    }


    private void Update() {
        // if (!p_PlayerHealth.GetStatus()) {
        //     Death();
        // }

        if (Input.GetMouseButtonDown(0)) {
            print("MOUSECLICK: 0");
            SetTargetedAttackPosition();
        }
        if (Input.anyKeyDown) {
            HandleInput();
        }

    }

    private void FixedUpdate() {
        p_Current = p_Rigidbody.position;
        p_AssetMoveInput = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")), 1);
        p_Velocity = p_AssetMoveInput * p_Speed;

        Vector3 updatedPos = p_Current + (p_Velocity * Time.deltaTime);
        p_Rigidbody.MovePosition(updatedPos);

        (bool success, Vector3 position) = GetMousePosition();
        if (success) {
            // Calculate direction
            Vector3 direction = position - p_Player.position;
            // Ignore the height.
            direction.y = 0;
            // Make transform look in the direction.
            p_Player.forward = direction;
        }
    }

    // TODO
    private void Death() {
        p_Enabled = false;
        p_IsMoving = false;
        p_IsWalking = false;
        p_IsRunning = false;
        p_IsJumping = false;
        p_Rigidbody.Sleep();
        // Destroy(gameObject);
    }


    private (bool success, Vector3 position) GetMousePosition() {
        Ray ray = p_Camera.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity /*, groundMask*/) ? (success: true, position: hitInfo.point) : (success: false, position: Vector3.zero);
    }



    /// <summary> </summary>
    /// <param name="..."></param>
    /// <returns></returns>
    private void DoNothing() {
        p_Camera.enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;

    }


    /// <summary>
    ///     Function to interpret keyboard button presses and initiate the corresponding output. 
    /// </summary>
    private void HandleInput() {

        if (Input.GetKeyDown(KeyCode.Q)) {
            print("KEY PRESSED: Q");
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            print("KEY PRESSED: E");
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            print("KEY PRESSED: SPACE");
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            print("KEY PRESSED: R");
        }
    }


    /// <summary>
    ///     Interprets the players mouseclick position in the game world, and instigates the player objects movestate.
    /// </summary>
    private void SetTargetedMovePosition() {
        p_TargetedPosition = p_Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(p_TargetedPosition, out p_TargetedMoveCast)) {
            p_IsMoving = true;
        }

    }


    /// <summary>
    ///     
    /// </summary>
    private void SetTargetedAttackPosition() {
        p_TargetedAttackPosition = p_Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(p_TargetedAttackPosition, out p_TargetedAttackCast)) {
            // Fire();
        }

    }


    /// <summary>
    ///     Function to update the NavMeshAgent's world position by translating the Raycast hit positions. 
    /// </summary>
    private void MovePlayerToTarget() {
        p_Agent.SetDestination(p_TargetedMoveCast.point);

        if (transform.position == p_TargetedMoveCast.normal) {
            p_IsMoving = false;
        }
    }


    /// <summary>Function to handle mouse aimed ability actions by the player.</summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private void PlayerDirectedAction<T, V, E>(Commander<Ray, RaycastHit, Player> action) {
        Ray        ray = p_Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit)) {}
    }


    /// <summary>
    ///     Instantiates a players Ability component. 
    /// </summary>
    // private void Fire() {
    //     // GameObject activeAbility = Instantiate(p_EquippedAbility[], p_Current, Quaternion.LookRotation(Input.mousePosition.normalized));
    //     // GameObject activeAbility = new AbilityController(p_Current, );
    //
    // }

}
