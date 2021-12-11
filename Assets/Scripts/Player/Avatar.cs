using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
///  A character creation system similar to the system used by Divinity Original Sin 2 Official Mod tutorial
/// </summary>
/// /* Tutorial: https://kybernetik.com.au/animancer/docs/introduction/features */
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Avatar : CharacterSkeleton {
    [HideInInspector] public uint player_ID;

    [Header("Components")]
    public PlayerAvatar playerAvi;
    public Rigidbody         rigbody;
    public SpellToolbox      toolbox;
    public List<SpellScript> scripts;
    public AvatarController  controller;
    public Plane             surfacePlane;

    [Header("Camera")]
    public Camera playerCamera;
    public GameObject TargetIndicatorPrefab;
    public GameObject TargetIndicator;
    // public enum CameraDirection { x, z }
    // public                   Transform       cameraLocation;
    // public                   Transform       aimPoint;
    // public                   CameraDirection cameraDirection = CameraDirection.x;
    // public                   float           cameraHeight    = 50f;
    // public                   float           cameraDistance  = 50;
    // [HideInInspector] public float           lookSpeed       = 3.0f;
    // [HideInInspector] public float           lookXLimit      = 45f;

    [Header("Movement")]
    public float jumpHeight = 20f;
    // [Range(1, 50)] private   float speed             = 20f;
    private const float BaseGravity = 20f;
    // [HideInInspector] public bool  isRunning         = false;
    // [HideInInspector] public bool  canCast           = true;
    // [HideInInspector] public float maxVelocityChange = 20.0f;
    // private static readonly  int   s_Velx            = Animator.StringToHash("velx");
    // private static readonly  int   s_Vely            = Animator.StringToHash("vely");

    private LayerMask LayerGround;
    private LayerMask LayerPlayer;
    private LayerMask LayerEnemy;
    private LayerMask LayerDefault;

    public KeyCode SpecCastButton    = KeyCode.Q;
    public KeyCode PrimaryCastButton = KeyCode.Mouse0;

    [Header("Living Status")]
    private static readonly int _Dying = Animator.StringToHash("Dying");
    private static readonly int _Alive  = Animator.StringToHash("Alive");
    private static readonly int _Moving = Animator.StringToHash("isMoving");

    [Header("Skills")]
    public CharacterTrait _ManaRegen;
    public CharacterTrait _MaxMana;
    public CharacterTrait _SwingSpeed;
    public CharacterTrait _SpellAttackRange;
    public CharacterTrait _AbilityAttackRange;

    public                   bool didCast    = false;
    public                   bool didAct     = false;
    public                   bool isGrounded = false;
    public                   bool isMoving   = false;
    [HideInInspector] public bool canMove    = true;
    [HideInInspector] public bool canJump    = true;
    public                   bool Allocated { get;                    private set; }
    public                   bool DidCast   { get { return didCast; } set { didCast = value; } }
    public                   bool DidAct    { get { return didAct; }  set { didAct = value; } }



    // Start Character
    public void Initiate(PlayerAvatar playerObj) {
        base.Initiate();
        playerAvi = playerObj;
        // aimPoint = playerCamera.transform;
        health = GetComponent<PlayerHealth>();
        toolbox = GetComponent<SpellToolbox>();
        animator = GetComponent<Animator>();
        controller = GetComponent<AvatarController>();
        playerCamera = GetComponent<Camera>();

        animator.SetFloat("VelocityX", 0);
        animator.SetFloat("VelocityZ", 0);
        animator.SetBool("isJumping", false);
        animator.SetBool("isCasting", false);
        animator.SetBool("Dying", false);
        animator.SetBool("Alive", true);

        if (TargetIndicatorPrefab) {
            TargetIndicator.SetActive(false);
        }


        health.OnDeath += (damage) => {
            animator.SetTrigger(_Dying);
            // playeravi.PlayerDead();
        };

        ((PlayerHealth)health).OnRespawn += (heal) => {
            animator.SetTrigger(_Alive);
        };

        Allocated = true;

    }

    public void OnEnable() {}

    void Awake() {
        health.maxHealth = maxHealth.CurrentValue;
        health.maxArmor = maxArmor.CurrentValue;
        rigbody = GetComponent<Rigidbody>();
        rigbody.useGravity = false;
        rigbody.freezeRotation = true;

        if (TargetIndicatorPrefab) {
            TargetIndicator = Instantiate(TargetIndicatorPrefab, Vector3.zero, Quaternion.identity);
        }
        Cursor.visible = false;
    }


    // void FixedUpdate() {
    //     PrimaryActionButtonState.EvaluateInput();
    //     cameraDistance = Mathf.Clamp(cameraDistance + Input.GetAxis("Mouse ScrollWheel") * 5, 30, 60);
    //     cameraHeight = Mathf.Clamp(cameraHeight + Input.GetAxis("Mouse ScrollWheel") * 5, 50, 100);
    //
    //     // isGrounded = Physics.Raycast(.position, Vector3.down, 0.1f, ground);
    //
    //     /*
    //      * https://sharpcoderblog.com/blog/top-down-character-controller-example
    //      */
    //     // Camera offset
    //     Vector3 cameraOffset = Vector3.zero;
    //     if (cameraDirection == CameraDirection.x) {
    //         cameraOffset = new Vector3(cameraDistance, cameraHeight, 0);
    //     }
    //     else if (cameraDirection == CameraDirection.z) {
    //         cameraOffset = new Vector3(0, cameraHeight, cameraDistance);
    //     }
    //
    //
    //     if (canMove /*&& IsGrounded()*/) {
    //         float step = speed * Time.deltaTime; // calculate distance to move
    //         animator.ResetTrigger("isJumping");
    //
    //
    //         Vector3 targetVelocity = Vector3.zero;
    //         if (cameraDirection == CameraDirection.x) {
    //             targetVelocity = new Vector3(Input.GetAxis("Vertical") * (cameraDistance >= 0 ? -1 : 1), 0, Input.GetAxis("Horizontal") * (cameraDistance >= 0 ? 1 : -1));
    //         }
    //         else if (cameraDirection == CameraDirection.z) {
    //             targetVelocity = new Vector3(Input.GetAxis("Horizontal") * (cameraDistance >= 0 ? -1 : 1), 0, Input.GetAxis("Vertical") * (cameraDistance >= 0 ? -1 : 1));
    //         }
    //
    //         targetVelocity *= speed;
    //         Vector3 velocity       = rigidbody.velocity;
    //         Vector3 velocityChange = (targetVelocity - velocity);
    //         velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
    //         velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
    //         velocityChange.y = 0f;
    //         rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    //
    //         if (Input.GetKey(KeyCode.W)) {
    //             // transform.position += Vector3.ClampMagnitude(transform.forward, 1f) * step;
    //             rigidbody.AddForce(-transform.forward * step * Time.deltaTime, ForceMode.Force);
    //             // transform.position = Vector3.MoveTowards(transform.position, TargetIndicator.transform.position, step);
    //         }
    //         if (Input.GetKey(KeyCode.S)) {
    //             // transform.position -= Vector3.ClampMagnitude(transform.forward, 1f) * step;
    //             rigidbody.AddForce(transform.forward * step * Time.deltaTime, ForceMode.Force);
    //         }
    //         if (Input.GetKey(KeyCode.A)) {
    //             // transform.position -= Vector3.ClampMagnitude(transform.right, 1f) * step;
    //             rigidbody.AddForce(-transform.right * step * Time.deltaTime, ForceMode.Force);
    //         }
    //         if (Input.GetKey(KeyCode.D)) {
    //             // transform.position += Vector3.ClampMagnitude(transform.right, 1f) * step;
    //             rigidbody.AddForce(transform.right * step * Time.deltaTime, ForceMode.Force);
    //         }
    //         animator.SetFloat("VelocityX", velocity.x, 0.1f, Time.deltaTime);
    //         animator.SetFloat("VelocityZ", velocity.z, 0.1f, Time.deltaTime);
    //
    //         // TODO
    //         if (canJump && Input.GetButton("Jump")) {
    //             rigidbody.AddForce(transform.up * jumpHeight);
    //         }
    //     }
    //     rigidbody.AddForce(new Vector3(0, -BaseGravity * rigidbody.mass, 0));
    //     // canMove = false;
    //
    //
    //     //Camera follow
    //     playerCamera.transform.position = Vector3.Slerp(transform.position, transform.position + cameraOffset, Time.deltaTime * 7.4f);
    //     playerCamera.transform.LookAt(transform.position);
    //     playerCamera.transform.position = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y + 5, playerCamera.transform.position.z + 5);
    //
    //
    //     //Aim target position and rotation
    //     TargetIndicator.transform.position = GetAimTargetPos();
    //     TargetIndicator.transform.LookAt(new Vector3(transform.position.x, TargetIndicator.transform.position.y, transform.position.z));
    //     //Player rotation
    //     transform.LookAt(new Vector3(TargetIndicator.transform.position.x, transform.position.y, TargetIndicator.transform.position.z));
    //
    //
    //     UpdateSkillSet();
    //     CheckInput();

    // }



    public void Update() {
        UpdateSkillSet();
    }


    /// <summary>
    /// 
    /// </summary>
    private void OnCollisionStay() {
        canMove = true;
    }


    /// <summary>
    /// 
    /// </summary>
    private void UpdateSkillSet() {
        health.maxHealth = maxHealth.CurrentValue;
        health.maxArmor = maxArmor.CurrentValue;

        toolbox.switchDelay = 1 / _SwingSpeed.CurrentValue;
        toolbox._ManaRegen = _ManaRegen.CurrentValue;
        toolbox._MaximumMana = _MaxMana.CurrentValue;
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="spellIndex"></param>
    private void SelectSpell(int spellIndex) {
        toolbox.ScriptSelection(spellIndex);
    }


    /// <summary>
    /// 
    /// </summary>
    private void SpecialCast() {
        toolbox.SpecialFire();
        DidCast = true;
    }

    
    /// <summary>
    /// 
    /// </summary>
    public void BasicCast() {
        Debug.Log("BasicCast() called!");
        toolbox.BasicFire();
    }


    /// <summary>
    /// 
    /// </summary>
    private void CancelAttack() {
        Debug.Log("CancelSpell() called!");
        toolbox.CancelCasting();
        DidCast = false;
    }


    /// <summary>
    /// Animation Trigger
    /// </summary>
    private void OnAttack() {
        animator.SetTrigger("Fire");
    }
    

    // void onColisionStay() {}

    /*/// <summary>
    /// Hit Trigger
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Projectile>() != null) {
            // other.GetComponent<Projectile>(). --health.currentHealth;
            if (health.currentHealth == 0) { GameManager.Destroy(gameObject); }
        }
    }*/



    private void CastReport(Vector3 mousePoint) {
        Debug.Log("CastReport called");


    }



    // ---------------------------------------------------- Required Overrides ---------------------------------------------------- \\
    public override void ApplyBuff(Health     h,     ConditionInventory con)    { playerAvi.PostBuff(h, con); }
    public override void ApplyDebuff(Health   h,     ConditionInventory con)    { playerAvi.PostDebuff(h, con); }
    public override void AfflictDamage(Health other, float   amount)          { playerAvi.DealDamage(other, amount); }
    public override void GetFromPool(string   a,     Vector3 b, Quaternion c) { throw new NotImplementedException(); }



    // ----------------------------------------------------Checkers---------------------------------------------------- \\
    public bool IsGrounded() {
        float distToGround = GetComponent<Collider>().bounds.extents.y;
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }


    private bool CheckMoving() {
        isMoving = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D));
        return isMoving;
    }


    private float CalculateJumpVerticalSpeed() {
        return Mathf.Sqrt(2 * jumpHeight * BaseGravity);
    }


    /*
    * https://sharpcoderblog.com/blog/top-down-character-controller-example
    */
    private Vector3 GetAimTargetPos() {
        //Update surface plane
        surfacePlane.SetNormalAndPosition(Vector3.up, transform.position);

        //Create a ray from the Mouse click position
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (surfacePlane.Raycast(ray, out float enter)) {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);
            hitPoint.y += 1;
            return hitPoint;
        }

        //No raycast hit, hide the aim target by moving it far away
        return new Vector3(-5000, -5000, -5000);
    }

}
