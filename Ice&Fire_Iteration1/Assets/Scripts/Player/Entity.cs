using UnityEngine;
using Mirror;


/*
 * https://mirror-networking.gitbook.io/docs/community-guides/quick-start-guide#part-4
 */
public class Entity : EntitySkeleton {

    // Server Variable \\
    [SyncVar] public uint _EntityNetId;

    // Variables \\
    public  Player              _Player;
    private CharacterController p_CharacterController;
    public  AbilityToolbox      _AToolBox;
    public  SpellToolbox        _SToolBox;
    public  CharacterSkills     _jumpSpeed;


    [Header("Skills")]
    public CharacterSkills _ManaRegen;
    public CharacterSkills _MaxMana;
    public CharacterSkills _SwingSpeed;
    public CharacterSkills _SpellAttackRange;
    public CharacterSkills _AbilityAttackRange;
    


    public float _SpeedModifier = 2f;
    public float _Gravity       = 20f;

    public                   float     _LookSpeed     = 2.0f;
    public                   float     _LookXLimit    = 45f;
    public                   Vector3   observerOffset = new Vector3(8f, 4f, 12f);
    public                   Vector3   _moveDirection = Vector3.zero;
    public                   float     _rotationX     = 0;
    public                   Transform cameraLocation;
    public                   Camera    _PlayerCamera;
    [HideInInspector] public bool      canMove = true;
    [HideInInspector] public bool      Initiated;


    private bool p_DidCast = false;
    private bool p_DidAct  = false;
    public  bool DidCast { get { return p_DidCast; } set { p_DidAct = value; } }
    public  bool DidAct  { get { return p_DidAct; }  set { p_DidAct = value; } }

    public  AnimationClip              currentCastAnimation;
    private AnimatorOverrideController overrideController;



    private readonly ActionButtonState CastActionButtonState = new ActionButtonState("Cast");
    public           bool        CastDown     { get { return CastActionButtonState.DOWN; } }
    public           bool        CastHeld     { get { return CastActionButtonState.HELD; } }
    public           bool        CastReleased { get { return CastActionButtonState.RELEASED; } }

    private readonly ActionButtonState CancelSpellActionButtonState = new ActionButtonState("CancelCast");
    public           bool        CancelSpellDown     { get { return CancelSpellActionButtonState.DOWN; } }
    public           bool        CancelSpellHeld     { get { return CancelSpellActionButtonState.HELD; } }
    public           bool        CancelSpellReleased { get { return CancelSpellActionButtonState.RELEASED; } }


    private readonly ActionButtonState ActionActionButtonState = new ActionButtonState("Action");
    public           bool        ActionDown     { get { return ActionActionButtonState.DOWN; } }
    public           bool        ActionHeld     { get { return ActionActionButtonState.HELD; } }
    public           bool        ActionReleased { get { return ActionActionButtonState.RELEASED; } }

    private readonly ActionButtonState CancelActionActionButtonState = new ActionButtonState("CancelAction");
    public           bool        CancelActionDown     { get { return CancelActionActionButtonState.DOWN; } }
    public           bool        CancelActionHeld     { get { return CancelActionActionButtonState.HELD; } }
    public           bool        CancelActionReleased { get { return CancelActionActionButtonState.RELEASED; } }


    public                  Vector3 _LookDirection;
    private static readonly int     p_IsJumping_  = Animator.StringToHash("isJumping");
    private static readonly int     p_Velocity_X_ = Animator.StringToHash("velocity_x");
    private static readonly int     p_Velocity_Y_ = Animator.StringToHash("velocity_y");
    private static readonly int     p_Alive_      = Animator.StringToHash("Alive");
    private static readonly int     p_Dying_      = Animator.StringToHash("Dying");

    
    private class ActionButtonState {
        private readonly string p_ButtonId;
        private          bool   down;
        private          bool   held;
        private          bool   up;
        private          bool   released;

