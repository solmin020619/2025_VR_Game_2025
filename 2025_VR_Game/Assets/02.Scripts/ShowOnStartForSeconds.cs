using System.Collections;
using UnityEngine;

public class ShowOnStartForSeconds : MonoBehaviour
{
    public float showSeconds = 5f;
    public bool hideOnStart = false; // ¾À ·Îµå Á÷ÈÄ Àá±ñ ¼û°å´Ù°¡ ÄÓÁö(¿øÇÏ¸é ON)

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        if (hideOnStart)
        {
            gameObject.SetActive(false);
            yield return null;
            gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(showSeconds);
        gameObject.SetActive(false);
    }
}
