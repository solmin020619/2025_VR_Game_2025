using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TreasureCollector : MonoBehaviour
{
    public Text messageText;
    public float messageSeconds = 1.5f;

    public bool hasTreasure { get; private set; }

    Coroutine msgCo;

    public void OnTreasureCollected()
    {
        if (hasTreasure) return; // 이미 들고 있으면 중복 방지(원하면 여러 개로 바꿀 수 있음)
        hasTreasure = true;

        ShowMessage("보물 획득!");
        Debug.Log("Treasure Collected");
    }

    void ShowMessage(string msg)
    {
        if (messageText == null) return;

        if (msgCo != null) StopCoroutine(msgCo);
        msgCo = StartCoroutine(MessageRoutine(msg));
    }

    IEnumerator MessageRoutine(string msg)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = msg;

        yield return new WaitForSeconds(messageSeconds);

        messageText.text = "";
        messageText.gameObject.SetActive(false);
        msgCo = null;
    }
}
