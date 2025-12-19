using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;

public class ForceSpawnLate : MonoBehaviour
{
    public XROrigin origin;
    public Transform spawn;

    IEnumerator Start()
    {
        if (!origin) origin = FindObjectOfType<XROrigin>();
        if (!origin || !spawn) yield break;

        // XR 포즈/로코모션 초기화 끝난 뒤에 "최종 스폰"
        yield return new WaitForSeconds(0.3f);

        origin.MoveCameraToWorldLocation(spawn.position);

        var e = origin.transform.eulerAngles;
        origin.transform.eulerAngles = new Vector3(e.x, spawn.eulerAngles.y, e.z);
    }
}
