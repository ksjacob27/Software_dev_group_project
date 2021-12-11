using System;
using JetBrains.Annotations;
using UnityEngine;


/* Tutorials: 
    https://titanwolf.org/Network/Articles/Article?AID=bbfd7597-9471-4a0e-9c80-667ac8528213
    
*/
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class AvatarController : MonoBehaviour {
    public static AvatarController Instance { get; [UsedImplicitly] private set; }
    public        Avatar           avatar;
    private       LayerMask        LayerGround;
    private       LayerMask        LayerPlayer;
    private       LayerMask        LayerEnemy;
    private       LayerMask        LayerDefault;
    public        Plane            surfacePlane;

    [Header("Firing")]
    public KeyCode specialCast = KeyCode.Q;
    public GameObject projectilePrefab;
    public Transform  aimPoint;

    [Header("Movement Settings")]
    [Range(1, 20)] public float moveSpeed = 8f;
    public                   float turnSensitivity   = 5f;
    public                   float maxTurnSpeed      = 150f;
    private const            float BaseGravity       = 20f;
    [HideInInspector] public bool  isRunning         = false;
    [HideInInspector] public bool  canCast           = true;
    [HideInInspector] public float maxVelocityChange = 2f;
    private static readonly  int   m_VelocityX            = Animator.StringToHash("velx");
    private static readonly  int   m_VelocityY            = Animator.StringToHash("vely");

    [Header("Diagnostics")]
    public float horizontal;
    public float vertical;
    public float turn;
    public float jumpSpeed;
    public bool  isGrounded = true;
    public bool  isFalling;

    [Header("Camera Settings")]
    public Transform cameraLocation;
    public enum CameraDirection { x, z }
    public                   CameraDirection cameraDirection = CameraDirection.x;
    public                   float           cameraHeight    = 25f;
    public                   float           cameraDistance  = 30f;
    [HideInInspector] public float           lookSpeed       = 3.0f;
    [HideInInspector] public float           lookXLimit      = 45f;

    [Header("Animations")]
    public string animationTrigger_Casting = "Cast Spell";
    public string animationTrigger_Postpone = "isCasting";
    public string animationTrigger_Throw = "Punch Attack";
    public string animationTrigger_TakeHit = "Take Damage";
    public string animationTrigger_Defense = "Defend";
    public string animationTrigger_Die = "Die";
    public string animationTrigger_Jump = "Jump";
    
    [Header("Ability Settings")]
    public KeyCode SpecCastButton = KeyCode.Q;
    public KeyCode PrimaryCastButton = KeyCode.Mouse0;



    private class ActionButtonState {
        private readonly string p_ID;
        private          bool   p_Down;
        private          bool   p_Held;
        private          bool   p_Up;
        private          bool   p_Released;

        public bool DOWN     { get { return p_Down; }     set {} }
        public bool HELD     { get { return p_Held; }     set {} }
        public bool UP       { get { return p_Up; }       set {} }
        public bool RELEASED { get { return p_Released; } set {} }



        public ActionButtonState(string buttonId) { p_ID = buttonId; }


        public void EvaluateInput() {
            p_Held = Input.GetButton(p_ID);
            p_Down = Input.GetButtonDown(p_ID);
            p_Released = Input.GetButtonUp(p_ID);
            p_Up = !p_Held;
        }
    }

    private readonly ActionButtonState SpecActionButtonState = new ActionButtonState("SpecialCast");
    public           bool              SpecCastDown     { get { return SpecActionButtonState.DOWN; } }
    public           bool              SpecCastHeld     { get { return SpecActionButtonState.HELD; } }
    public           bool              SpecCastReleased { get { return SpecActionButtonState.RELEASED; } }

    private readonly ActionButtonState PrimaryActionButtonState = new ActionButtonState("BasicCast");
    public           bool              PrimaryCastDown     { get { return PrimaryActionButtonState.DOWN; } }
    public           bool              PrimaryCastHeld     { get { return PrimaryActionButtonState.HELD; } }
    public           bool              PrimaryCastReleased { get { return PrimaryActionButtonState.RELEASED; } }

    private readonly ActionButtonState CancelCastActionButtonState = new ActionButtonState("CancelCast");
    public           bool              CancelSpellDown     { get { return CancelCastActionButtonState.DOWN; } }
    public           bool              CancelSpellHeld     { get { return CancelCastActionButtonState.HELD; } }
    public           bool              CancelSpellReleased { get { return CancelCastActionButtonState.RELEASED; } }



    // public void OnValidate() {
    //     // if (characterController == null) { characterController = GetComponent<CharacterController>(); }
    //     
    //
    //     // GetComponent<Rigidbody>().isKinematic = true;
    //     // GetComponent<GameManager>()
    // }


    public void Awake() {
        if (avatar.playerCamera == null) { avatar.playerCamera = GetComponent<Camera>(); }
        avatar.rigbody = GetComponent<Rigidbody>();
        avatar.rigbody.useGravity = false;
        avatar.rigbody.freezeRotation = true;
    }


    public void Start() {
        // if (characterController == null) { characterController = GetComponent<CharacterController>(); }

        avatar = GetComponent<Avatar>();
        avatar.playerCamera.orthographic = true;
        avatar.playerCamera.transform.SetParent(transform);
        avatar.playerCamera.transform.localPosition = new Vector3(0f, 3f, -8f);
        avatar.playerCamera.transform.localEulerAngles = new Vector3(10f, 0f, 0f);

        LayerGround = LayerMask.NameToLayer("Ground");
        LayerPlayer = LayerMask.NameToLayer("Player");
        LayerEnemy = LayerMask.NameToLayer("Enemy");
        LayerDefault = LayerMask.NameToLayer("Default");

        // characterController.enabled = true;
    }


    public void OnDisable() {
        if (avatar.playerCamera != null) {
            avatar.playerCamera.orthographic = true;
            avatar.playerCamera.transform.SetParent(null);
            avatar.playerCamera.transform.localPosition = new Vector3(0f, 70f, 0f);
            avatar.playerCamera.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
        }
    }



    void Update() {
        PrimaryActionButtonState.EvaluateInput();

        // if (characterController == null || !characterController.enabled) return;

        // horizontal = Input.GetAxis("Horizontal");
        // vertical = Input.GetAxis("Vertical");

        // // Q and E cancel each other out, reducing the turn to zero
        // if (Input.GetKey(KeyCode.A))
        //     turn = Mathf.MoveTowards(turn, -maxTurnSpeed, turnSensitivity);
        // if (Input.GetKey(KeyCode.D))
        //     turn = Mathf.MoveTowards(turn, maxTurnSpeed, turnSensitivity);
        // if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E))
        //     turn = Mathf.MoveTowards(turn, 0, turnSensitivity);
        // if (!Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
        //     turn = Mathf.MoveTowards(turn, 0, turnSensitivity);

        if (isGrounded) {
            isFalling = false;
        }

        if ((isGrounded || !isFalling) && jumpSpeed < 1f && Input.GetKey(KeyCode.Space)) {
            jumpSpeed = Mathf.Lerp(jumpSpeed, 1f, 0.5f);
        }
        else if (!isGrounded) {
            isFalling = true;
            jumpSpeed = 0;
        }


        CheckInput();
    }


    void FixedUpdate() {
        PrimaryActionButtonState.EvaluateInput();
        cameraHeight = Mathf.Clamp(cameraHeight + Input.GetAxis("Mouse ScrollWheel") * 2, 25, 60);
        cameraDistance = Mathf.Clamp(cameraDistance + Input.GetAxis("Mouse ScrollWheel") * 2, 30, 100);

        // isGrounded = Physics.Raycast(.position, Vector3.down, 0.1f, ground);

        /* * https://sharpcoderblog.com/blog/top-down-character-controller-example */
        // Camera offset
        Vector3 cameraOffset = Vector3.zero;
        if (cameraDirection == CameraDirection.x) {
            cameraOffset = new Vector3(cameraDistance, cameraHeight, 0);
        }
        else if (cameraDirection == CameraDirection.z) {
            cameraOffset = new Vector3(0, cameraHeight, cameraDistance);
        }


        if (avatar.canMove /*&& IsGrounded()*/) {
            // float step = moveSpeed * Time.deltaTime; // calculate distance to move
            avatar.animator.ResetTrigger("isJumping");


            Vector3 targetVelocity = Vector3.zero;
            if (cameraDirection == CameraDirection.x) {
                targetVelocity = new Vector3(Input.GetAxis("Vertical") * (cameraDistance >= 0 ? -1 : 1), 0, Input.GetAxis("Horizontal") * (cameraDistance >= 0 ? 1 : -1));
            }
            else if (cameraDirection == CameraDirection.z) {
                targetVelocity = new Vector3(Input.GetAxis("Horizontal") * (cameraDistance >= 0 ? -1 : 1), 0, Input.GetAxis("Vertical") * (cameraDistance >= 0 ? -1 : 1));
            }

            targetVelocity *= moveSpeed;
            Vector3 velocity       = avatar.rigbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0f;
            avatar.rigbody.AddForce(velocityChange, ForceMode.VelocityChange);

            if (Input.GetKey(KeyCode.W)) {
                // transform.position += Vector3.ClampMagnitude(transform.forward, 1f) * step;
                avatar.rigbody.AddForce(-transform.forward * (moveSpeed), ForceMode.Force);
                // transform.position = Vector3.MoveTowards(transform.position, TargetIndicator.transform.position, step);
            }
            if (Input.GetKey(KeyCode.S)) {
                // transform.position -= Vector3.ClampMagnitude(transform.forward, 1f) * step;
                avatar.rigbody.AddForce(transform.forward * (moveSpeed), ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.A)) {
                // transform.position -= Vector3.ClampMagnitude(transform.right, 1f) * step;
                avatar.rigbody.AddForce(-transform.right * (moveSpeed), ForceMode.Force);
            }
            if (Input.GetKey(KeyCode.D)) {
                // transform.position += Vector3.ClampMagnitude(transform.right, 1f) * step;
                avatar.rigbody.AddForce(transform.right * (moveSpeed), ForceMode.Force);
            }
            avatar.animator.SetFloat("VelocityX", velocity.x, 0.1f, Time.deltaTime);
            avatar.animator.SetFloat("VelocityZ", velocity.z, 0.1f, Time.deltaTime);

            // TODO
            if (avatar.canJump && Input.GetButton("Jump")) {
                avatar.rigbody.AddForce(transform.up * avatar.jumpHeight);
            }
        }
        avatar.rigbody.AddForce(new Vector3(0, -BaseGravity * avatar.rigbody.mass, 0));
        // canMove = false;


        //Camera follow
        avatar.playerCamera.transform.position = Vector3.Slerp(transform.position, transform.position + cameraOffset, Time.deltaTime * 7.4f);
        avatar.playerCamera.transform.LookAt(transform.position);
        avatar.playerCamera.transform.position = new Vector3(avatar.playerCamera.transform.position.x + cameraOffset.x, avatar.playerCamera.transform.position.y + cameraOffset.y, avatar.playerCamera.transform.position.z + cameraOffset.z);


        //Aim target position and rotation
        avatar.TargetIndicator.transform.position = GetAimTargetPos();
        avatar.TargetIndicator.transform.LookAt(new Vector3(transform.position.x, avatar.TargetIndicator.transform.position.y, transform.position.z));
        //Player rotation
        transform.LookAt(new Vector3(avatar.TargetIndicator.transform.position.x, transform.position.y, avatar.TargetIndicator.transform.position.z));


        //-------------------------------------------------------------------------------------------------------------\\
        transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);

        Vector3 direction = new Vector3(horizontal, jumpSpeed, vertical);
        direction = Vector3.ClampMagnitude(direction, 1f);
        direction = transform.TransformDirection(direction);
        direction *= moveSpeed;

        if (jumpSpeed > 0) { avatar.transform.up = (direction * Time.fixedDeltaTime); }
        // else
        // avatar.transform.SimpleMove(direction);

        isGrounded = avatar.isGrounded;
    }


    /*
   * https://sharpcoderblog.com/blog/top-down-character-controller-example
   */
    private Vector3 GetAimTargetPos() {
        //Update surface plane
        surfacePlane.SetNormalAndPosition(Vector3.up, transform.position);

        //Create a ray from the Mouse click position
        Ray ray = avatar.playerCamera.ScreenPointToRay(Input.mousePosition);

        if (surfacePlane.Raycast(ray, out float enter)) {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);
            hitPoint.y += 1;
            return hitPoint;
        }

        //No raycast hit, hide the aim target by moving it far away
        return new Vector3(-5000, -5000, -5000);
    }


    private void CheckInput() {
        if (PrimaryCastDown || SpecCastDown) {
            // Check if uninterrupted.
            if (canCast) {
                // Can only cast one spell at a time. Primary filler spell takes priority.
                if (PrimaryCastDown) {
                    Debug.Log("BaseCast Pressed!");

                    if (PrimaryCastHeld) {
                        Debug.Log("BaseCast Held!");
                        avatar.toolbox.Postpone();
                    }
                    else {
                        BasicCast();
                    }
                }
                // Specialized spell if Primary is not called.
                else if (SpecCastDown) {
                    Debug.Log("SpecCast Pressed!");
                    if (SpecCastHeld) {
                        Debug.Log("SpecCast Held!");
                        avatar.toolbox.Postpone();
                    }
                    else {
                        SpecialCast();
                    }
                }
            }
        }

        // if (CastActionButtonState.DOWN) {
        //     Cast();
        // }
        // if (CastActionButtonState.HELD) {
        //     toolbox.Postpone();
        // }
        // if (CastActionButtonState.RELEASED) {
        //     toolbox.Release();
        // }
        CancelCastActionButtonState.EvaluateInput();
        if (CancelCastActionButtonState.DOWN) {
            CancelSpell();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SelectSpell(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SelectSpell(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SelectSpell(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SelectSpell(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            SelectSpell(4);
        }
    }




    private void SelectSpell(int spellIndex) {
        avatar.toolbox.ScriptSelection(spellIndex);
    }


    private void SpecialCast() {
        avatar.toolbox.SpecialFire();
        avatar.DidCast = true;
    }


    public void BasicCast() {
        Debug.Log("BasicCast() called!");
        avatar.toolbox.BasicFire();
    }


    private void CancelSpell() {
        Debug.Log("CancelSpell() called!");
        avatar.toolbox.CancelCasting();
        avatar.DidCast = false;
    }


    // Called on the Avatar that fired for all other observers.
    private void OnAttack() {
        avatar.animator.SetTrigger("Fire");
    }


    void onColisionStay() {}


    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Projectile>() != null) {
            // other.GetComponent<Projectile>(). --health.currentHealth;
            if (avatar.health.currentHealth == 0) { GameManager.Destroy(gameObject); }
        }
    }

}
