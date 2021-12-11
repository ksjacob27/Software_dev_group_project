using UnityEngine;




public class PlayerHealth : Health {
    public Avatar   avatar;
    // public event OnDamageEvent OnDeath;
    public event OnHealthEvent OnRespawn;

    
    
    protected override void Start() {
        base.Start();
        OnRespawn += (health) => {};
    }

    public override void OnHealthChanged(float currHealth, float newHealth) {
        base.OnHealthChanged(currHealth, newHealth);
        if (currentHealth < 1 && newHealth > 1 && OnRespawn != null) { OnRespawn(newHealth); }
    }

    
    /// <summary>
    /// Respawn event
    /// </summary>
    public void WhenRespawn() {
        currentHealth = maxHealth;
        IsDead = false;
        OnRespawn?.Invoke(halfHealth);
    }  
    
    
    /// <summary>
    /// Death event
    /// </summary>
    public void WhenDead() {
        currentHealth = 0;
        IsDead = true;
        Die();
    }
}
