using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // Start is called before the first frame update
    void Damage(int amount, uint shooterID);

    void Die();
    
       
    
}
