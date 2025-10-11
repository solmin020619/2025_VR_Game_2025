using UnityEngine;
using System.Collections;

public class SpikeTrapDmg : MonoBehaviour
{
    [Header("움직임 설정")]
    public float upHeight = 2f;        // 위로 솟는 높이
    public float speed = 2f;           // 움직이는 속도
    public float delayTime = 2f;       // 대기 시간

    [Header("데미지 설정")]
    public int damage = 20;            // 입히는 데미지
    public float damageCooldown = 0.5f; // 연속 데미지 방지 시간

    private Vector3 startPos;
    private bool isUp = false;
    private float lastHitTime = -999f;

    void Start()
    {
        startPos = transform.position;
        StartCoroutine(TrapRoutine());
    }

    IEnumerator TrapRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayTime);
            isUp = !isUp;

            Vector3 target = startPos + (isUp ? Vector3.up * upHeight : Vector3.zero);
            Vector3 init = transform.position;
            float t = 0;

            while (t < 1)
            {
                transform.position = Vector3.Lerp(init, target, t);
                t += Time.deltaTime * speed;
                yield return null;
            }

            transform.position = target;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어에 PlayerHealth 스크립트가 붙어있는지 확인
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            TryDamage(player);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            TryDamage(player);
        }
    }

    private void TryDamage(PlayerHealth player)
    {
        if (Time.time - lastHitTime >= damageCooldown)
        {
            player.TakeDamage(damage);
            lastHitTime = Time.time;
            Debug.Log("Spike Trap hit! -" + damage + " HP");
        }
    }
}
