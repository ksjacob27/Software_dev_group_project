using UnityEngine;



/* Tutorial: https://kybernetik.com.au/platformer/docs/characters/states/flinch/ */
public class PlayerHealth : Health {
    public Entity _Entity;
    public float  _HalfHealth;

    public event OnHealthEvent OnRespawn;
    public event OnDamageEvent OnDeath;


    
    protected override void Start() {
        base.Start();

        OnRespawn += (damage) => {};
    }

    private void Update() {}

    public override void OnHealthChanged(float currentHealth, float newHealth) {
        base.OnHealthChanged(currentHealth, newHealth);

        if (currentHealth < 1 && newHealth > 1 && OnRespawn != null) {
            OnRespawn(newHealth);
        }

    }


    public override void Die() {
        Debug.Log($"Entity {_Entity._Player._playerName} - {_Entity._EntityNetId}: Died.");
        _IsDead = true;
    }
    
    
    public void Respawn() {
        _CurrentHealth = _MaxHealth;
        _IsDead = false;
        OnRespawn?.Invoke(_CurrentHealth);
    }
}