        public bool DOWN     { get { return down; }     set { down = value; } }
        public bool HELD     { get { return held; }     set { held = value; } }
        public bool UP       { get { return up; }       set { up = value; } }
        public bool RELEASED { get { return released; } set { released = value; } }



        public ActionButtonState(string buttonId) { p_ButtonId = buttonId; }


        public void Evaluate() {
            HELD = Input.GetButton(p_ButtonId);
            DOWN = Input.GetButtonDown(p_ButtonId);
            RELEASED = Input.GetButtonUp(p_ButtonId);
            UP = !HELD;
        }
    }
    
    
    public void Init(Player player) {
        base.Init();

        this._Player = player;
        _AimPoint = _PlayerCamera.transform;
        _AToolBox = GetComponent<AbilityToolbox>();
        _SToolBox = GetComponent<SpellToolbox>();
        _Health = GetComponent<PlayerHealth>();
        _Animator.SetTrigger(p_Alive_);
        
        if (_Player.hasAuthority) {
            p_CharacterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else {
            _PlayerCamera.enabled = false;
        }
        
        Initiated = true;
    }

    
    private void Update() {
        if (!Initiated || !_Player.hasAuthority) {
            return;
        }

        if (_Health._IsDead) {
            _PlayerCamera.transform.position = transform.position + observerOffset;
            _PlayerCamera.transform.rotation = Quaternion.LookRotation(transform.position - _PlayerCamera.transform.position);
        }
        

        bool    isRunning = Input.GetMouseButtonDown(1);
        Vector3 currSpeed = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        currSpeed = canMove ? (isRunning ? currSpeed * _MoveSpeed.CurrentValue * _SpeedModifier : currSpeed * _MoveSpeed.CurrentValue) : Vector3.zero;
        float moveDirectionY = _moveDirection.y;

        _moveDirection = transform.forward * currSpeed.z + transform.right * currSpeed.x;

        if (Input.GetButton("Jump") && p_CharacterController.isGrounded) {
            _Animator.SetTrigger(p_IsJumping_);
            _moveDirection.y = _jumpSpeed.CurrentValue;
        }
        else {
            _moveDirection.y = moveDirectionY;
        }

        if (!p_CharacterController.isGrounded) {
            _moveDirection.y -= _Gravity * Time.deltaTime;
        }
        

        if (canMove) {
            _rotationX += -Input.GetAxis("Mouse Y") * _LookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -_LookXLimit, _LookXLimit);
            _PlayerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            _PlayerCamera.transform.localPosition = cameraLocation.transform.localPosition;
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _LookSpeed, 0);
        }

        p_DidCast = false;
        p_DidAct = false;
        CastActionButtonState.Evaluate();


        if (CastActionButtonState.DOWN) {
            Cast();
        }
        if (CastActionButtonState.HELD) {
            _SToolBox.Postpone();
        }
        if (CastActionButtonState.RELEASED) {
            _SToolBox.Release();
        }
        CancelSpellActionButtonState.Evaluate();
        if (CancelSpellActionButtonState.DOWN) {
            CancelSpell();
        }

    }

    

    private void SelectSpell(int spellIndex) {
        _SToolBox.Select(spellIndex);
    }

    private void Cast() {
        _SToolBox.Cast();
        p_DidCast = true;
    }

    private void CancelSpell() {
        _SToolBox.Cancel();
        DidCast = false;
    }

    private void Action() {
        _AToolBox.Action();
        DidAct = true;
    }

    private void CancelAct() {
        _AToolBox.Cancel();
        DidAct = false;
    }



    // ---------------------------------------------------- Required Overrides ---------------------------------------------------- \\
    public override void GetFromPool(string pName, Vector3 position, Quaternion rotation) {
        _Player.GetFromPool(pName, position, rotation);
    }


    public override void AddBuff(Health health, ConditionInventory condition) {
        _Player.PostBuff(health, condition);
    }


    public override void AddDebuff(Health health, ConditionInventory condition) {
        _Player.PostDebuff(health, condition);
    }


    public override void DealDamage(Health target, float amount) {
        _Player.DealDamage(target, amount);
    }
}
