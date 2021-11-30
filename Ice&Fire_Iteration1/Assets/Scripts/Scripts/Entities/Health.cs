using System.Collections.Generic;
using Mirror;
using UnityEngine;



/* Tutorial: https://kybernetik.com.au/platformer/docs/characters/states/flinch/ */
public class Health : NetworkBehaviour {

    // ---------------------------------------------------- Server Variables ---------------------------------------------------- \\
    [SyncVar]                                    public float _Armor;
    [SyncVar(hook = nameof(OnMaxArmorChanged))]  public float _MaxArmor;
    [SyncVar(hook = nameof(OnMaxHealthChanged))] public float _MaxHealth;
    [SyncVar(hook = nameof(OnHealthChanged))]    public float _CurrentHealth;
    [SyncVar]                                    public bool  _IsDead;
    [SyncVar]                                    public bool  _IsHalfHealth;
    [SyncVar]                                    public float _HalfHealth;

    public delegate void       OnDamageEvent(float damage);
    public event OnDamageEvent OnDeath;
    public event OnDamageEvent OnHealthDamaged;
    public event OnDamageEvent OnArmorBroken;
    public event OnDamageEvent OnArmorDamaged;
    public event OnDamageEvent WhenDamaged;

    public delegate void       OnHealthEvent(float healing);
    public event OnHealthEvent OnHealthReplenish;
    public event OnHealthEvent OnRevive;

    public delegate void        ConditionEvent(Condition status);
    public event ConditionEvent WhenDebuffApplied;
    public event ConditionEvent WhenDebuffRemoved;
    public event ConditionEvent WhenBuffApplied;
    public event ConditionEvent WhenBuffRemoved;


    // Status/Condition containers
    public List<ConditionInventory> _ActiveDebuffs = new List<ConditionInventory>();
    public List<ConditionInventory> _ActiveBuffs   = new List<ConditionInventory>();

    public List<ConditionInventory> CurrentDebuffEffects {
        get { return _ActiveDebuffs; }
    }
    public List<ConditionInventory> CurrentBuffEffects {
        get { return _ActiveBuffs; }
    }



    // NetworkBehaviour Override
    protected virtual void Start() {
        OnDeath += damage => {};
        WhenDamaged += damage => {};
        OnHealthDamaged += damage => {};
        OnArmorBroken += damage => {};
        OnArmorDamaged += damage => {};
        OnHealthReplenish += healing => {};
        OnRevive += healing => {};
        WhenDebuffApplied += data => {};
        WhenDebuffRemoved += data => {};
        WhenBuffApplied += data => {};
        WhenBuffRemoved += data => {};
    }


    public virtual void Die() {
        _ActiveDebuffs.Clear();
        _ActiveBuffs.Clear();
    }


    // NetworkBehaviour Override
    public override void OnStartServer() {
        ObjectPool.Register(GetComponent<NetworkIdentity>().netId, gameObject);
        _CurrentHealth = _MaxHealth;
    }


    // ---------------------------------------------------- Server Callbacks ---------------------------------------------------- \\
    [Server] public virtual void OnTakeDamage(float damage, EntitySkeleton origin) {
        if (_IsDead) { return; }


        if (_MaxArmor > 0) {
            float damageTaken = damage - _MaxArmor;
            if (damageTaken > 0) {
                _MaxArmor = 0;
                OnArmorBroken?.Invoke(damage);
                _CurrentHealth -= damageTaken;
                OnHealthDamaged?.Invoke(damageTaken);
            }
            else {
                _MaxArmor -= damage;
                OnArmorDamaged?.Invoke(damage);
            }
        }
        else {
            _CurrentHealth -= damage;
            OnHealthDamaged?.Invoke(damage);
        }

        WhenDamaged?.Invoke(damage);
        _HalfHealth = _MaxHealth / 2;
        _IsHalfHealth = (_HalfHealth >= _CurrentHealth);

        if (_CurrentHealth <= 0) {
            _IsDead = true;
            OnDeath?.Invoke(damage);
            Die();
        }


    }


    [Server] public virtual void OnReceiveHealing(float healing, EntitySkeleton origin) {
        if (_IsDead) {}

    }


    // Append or update a Debuff condition.
    [Server] public void AddDebuff(ConditionInventory condition, EntitySkeleton origin) {
        if (_ActiveDebuffs.Contains(condition))
            return;
        Condition con = condition.GetCondition(this, origin);
        _ActiveDebuffs.Add(condition);

        con.OnDropDebuffCondition += () => {
            _ActiveDebuffs.Remove(condition);
            WhenDebuffRemoved?.Invoke(con);
        };
        con.AddDebuffCondition();
        WhenDebuffApplied?.Invoke(con);
    }


    // Append or update a Buff condition.
    [Server] public void AddBuff(ConditionInventory condition, EntitySkeleton origin) {
        if (_ActiveBuffs.Contains(condition))
            return;
        Condition con = condition.GetCondition(this, origin);
        _ActiveBuffs.Add(condition);

        con.OnDropBuffCondition += () => {
            _ActiveBuffs.Remove(condition);
            WhenBuffRemoved?.Invoke(con);
        };
        con.AddBuffCondition();
        WhenBuffApplied?.Invoke(con);
    }


    public virtual void OnHealthChanged(float oldVal, float newVal) {
        if (newVal < 1)
            Die();
    }


    public virtual void OnMaxHealthChanged(float oldVal, float newVal) {
        if (_CurrentHealth > newVal) _CurrentHealth = newVal;
    }


    public virtual void OnMaxArmorChanged(float oldVal, float newVal) {
        if (_MaxArmor > newVal) _MaxArmor = newVal;
    }

}
