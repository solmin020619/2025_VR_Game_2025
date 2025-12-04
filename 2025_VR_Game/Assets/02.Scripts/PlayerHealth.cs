using UnityEngine;
using UnityEngine.UI; // ★ 1. 이게 추가됨 (글자 제어용)
using UnityEngine.SceneManagement; // ★ 2. 이게 추가됨 (죽으면 재시작)

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    [HideInInspector] public int currentHP;
    public bool isDead { get; private set; }

    public float invincibleTime = 0.3f;
    private float lastHitTime = -999f;

    // ★ 3. 인스펙터에서 텍스트를 연결할 구멍을 뚫어줌
    public Text hpText;

    void Start()
    {
        currentHP = maxHP;
        isDead = false;
        UpdateUI(); // 시작하자마자 HP: 100 띄우기
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (Time.time - lastHitTime < invincibleTime) return;
        lastHitTime = Time.time;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log("Damage " + amount + " → HP: " + currentHP);

        // ★ 4. 맞을 때마다 화면 글씨 갱신!
        UpdateUI();

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player Dead");

        // ★ 5. 죽으면 현재 씬 재시작 (루프)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ★ 6. 화면 글씨 바꾸는 함수
    void UpdateUI()
    {
        if (hpText != null)
        {
            hpText.text = "HP: " + currentHP;
        }
    }
}