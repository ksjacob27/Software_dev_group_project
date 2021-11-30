using System.Collections;
using System.Linq;
using UnityEngine;
using Mirror;




// [CreateAssetMenu(fileName = "NewAbility", menuName = "ScriptableObjects/Abilities", order = 0)]
public abstract class Ability : NetworkBehaviour, IPoolableObject /*, ScriptableObject*/ {
    public Entity _Actor;
    public int    _AbilityID;


    public bool    _ParentToActor;
    public string  _AbilityTitle;
    public int     _DamageOutput;
    public int     _HelpOutput;
    public bool    _Block;
    public bool    _Anchored;
    public Schools _Type;

    public string         _EffectDescription;
    public ParticleSystem _ParticleEffect;
    public AudioClip      _AudioClip;
    public Animation      _Animation;
    public GameObject     _AbilityPrefab;
    public Sprite         _AbilitySprite;
    public float          _ManaCost;
    public float          _EnergyCost;

    public bool   _Consumable;
    public bool   _Obtainable;
    public float  _Range    = 15;
    public bool   _Thrown   = false;
    public float  _Cooldown = 0.5f;
    public Sprite _ToolIcon;

    private float           _ActEnd;
    private AbilityEffect[] _AEffects;

    public delegate void      LaunchAction();
    public event LaunchAction OnAction;
    public event LaunchAction OnPostpone;
    public event LaunchAction OnRelease;
    public event LaunchAction OnFinish;
    public event LaunchAction OnCancelAction;

    public delegate void        HitAnyResponse(GameObject other);
    public event HitAnyResponse OnAnyHitBy;

    public delegate void         HitHealthAction(Health target);
    public event HitHealthAction OnHitHealth;


    /// <summary>
    /// 
    /// </summary>
    public void Init() {
        OnAction += () => {};
        OnPostpone += () => {};
        OnRelease += () => {};
        OnFinish += () => {};
        OnAnyHitBy += (other) => {};
        OnHitHealth += (target) => {};
        OnCancelAction += () => {};

        _AEffects = GetComponents<AbilityEffect>();
        foreach (AbilityEffect effect in _AEffects) {
            effect.Register(this);
        }

    }


    [ClientRpc] public void RpcInit() {
        Init();
    }


    public override void OnStartClient() {
        ObjectPool._Singleton._SpawnedObj.Add(GetComponent<NetworkIdentity>().netId, gameObject);
    }

    
    // --------------------------------------------Events-------------------------------------------- \\
    /// <summary>
    /// 
    /// </summary>
    public void Action() {
        // TODO: -Transform-
        OnAction?.Invoke();
    }


    /// <summary>
    /// 
    /// </summary>
    public void Postpone() {
        OnPostpone?.Invoke();
    }


    /// <summary>
    /// 
    /// </summary>
    public void Release() {
        OnRelease?.Invoke();
    }


    public void HitAny(GameObject other) {
        OnAnyHitBy?.Invoke(other);
    }


    /// <summary>
    /// Damage dealing collision response. 
    /// </summary>
    /// <param name="target"></param>
    public void HitHealth(Health target) {
        OnHitHealth?.Invoke(target);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool Done() {
        if (_AEffects.Any(effect => !effect.RgstDone())) { return false; }
        Command_Done();
        return true;
    }


    // --------------------------------------------Commands-------------------------------------------- \\

    /// <summary>
    /// 
    /// </summary>
    [Command] public void Command_Done() {
        Rpc_Done();
    }


    /// <summary>
    /// 
    /// </summary>
    [ClientRpc] public void Rpc_Done() {
        OnFinish?.Invoke();
    }


    /// <summary>
    /// 
    /// </summary>
    public void Despawn() {
        if (hasAuthority) {
            Command_Despawn();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Command] public void Command_Despawn() {
        StartCoroutine(WaitThenDespawn(5f));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator WaitThenDespawn(float duration) {
        yield return new WaitForSeconds(duration);
        transform.SetParent(null);
        gameObject.SetActive(false);
        NetworkServer.UnSpawn(this.gameObject);
    }

    
}
