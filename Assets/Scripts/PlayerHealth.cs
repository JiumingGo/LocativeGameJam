using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 10;
    [SerializeField] private float invincibleSeconds = 0.5f;

    public UnityEvent<int, int> onHealthChanged; // (current, max)

    private int hp;
    private float invTimer;
    
    public int CurrentHP => hp;
    public int MaxHP => maxHP;


    private void Awake()
    {
        hp = maxHP;
        onHealthChanged?.Invoke(hp, maxHP);
    }

    private void Update()
    {
        if (invTimer > 0f) invTimer -= Time.deltaTime;
    }

    public bool CanTakeDamage => invTimer <= 0f;

    public void TakeDamage(int amount)
    {
        if (!CanTakeDamage) return;

        hp = Mathf.Max(0, hp - amount);
        invTimer = invincibleSeconds;

        onHealthChanged?.Invoke(hp, maxHP);

        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Player Dead!");
        // Later: show UI, stop spawns, restart button, etc.
    }
}