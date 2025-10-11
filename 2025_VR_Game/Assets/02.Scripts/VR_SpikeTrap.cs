using UnityEngine;

public class VR_SpikeTrap : MonoBehaviour
{
    [Header("References")]
    public Transform spikes;
    public Transform spikeStartPos;
    public Transform spikeUpPos;
    public Collider triggerZone; // 자식 TriggerZone을 연결

    [Header("Trap Settings")]
    public float riseSpeed = 3f;
    public float stayTime = 1f;
    public float resetSpeed = 2f;
    public int damage = 20;
    public string playerTag = "Player";

    private bool isActive = false;

    private void Start()
    {
        // 트리거존이 있다면 자동으로 이벤트 연결
        if (triggerZone != null)
        {
            triggerZone.isTrigger = true;
            // 이벤트 등록
            var zoneHandler = triggerZone.gameObject.AddComponent<TrapZoneHandler>();
            zoneHandler.parentTrap = this;
        }

        if (spikes && spikeStartPos)
            spikes.position = spikeStartPos.position;
    }

    // 자식이 트리거 이벤트를 감지하면 여기서 처리
    public void TriggerActivated(Collider other)
    {
        if (!isActive && other.CompareTag(playerTag))
        {
            StartCoroutine(ActivateTrap(other));
        }
    }

    private System.Collections.IEnumerator ActivateTrap(Collider target)
    {
        isActive = true;

        // 스파이크 상승
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * riseSpeed;
            spikes.position = Vector3.Lerp(spikeStartPos.position, spikeUpPos.position, t);
            yield return null;
        }

        // 데미지 적용
        var health = target.GetComponentInParent<PlayerHealth>();
        if (health != null)
            health.TakeDamage(damage);

        yield return new WaitForSeconds(stayTime);

        // 스파이크 복귀
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * resetSpeed;
            spikes.position = Vector3.Lerp(spikeUpPos.position, spikeStartPos.position, t);
            yield return null;
        }

        isActive = false;
    }
}
