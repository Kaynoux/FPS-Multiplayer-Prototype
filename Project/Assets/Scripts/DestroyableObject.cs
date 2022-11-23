using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : NetworkBehaviour, IDamageable
{
    [SerializeField] int HealthMax = 1;
    int Health;

    // Start is called before the first frame update
    private void Start()
    {
        SetMaxHP();   
    }

    // Update is called once per frame
    [Server]
    public void Damage(int amount, uint shooterID)
    {
        Health -= amount;
        if (Health < 1)
        {
            Die();
        }
            
    }
    [Server]
    private void SetMaxHP()
    {
        Health = HealthMax;
    }
    [Server]
    public void Die()
    {
        Destroy(gameObject);
    }
}
