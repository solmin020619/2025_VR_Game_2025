using System.Collections;
using UnityEngine;

public class VR_PressureSpearTrap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform spear;          // 움직일 창(메쉬/콜라이더 포함)
    [SerializeField] private Transform downPoint;      // 내려간 위치 (Empty)
    [SerializeField] private Transform upPoint;        // 올라간 위치 (Empty)
    [SerializeField] private Collider hitCollider;     // 창 콜라이더 (Trigger)

    [Header("Movement")]
    public float riseTime = 0.3f;
    public float stayTime = 0.5f;
    public float downTime = 0.3f;
    public float interval = 1.0f;

    [Header("Damage")]
    public int damage = 20;
    public float damageCooldown = 0.5f;

    private enum Phase { Down, Rising, Up, Falling }
    private Phase phase = Phase.Down;

    private float lastHitTime = -999f;

    private void Awake()
    {
        if (!spear || !downPoint || !upPoint || !hitCollider)
        {
            Debug.LogError($"[{name}] 필수 슬롯 안 채워짐 (spear/downPoint/upPoint/hitCollider)");
            enabled = false;
            return;
        }

        hitCollider.isTrigger = true;

        var rb = hitCollider.attachedRigidbody;
        if (rb == null)
        {
            rb = hitCollider.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        else
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        spear.position = downPoint.position;
        StartCoroutine(TrapLoop());
    }

    private IEnumerator TrapLoop()
    {
        while (true)
        {
            phase = Phase.Rising;
            yield return Move(downPoint.position, upPoint.position, riseTime);

            phase = Phase.Up;
            yield return new WaitForSeconds(stayTime);

            phase = Phase.Falling;
            yield return Move(upPoint.position, downPoint.position, downTime);

            phase = Phase.Down;
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator Move(Vector3 from, Vector3 to, float time)
    {
        float t = 0f;
        float dur = Mathf.Max(time, 0.0001f);

        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            spear.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        spear.position = to;
    }

    private void OnTriggerEnter(Collider other) => TryDamage(other);
    private void OnTriggerStay(Collider other) => TryDamage(other);

    private void TryDamage(Collider other)
    {
        // 내려갈 때/내려가 있을 때는 데미지 금지 (겹침 상태로 내려가며 맞는 현상 방지)
        if (phase == Phase.Down || phase == Phase.Falling) return;

        if (Time.time - lastHitTime < damageCooldown) return;

        var hp = other.GetComponentInParent<PlayerHealth>();
        if (hp == null) return;

        hp.TakeDamage(damage);
        lastHitTime = Time.time;
    }
}
