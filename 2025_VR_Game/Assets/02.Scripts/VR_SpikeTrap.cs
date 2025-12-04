using System.Collections;
using UnityEngine;

public class VR_SpikeTrap : MonoBehaviour
{
    public Transform spikes;
    public Transform spikeStartPos;
    public Transform spikeUpPos;

    public float riseTime = 0.3f;
    public float stayTime = 0.6f;
    public float downTime = 0.3f;
    public float intervalTime = 1.0f;

    public int damage = 20;
    public string playerTag = "Player";

    private bool isUp;

    private void Start()
    {
        if (spikes == null)
            spikes = transform;

        if (spikeStartPos != null)
            spikes.position = spikeStartPos.position;

        StartCoroutine(TrapLoop());
    }

    private IEnumerator TrapLoop()
    {
        while (true)
        {
            yield return MoveSpike(spikeStartPos.position, spikeUpPos.position, riseTime);
            isUp = true;

            yield return new WaitForSeconds(stayTime);

            isUp = false;
            yield return MoveSpike(spikeUpPos.position, spikeStartPos.position, downTime);

            yield return new WaitForSeconds(intervalTime);
        }
    }

    private IEnumerator MoveSpike(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        float inv = 1f / Mathf.Max(duration, 0.0001f);

        while (t < 1f)
        {
            t += Time.deltaTime * inv;
            spikes.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        spikes.position = to;
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyDamage(other);
    }

    private void ApplyDamage(Collider other)
    {
        if (!isUp)
            return;

        if (!other.CompareTag(playerTag))
            return;

        var health = other.GetComponentInParent<PlayerHealth>();
        if (health == null)
            return;

        health.TakeDamage(damage);
    }
}
