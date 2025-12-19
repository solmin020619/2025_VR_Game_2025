using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR;

public class XROriginSpawn : MonoBehaviour
{
    public XROrigin origin;
    public Transform spawnPoint;
    public bool recenter = true;

    IEnumerator Start()
    {
        if (!origin) origin = FindObjectOfType<XROrigin>();
        if (!origin || !spawnPoint) yield break;

        // XR 서브시스템/트래킹 포즈 적용될 시간 한 프레임~약간 대기
        yield return null;
        yield return new WaitForEndOfFrame();

        // (선택) 리센터 - Quest에서 방향/원점 튐 줄이는 데 도움
        if (recenter)
        {
            var subs = new System.Collections.Generic.List<XRInputSubsystem>();
            SubsystemManager.GetInstances(subs);
            foreach (var s in subs) s.TryRecenter();
        }

        // 핵심: 카메라(머리)가 spawnPoint에 오도록 월드 기준 이동
        origin.MoveCameraToWorldLocation(spawnPoint.position);

        // 회전도 맞추고 싶으면(원하면)
        Vector3 e = origin.transform.eulerAngles;
        origin.transform.eulerAngles = new Vector3(e.x, spawnPoint.eulerAngles.y, e.z);
    }
}
