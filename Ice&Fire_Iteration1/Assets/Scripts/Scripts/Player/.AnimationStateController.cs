using UnityEngine;
using UnityEngine.UI;


public class AnimationStateController : MonoBehaviour {
    public                  Animator         _animator;
    public                  PlayerController _playerController;
    private static readonly int              _PSpeed_     = Animator.StringToHash("_pSpeed");
    private static readonly int              _PIsWalking_ = Animator.StringToHash("_pIsWalking");
    private static readonly int              _PIsRunning_ = Animator.StringToHash("_pIsRunning");
    private static readonly int              _PIsJumping_ = Animator.StringToHash("_pIsJumping");
    private static readonly int              _PIsEnabled_ = Animator.StringToHash("_pEnabled");


    // Start is called before the first frame update
    void Start() {
        _animator         = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        Debug.Log("Player Character Animator active.");
    }


    // Update is called once per frame
    void Update() {
        if (!Input.anyKey) { return; }

        if (_playerController.PEnabled == true) {

            // Move to PlayerController
            if (Input.GetKey("w")) {

                if (_playerController.PGrounded == true) {
                    if (_PSpeed_ < 2 && _PSpeed_ > 0) {
                        this._animator.SetBool(_PIsWalking_, true);
                    }
                    else if (_PSpeed_ > 2) {
                        this._animator.SetBool(_PIsRunning_, true);
                    }
                }
            }

            if (!Input.GetKey("w") || (!Input.GetKey("s"))) {
                if (_PSpeed_ < 1) {
                    this._animator.SetBool(_PIsWalking_, false);
                    this._animator.SetBool(_PIsRunning_, false);

                }
            }

            if (Input.GetKey("s")) {
                Debug.Log("VerticalKey: " + Input.inputString);
            }

        }

        // If the player is not enabled, but the player's speed is not zero, set player speed to zero.
        else {
            _animator.SetFloat(_PSpeed_, 0);
        }

    }

}





/*switch (Input.anyKey) {
    case true: {
        if (Input.GetKey("w")) {
            if (GetSpeed() > 2) {
                ToggleRunning();
            }
            else {
                ToggleWalking();
            }
        }
        if (Input.GetKey("s")) {
            Debug.Log("VerticalKey: " + Input.inputString);
        }

        break;
    }
}*/
