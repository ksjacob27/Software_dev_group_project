using System.Collections;
using System.Linq;
using UnityEngine;
using Mirror;



/*
	Documentation:  https://mirror-networking.gitbook.io/docs/guides/communications/remote-actions
	API:
*/

// [CreateAssetMenu(fileName = "NewSpell", menuName = "ScriptableObjects/Spells", order = 0)]
public class Spell : NetworkBehaviour, IPoolableObject /*, ScriptableObject*/ {
    public Entity  _Caster;
    public bool    _ParentToCaster;


    public delegate void    CastAction();
    public event CastAction OnCast;
    public event CastAction OnPostpone;
    public event CastAction OnRelease;
    public event CastAction OnFinish;
    public event CastAction OnCancelCast;


    public delegate void        HitAnyResponse(GameObject other);
    public event HitAnyResponse OnAnyHitBy;

    public delegate void         HitHealthAction(Health target);
    public event HitHealthAction OnHitHealth;

    public bool _Anchored;
    public bool _DeflectDuration;

    private float         _SpellEnd;
    private SpellEffect[] _SEffects;



    /// <summary>
    /// 
    /// </summary>
    public void Init() {
        // Register event defaults
        OnCast += () => {};
        OnPostpone += () => {};
        OnRelease += () => {};
        OnFinish += () => {};
        OnAnyHitBy += (other) => {};
        OnHitHealth += (target) => {};
        OnCancelCast += () => {};

        // Register all spell effects to effect events
        _SEffects = GetComponents<SpellEffect>();
        foreach (SpellEffect effect in _SEffects) {
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
    public void Cast() {
        if (_ParentToCaster) {
            // transform.SetParent(_Caster.aimTransform);
        }

        OnCast?.Invoke();
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
        if (_SEffects.Any(effect => !effect.RgstDone())) { return false; }
        Command_Done();
        return true;
    }

    
    /// <summary>
    /// Initiates the process of despawning the object from the local client. 
    /// </summary>
    public void Despawn() {
        if (hasAuthority) {
            Command_Despawn();
        }
    }

    // ---------------------------------------------------- Remote Command Call-Backs ---------------------------------------------------- \\
    /// <summary>
    /// Clean-up. Call to inform that a remote subroutine has finished.
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
    /// Client call that Initiates the despawning process on the server.
    /// </summary>
    [Command] public void Command_Despawn() {
        StartCoroutine(WaitThenDespawn(5f));
    }


    /// <summary>
    /// Initiates the process of despawning the object from the network 
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
