using System.Collections;
using UnityEngine;

public class VR_SpikeTrap : MonoBehaviour
{
    [Header("Spike Move")]
    public Transform spikes;
    public Transform spikeStartPos;
    public Transform spikeUpPos;

    public float riseTime = 0.3f;
    public float stayTime = 0.6f;
    public float downTime = 0.3f;
    public float intervalTime = 1.0f;

    [Header("Damage")]
    public int damage = 20;
    public float damageCooldown = 0.5f;

    [Header("Hit Filter")]
    public LayerMask playerLayers = ~0;   // 필요하면 Player 레이어만 체크
    public bool requirePlayerHealth = true;

    bool isUp;
    float nextDamageTime = -999f;

    void Start()
    {
        if (spikes == null) spikes = transform;

        if (spikeStartPos == null || spikeUpPos == null)
        {
            Debug.LogError($"[{name}] spikeStartPos / spikeUpPos 비어있음");
            enabled = false;
            return;
        }

        spikes.position = spikeStartPos.position;
        StartCoroutine(TrapLoop());
    }

    IEnumerator TrapLoop()
    {
        while (true)
        {
            isUp = false;
            yield return MoveSpike(spikeStartPos.position, spikeUpPos.position, riseTime);

            isUp = true;
            yield return new WaitForSeconds(stayTime);

            isUp = false;
            yield return MoveSpike(spikeUpPos.position, spikeStartPos.position, downTime);

            yield return new WaitForSeconds(intervalTime);
        }
    }

    IEnumerator MoveSpike(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        float inv = 1f / Mathf.Max(duration, 0.0001f);

        while (t < 1f)
        {
            t += Time.deltaTime * inv;
            spikes.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        spikes.position = to;
    }

    void OnTriggerEnter(Collider other) => TryDamage(other);
    void OnTriggerStay(Collider other) => TryDamage(other);

    void TryDamage(Collider other)
    {
        if (!isUp) return;
        if (Time.time < nextDamageTime) return;

        // 1) 레이어로 1차 필터 (선택)
        if (((1 << other.gameObject.layer) & playerLayers.value) == 0)
            return;

        // 2) XR 구조 대응: 부모까지 포함해서 PlayerHealth 탐색
        var health = other.GetComponentInParent<PlayerHealth>();

        if (requirePlayerHealth && health == null)
            return;

        if (health != null)
        {
            health.TakeDamage(damage);
            nextDamageTime = Time.time + damageCooldown;
            Debug.Log($"[SpikeTrap] HIT {other.name} -{damage}");
        }
    }
}
