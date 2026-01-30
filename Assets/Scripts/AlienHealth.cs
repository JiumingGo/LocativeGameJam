using System;
using UnityEngine;

public class AlienHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;

    private int hp;

    private void Awake()
    {
        hp = maxHP;
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;
        
        if (hp <= 0)
            Die();
    }
    private void Die()
    {
        FindFirstObjectByType<AREncounterController>()?.RegisterKill();
        Destroy(gameObject);
    }
}
