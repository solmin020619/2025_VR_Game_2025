using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    [HideInInspector] public int currentHP;
    public bool isDead { get; private set; }   // 외부에서 읽기 가능, 내부에서만 수정

    public float invincibleTime = 0.3f;
    private float lastHitTime = -999f;

    void Start()
    {
        currentHP = maxHP;
        isDead = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (Time.time - lastHitTime < invincibleTime) return;
        lastHitTime = Time.time;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log("Damage " + amount + " → HP: " + currentHP);

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player Dead");
    }
}
