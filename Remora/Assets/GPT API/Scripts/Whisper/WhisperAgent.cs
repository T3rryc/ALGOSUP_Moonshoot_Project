using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace TzarGPT
{
    public class WhisperAgent : MonoBehaviour
    {
        /*
         curl --request POST \
      --url https://api.openai.com/v1/audio/transcriptions \
      --header 'Authorization: Bearer TOKEN' \
      --header 'Content-Type: multipart/form-data' \
      --form file=@/path/to/file/openai.mp3 \
      --form model=whisper-1
         */

        [SerializeField] string API_URL = "https://api.openai.com/v1/audio/transcriptions";
        [SerializeField] ChatProfile chatProfile;
        [SerializeField] GPTAgent gptAgent;

        public async Task<string> UploadAudioAsync(string audioFilePath)
        {
            // Load audio file into memory as a byte array
            byte[] audioBytes = File.ReadAllBytes(audioFilePath);

            // Create new form data
            WWWForm form = new WWWForm();
            form.AddField("model", chatProfile.model);
            form.AddField("response_format", "text");
            form.AddBinaryData("file", audioBytes, Path.GetFileName(audioFilePath), "audio/wav");

            // Create new request
            UnityWebRequest www = UnityWebRequest.Post(API_URL, form);
            www.SetRequestHeader("Authorization", $"Bearer {gptAgent.API_KEY}");

            // Send request and wait for response
            var request = www.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Delay(10);
            }

            // Check for errors
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error uploading audio: {www.error}");
                return null;
            }

            // Extract the response text from the response
            string responseText = www.downloadHandler.text;
            //Debug.Log($"Response text: {responseText}");

            return responseText;
        }
    }
}
