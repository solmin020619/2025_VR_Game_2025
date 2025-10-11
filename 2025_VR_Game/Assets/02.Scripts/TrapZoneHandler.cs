using UnityEngine;

public class TrapZoneHandler : MonoBehaviour
{
    public VR_SpikeTrap parentTrap;

    private void OnTriggerEnter(Collider other)
    {
        if (parentTrap != null)
        {
            Debug.Log("[TrapZone] Trigger detected: " + other.name);
            parentTrap.TriggerActivated(other);
        }
    }
}
