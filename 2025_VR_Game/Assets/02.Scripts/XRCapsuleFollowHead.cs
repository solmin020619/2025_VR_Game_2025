using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class XRCapsuleFollowHead : MonoBehaviour
{
    public Transform head;                 // Main Camera ³Ö±â
    public float skin = 0.02f;             // º® ÆÄ°íµê ¹æÁö ¿©À¯
    public float minHeight = 1.0f;
    public float maxHeight = 2.0f;

    CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (head == null && Camera.main != null) head = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (head == null) return;

        // HMD ³ôÀÌ¿¡ ¸ÂÃç Ä¸½¶ ³ôÀÌ Á¶Á¤
        float h = Mathf.Clamp(head.localPosition.y, minHeight, maxHeight);
        cc.height = h;

        // Ä¸½¶ Áß½ÉÀ» Ä«¸Þ¶ó XZ¿¡ ¸ÂÃã (Y´Â Ä¸½¶ Áß¾Ó)
        Vector3 c = cc.center;
        c.x = head.localPosition.x;
        c.z = head.localPosition.z;
        c.y = cc.height / 2f + skin;
        cc.center = c;
    }
}
