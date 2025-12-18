using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    [HideInInspector] public int currentHP;
    public bool isDead { get; private set; }

    public float invincibleTime = 0.3f;
    private float lastHitTime = -999f;

    public Text hpText;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float respawnDelay = 0.2f;
    public float respawnInvincible = 1.0f;

    XROrigin xrOrigin;
    CharacterController cc;
    LocomotionProvider[] locomotionProviders;

    bool respawning;

    void Awake()
    {
        // ✅ 어디에 붙어있든 XROrigin을 부모에서 찾음
        xrOrigin = GetComponentInParent<XROrigin>();
        if (xrOrigin == null) xrOrigin = FindObjectOfType<XROrigin>();

        // ✅ 캐릭터컨트롤러도 XROrigin쪽에서 찾는게 안전
        if (xrOrigin != null)
        {
            cc = xrOrigin.GetComponent<CharacterController>();
            if (cc == null) cc = xrOrigin.GetComponentInChildren<CharacterController>(true);
        }
        else
        {
            cc = GetComponentInParent<CharacterController>();
        }

        // ✅ 이동/회전/텔포 같은 로코모션 provider들 (MoveProvider/TurnProvider 등)
        locomotionProviders = FindObjectsOfType<LocomotionProvider>(true);
    }

    void Start()
    {
        currentHP = maxHP;
        isDead = false;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead || respawning) return;
        if (Time.time - lastHitTime < invincibleTime) return;

        lastHitTime = Time.time;

        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        Debug.Log("Damage " + amount + " → HP: " + currentHP);
        UpdateUI();

        if (currentHP <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player Dead");
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        respawning = true;

        yield return new WaitForSeconds(respawnDelay);

        // 1) 로코모션 잠깐 끄기 (리스폰 직후 다시 밀리는 현상 방지)
        SetLocomotionProviders(false);

        // 2) CC 잠깐 끄기 (충돌 튕김 방지)
        if (cc) cc.enabled = false;

        // 3) 리스폰 이동 (XROrigin을 "카메라 기준"으로 이동)
        if (respawnPoint != null && xrOrigin != null)
        {
            xrOrigin.MoveCameraToWorldLocation(respawnPoint.position);

            // 방향도 맞추고 싶으면 Y만 맞추기
            Vector3 e = xrOrigin.transform.eulerAngles;
            xrOrigin.transform.rotation = Quaternion.Euler(0f, respawnPoint.eulerAngles.y, 0f);
        }
        else if (respawnPoint != null)
        {
            // 혹시 xrOrigin 못찾았을 때 안전장치
            transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("[PlayerHealth] respawnPoint가 연결 안 됨.");
        }

        yield return null; // 1프레임 안정화

        // 4) 다시 켜기
        if (cc) cc.enabled = true;
        SetLocomotionProviders(true);

        // 5) HP 초기화
        currentHP = maxHP;
        UpdateUI();

        // 6) 리스폰 직후 무적
        lastHitTime = Time.time;
        yield return new WaitForSeconds(respawnInvincible);

        isDead = false;
        respawning = false;
    }

    void SetLocomotionProviders(bool enabled)
    {
        if (locomotionProviders == null) return;
        foreach (var p in locomotionProviders)
        {
            if (p == null) continue;
            p.enabled = enabled;
        }
    }

    void UpdateUI()
    {
        if (hpText != null) hpText.text = "HP: " + currentHP;
    }
}
