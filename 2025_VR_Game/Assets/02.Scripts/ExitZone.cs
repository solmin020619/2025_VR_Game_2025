using UnityEngine;

public class ExitZone : MonoBehaviour
{
    public string playerTag = "Player";
    public string needTreasureMessage = "보물을 먼저 찾으세요!";
    public string clearMessage = "탈출 성공!";

    bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // 플레이어가 자식 콜라이더(카메라/손)로 닿아도 처리되게
        bool isPlayer = other.CompareTag(playerTag) ||
                        (other.transform.root != null && other.transform.root.CompareTag(playerTag));

        if (!isPlayer) return;

        var collector = other.GetComponentInParent<TreasureCollector>();
        if (collector == null)
        {
            // 보물 시스템이 아직 없으면 안내만
            GameSessionManager.I?.ShowInfo(needTreasureMessage);
            return;
        }

        if (!collector.hasTreasure)
        {
            GameSessionManager.I?.ShowInfo(needTreasureMessage);
            return;
        }

        triggered = true;

        GameSessionManager.I?.ClearGame(clearMessage);
        // 필요하면 여기서 출구 이펙트/문 열기/이동 막기 추가 가능
    }
}
