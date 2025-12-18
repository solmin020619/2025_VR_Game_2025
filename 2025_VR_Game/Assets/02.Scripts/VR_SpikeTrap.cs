using System.Collections;
using UnityEngine;

public class VR_SpikeTrap : MonoBehaviour
{
    [Header("Spike Move")]
    public Transform spikes;            // 움직이는 스파이크(메쉬/트리거가 붙은 오브젝트)
    public Transform spikeStartPos;     // 내려간 위치(Empty)
    public Transform spikeUpPos;        // 올라간 위치(Empty)

    public float riseTime = 0.3f;
    public float stayTime = 0.6f;
    public float downTime = 0.3f;
    public float intervalTime = 1.0f;

    [Header("Damage")]
    public int damage = 20;
    public float damageCooldown = 0.5f;   // 연속 데미지 방지
    public string playerTag = "Player";   // 태그 쓰고 싶으면 유지

    private bool isUp;
    private float lastHitTime = -999f;

    private void Start()
    {
        if (spikes == null) spikes = transform;

        // 필수 포지션 없으면 움직임 자체를 막아버림(에러 방지)
        if (spikeStartPos == null || spikeUpPos == null)
        {
            Debug.LogError($"[{name}] spikeStartPos / spikeUpPos가 비어있음! Empty 두 개 넣어줘.");
            enabled = false;
            return;
        }

        spikes.position = spikeStartPos.position;
        StartCoroutine(TrapLoop());
    }

    private IEnumerator TrapLoop()
    {
        while (true)
        {
            // 올라오기
            yield return MoveSpike(spikeStartPos.position, spikeUpPos.position, riseTime);
            isUp = true;

            yield return new WaitForSeconds(stayTime);

            // 내려가기
            isUp = false;
            yield return MoveSpike(spikeUpPos.position, spikeStartPos.position, downTime);

            yield return new WaitForSeconds(intervalTime);
        }
    }

    private IEnumerator MoveSpike(Vector3 from, Vector3 to, float duration)
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

    private void OnTriggerEnter(Collider other) => TryDamage(other);
    private void OnTriggerStay(Collider other) => TryDamage(other);

    private void TryDamage(Collider other)
    {
        if (!isUp) return;

        // 쿨다운 (OnTriggerStay로 프레임마다 깎이는거 방지)
        if (Time.time - lastHitTime < damageCooldown) return;

        // 1) 태그가 Player면 OK
        // 2) 태그가 아니어도 PlayerHealth가 부모에 있으면 OK (XR 구조 대응)
        PlayerHealth health = null;

        if (other.CompareTag(playerTag))
        {
            health = other.GetComponentInParent<PlayerHealth>();
        }
        else
        {
            // 태그 안 맞아도 부모에 PlayerHealth 있으면 맞는 걸로 처리
            health = other.GetComponentInParent<PlayerHealth>();
        }

        if (health == null) return;

        health.TakeDamage(damage);
        lastHitTime = Time.time;
    }
}
