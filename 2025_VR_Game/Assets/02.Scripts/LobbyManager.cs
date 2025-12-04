using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void GameStart()
    {
        // 여기를 "BasicScene"에서 "Map Test"로 수정! (띄어쓰기 중요)
        SceneManager.LoadScene("Map Test");
    }

    public void GameExit()
    {
        Debug.Log("게임 종료 버튼 눌림!");
        Application.Quit();
    }
}