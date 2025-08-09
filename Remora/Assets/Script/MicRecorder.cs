using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using System;
using UnityEngine.XR; // For VR detection
using System.Linq;    // For FirstOrDefault


public class MicRecorder : MonoBehaviour
{
    public string apiKey = EnvLoader.Get("OPENAI_KEY");
    public DialogueManager dialogueManager;

    private AudioClip recordedClip;
    private string filePath;
    private string selectedMicDevice;


    void Start()
    {
        var devices = Microphone.devices;
        Debug.Log("🎙️ Available Mics:");
        foreach (var d in devices)
        {
            Debug.Log(" - " + d);
        }
    }

   // press button to start recording
   public void StartRecording()
    {
        string[] mics = Microphone.devices;
        bool isVrActive = XRSettings.isDeviceActive;
        Debug.Log("🎮 VR Headset Active? " + isVrActive);

        foreach (var mic in mics)
        {
            Debug.Log("Detected Mic: " + mic);
        }

        if (isVrActive)
        {
            // Look for a VR mic
            selectedMicDevice = mics.FirstOrDefault(m => m.ToLower().Contains("oculus") || m.ToLower().Contains("vive") || m.ToLower().Contains("vr"));
            if (selectedMicDevice == null && mics.Length > 0)
            {
                selectedMicDevice = mics[0]; // fallback
                Debug.LogWarning("⚠️ VR mic not found, using default mic: " + selectedMicDevice);
            }
        }
        else
        {
            // Use laptop/internal mic
            selectedMicDevice = mics.FirstOrDefault(m => m.ToLower().Contains("realtek") || m.ToLower().Contains("internal"));
            if (selectedMicDevice == null && mics.Length > 0)
            {
                selectedMicDevice = mics[0]; // fallback
                Debug.LogWarning("⚠️ No internal mic found, using default mic: " + selectedMicDevice);
            }
        }

        if (selectedMicDevice != null)
        {
            recordedClip = Microphone.Start(selectedMicDevice, false, 5, 44100);
            Debug.Log($"🎙️ Started recording on: {selectedMicDevice}");
        }
        else
        {
            Debug.LogError("❌ No microphone available!");
        }
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // press the button to start recording, wait silence and send the record to the API
    public void StartRecordingWithAutoStop()
    {
        string[] mics = Microphone.devices;
        selectedMicDevice = mics.Length > 0 ? mics[0] : null;

        if (selectedMicDevice == null)
        {
            Debug.LogError("❌ No microphone found.");
            return;
        }

        recordedClip = Microphone.Start(selectedMicDevice, true, 30, 44100);
        Debug.Log($"🎙️ Started recording on: {selectedMicDevice}");

        StartCoroutine(WaitForSilence());
    }

        private IEnumerator WaitForSilence()
    {
        float silenceThreshold = 0.01f; // Adjust based on mic sensitivity
        float silenceDuration = 1.0f;   // Seconds of silence before stopping
        float silentTime = 0f;

        int sampleWindow = 1024;
        float[] samples = new float[sampleWindow];

        while (true)
        {
            int micPos = Microphone.GetPosition(selectedMicDevice);

            // Avoid invalid sample positions
            if (micPos < sampleWindow || recordedClip == null)
            {
                yield return null;
                continue;
            }

            int startPos = Mathf.Clamp(micPos - sampleWindow, 0, recordedClip.samples - sampleWindow);
            recordedClip.GetData(samples, startPos);

            float maxVolume = 0f;
            foreach (var sample in samples)
            {
                float abs = Mathf.Abs(sample);
                if (abs > maxVolume) maxVolume = abs;
            }

            if (maxVolume < silenceThreshold)
            {
                silentTime += Time.deltaTime;
                if (silentTime >= silenceDuration)
                {
                    break; // Silence long enough — stop recording
                }
            }
            else
            {
                silentTime = 0f; // Reset if sound detected
            }

            yield return null;
        }

        StopRecordingAndSend();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // press button 2 to send the record to the API
    public void StopRecordingAndSend()
    {
        if (!Microphone.IsRecording(null))
        {
            Debug.LogWarning("Mic was not recording.");
            return;
        }

        Microphone.End(null);
        Debug.Log("🛑 Stopped recording. Saving WAV...");

        filePath = Path.Combine(Application.persistentDataPath, "recorded.wav");
        SaveWav(filePath, recordedClip);

        StartCoroutine(SendToWhisper(filePath));
    }

    private IEnumerator SendToWhisper(string path)
    {
        byte[] audioData = File.ReadAllBytes(path);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "recording.wav", "audio/wav");
        form.AddField("model", "whisper-1");

        UnityWebRequest www = UnityWebRequest.Post("https://api.openai.com/v1/audio/transcriptions", form);
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Whisper Error: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
        Debug.Log("📜 Whisper returned: " + json);

        WhisperResponse parsed = JsonUtility.FromJson<WhisperResponse>(json);
        if (!string.IsNullOrEmpty(parsed.text))
        {
            dialogueManager.SendUserSpeech(parsed.text); // This calls your AI pipeline
        }
    }

    [Serializable]
    public class WhisperResponse
    {
        public string text;
    }

    // WAV utility function (minimal implementation)
    void SaveWav(string filePath, AudioClip clip)
    {
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);

        byte[] wav = WavUtility.FromAudioClip(clip, samples, clip.channels, clip.frequency);
        File.WriteAllBytes(filePath, wav);
        Debug.Log("✅ WAV saved: " + filePath);
    }
}
