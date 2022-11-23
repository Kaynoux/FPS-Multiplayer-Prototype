using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Target : NetworkBehaviour
{
    [SyncVar] public int maxHealth = 100;
    [SyncVar] public int currentHealth;
    
    


    void Start()
    {
        if(isLocalPlayer)
        {
            currentHealth = maxHealth;
            CanvasManager.instance.UpdateMaxHp(maxHealth);
        }
        
    }
    private void Update()
    {
        if(isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                TakeDamage(20);
            }
        }
        
    }
    public void TakeDamage (int amount)
    {
        currentHealth -= amount;
        CanvasManager.instance.UpdateHp(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            

        }
    }

    void Die ()
    {
        Destroy(gameObject);
    }
}
