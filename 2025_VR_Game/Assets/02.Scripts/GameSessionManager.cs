using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager I { get; private set; }

    [Header("UI")]
    public Text timerText;
    public Text infoText;

    [Header("Freeze Player (VR)")]
    public GameObject locomotionRoot;

    [Header("Clear UI")]
    public GameObject clearPanel;
    public Text clearTitleText;
    public Text clearTimeText;
    public Button retryButton;

    [Header("Freeze Player (VR)")]
    public MonoBehaviour[] locomotionToDisable;
    // 여기에 Continuous Move Provider, Continuous Turn Provider, CharacterControllerDriver 같은 "이동 관련" 컴포넌트들 넣기

    bool cleared;
    bool running;
    float startTime;
    Coroutine infoCo;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        StartRun();
        SetupClearUI();
        ShowInfo("");
    }

    void Update()
    {
        if (!running || cleared) return;
        UpdateTimerUI(GetElapsed());
    }

    void SetupClearUI()
    {
        if (clearPanel) clearPanel.SetActive(false);
        if (retryButton) retryButton.onClick.AddListener(Retry);
    }

    public void StartRun()
    {
        startTime = Time.time;
        running = true;
        cleared = false;

        if (clearPanel) clearPanel.SetActive(false);
        SetLocomotionEnabled(true);

        if (locomotionRoot != null) locomotionRoot.SetActive(true);

    }

    public float GetElapsed() => Time.time - startTime;

    void UpdateTimerUI(float seconds)
    {
        if (timerText == null) return;
        int min = Mathf.FloorToInt(seconds / 60f);
        float sec = seconds - min * 60f;
        timerText.text = $"{min:00}:{sec:00.00}";
    }

    public void ShowInfo(string msg)
    {
        if (infoText == null) return;

        if (infoCo != null) StopCoroutine(infoCo);

        if (string.IsNullOrEmpty(msg))
        {
            infoText.text = "";
            infoText.gameObject.SetActive(false);
            return;
        }

        infoCo = StartCoroutine(InfoRoutine(msg));
    }

    IEnumerator InfoRoutine(string msg)
    {
        infoText.gameObject.SetActive(true);
        infoText.text = msg;
        yield return new WaitForSeconds(1.5f);
        infoText.text = "";
        infoText.gameObject.SetActive(false);
        infoCo = null;
    }

    public void ClearGame(string title)
    {
        if (cleared) return;
        cleared = true;
        running = false;

        SetLocomotionEnabled(false);
        if (locomotionRoot != null) locomotionRoot.SetActive(false);

        float t = GetElapsed();

        if (clearPanel) clearPanel.SetActive(true);
        if (clearTitleText) clearTitleText.text = title;
        if (clearTimeText)
        {
            int min = Mathf.FloorToInt(t / 60f);
            float sec = t - min * 60f;
            clearTimeText.text = $"걸린 시간: {min:00}:{sec:00.00}";
        }
    }

    void SetLocomotionEnabled(bool enabled)
    {
        if (locomotionToDisable == null) return;
        for (int i = 0; i < locomotionToDisable.Length; i++)
        {
            if (locomotionToDisable[i] != null)
                locomotionToDisable[i].enabled = enabled;
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
