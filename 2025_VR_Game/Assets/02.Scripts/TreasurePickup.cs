using UnityEngine;

public class TreasurePickup : MonoBehaviour
{
    public string playerTag = "Player";
    public float destroyDelay = 0f;

    bool picked;

    private void OnTriggerEnter(Collider other)
    {
        if (picked) return;

        // 플레이어가 캡슐/손/카메라 등 자식 콜라이더로 들어올 수 있어서 부모까지 확인
        if (!other.CompareTag(playerTag) && (other.transform.root == null || !other.transform.root.CompareTag(playerTag)))
            return;

        picked = true;

        // 플레이어에 있는 매니저(또는 PlayerHealth 같은 곳)에 알림
        var collector = other.GetComponentInParent<TreasureCollector>();
        if (collector != null)
            collector.OnTreasureCollected();

        // 트리거 재진입/Stay 방지용으로 콜라이더 끄기
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        // 눈에 보이게 바로 숨기기
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers) r.enabled = false;

        // 필요하면 파티클/사운드 여기서

        Destroy(gameObject, destroyDelay);
    }
}
