using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float upHeight = 2f;
    public float speed = 2f;
    private Vector3 startPos;
    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(TrapRoutine());
    }

    IEnumerator TrapRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            isActive = !isActive;
            Vector3 target = startPos + (isActive ? Vector3.up * upHeight : Vector3.zero);
            float t = 0f;
            Vector3 initial = transform.position;
            while (t < 1f)
            {
                transform.position = Vector3.Lerp(initial, target, t);
                t += Time.deltaTime * speed;
                yield return null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
