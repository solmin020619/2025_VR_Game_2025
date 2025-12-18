using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public int damage = 20;
    public float hitCooldown = 0.5f;
    float lastHit;

    private void OnTriggerStay(Collider other)
    {
        if (Time.time - lastHit < hitCooldown) return;

        // ✅ 태그 대신: 닿은 콜라이더의 "부모"에서 PlayerHealth 찾기
        var hp = other.GetComponentInParent<PlayerHealth>();
        if (hp == null) return;

        lastHit = Time.time;
        hp.TakeDamage(damage);
    }
}
