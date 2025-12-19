using UnityEngine;

public class TrackPositionChange : MonoBehaviour
{
    Vector3 last;
    void Start() => last = transform.position;

    void LateUpdate()
    {
        if ((transform.position - last).sqrMagnitude > 0.0001f)
        {
            Debug.Log($"[MOVED] {name} -> {transform.position}", this);
            last = transform.position;
        }
    }
}
