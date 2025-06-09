using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using System;

public class MicRecorder : MonoBehaviour
{
    public string apiKey = EnvLoader.Get("OPENAI_KEY");
    public DialogueManager dialogueManager;

    private AudioClip recordedClip;
    private string filePath;

    public void StartRecording()
    {
        recordedClip = Microphone.Start(null, false, 5, 44100);
        Debug.Log("🎙️ Started recording...");
    }

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
