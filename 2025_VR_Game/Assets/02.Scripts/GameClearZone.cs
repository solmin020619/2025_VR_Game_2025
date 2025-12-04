using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 닿은 녀석이 플레이어("Player")라면?
        if (other.CompareTag("Player"))
        {
            Debug.Log("탈출 성공!");
            // 로비 씬으로 이동 (씬 이름 대소문자 정확해야 함)
            SceneManager.LoadScene("LobbyScene");
        }
    }
}