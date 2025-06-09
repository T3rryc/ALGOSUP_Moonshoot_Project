using UnityEngine;

public class MicrophoneCapture : MonoBehaviour
{
    public AudioSource audioSource;
    private string microphoneDevice;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0]; // You can loop through to find the right device
            Debug.Log("Using Microphone: " + microphoneDevice);

            audioSource.clip = Microphone.Start(microphoneDevice, true, 10, 44100);
            audioSource.loop = true;

            // Wait until the microphone has started recording
            while (!(Microphone.GetPosition(microphoneDevice) > 0)) { }

            audioSource.Play();
        }
        else
        {
            Debug.LogError("No microphone detected.");
        }
    }
}

