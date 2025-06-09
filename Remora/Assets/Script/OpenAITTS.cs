using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using System.Collections;

public class OpenAITTS : MonoBehaviour
{
    public string apiKey;
    public AudioSource audioSource;

    void Awake()
    {
        apiKey = EnvLoader.Get("OPENAI_KEY")?.Trim();

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("🔑 OPENAI_KEY is missing or empty!");
        }
        else
        {
            Debug.Log($"✅ OPENAI_KEY loaded, length: {apiKey.Length}");
            Debug.Log($"Auth test header: Bearer {apiKey.Substring(0, 8)}...");
        }
    }

    public void Speak(string text)
    {
        StartCoroutine(SendTTSRequest(text));
    }

    IEnumerator SendTTSRequest(string inputText)
    {
        string url = "https://api.openai.com/v1/audio/speech";

        string jsonBody = $@"
        {{
            ""model"": ""tts-1"",
            ""input"": ""{inputText}"",
            ""voice"": ""coral"",
            ""instructions"": ""Speak in a cheerful and positive tone."",
            ""response_format"": ""mp3""
        }}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("Auth test header (FULL): Bearer " + apiKey);


            // SSL bypass (dev only, like --insecure)
            request.certificateHandler = new BypassCertificate();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"TTS Request Error: {request.responseCode} - {request.error}");
                Debug.LogError("Server response: " + request.downloadHandler.text);
                Debug.Log("Auth header: Bearer " + apiKey);
                yield break;
            }

            byte[] audioData = request.downloadHandler.data;
            string path = Path.Combine(Application.persistentDataPath, "speech.mp3");
            File.WriteAllBytes(path, audioData);

            using (UnityWebRequest wwwAudio = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
            {
                yield return wwwAudio.SendWebRequest();

                if (wwwAudio.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(wwwAudio);
                    audioSource.clip = clip;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogError("Audio loading error: " + wwwAudio.error);
                }
            }
        }
    }

    class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
