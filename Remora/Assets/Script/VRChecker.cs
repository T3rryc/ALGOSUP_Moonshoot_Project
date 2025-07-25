using UnityEngine;
using UnityEngine.XR;

public class VRChecker : MonoBehaviour
{
    public GameObject vrRig;
    public GameObject nonVrRig;

    void Start()
    {
        if (XRSettings.isDeviceActive)
        {
            // Headset is connected
            vrRig.SetActive(true);
            nonVrRig.SetActive(false);
        }
        else
        {
            // No headset found
            vrRig.SetActive(false);
            nonVrRig.SetActive(true);
        }
    }
}
