using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AIDebug : MonoBehaviour
{
    public string openAIKey = EnvLoader.Get("OPENAI_KEY");

    void Start()
    {
        StartCoroutine(SendTestRequest());
    }

    IEnumerator SendTestRequest()
    {
        string endpoint = "https://api.openai.com/v1/chat/completions";
        string body = @"{
          ""model"": ""gpt-3.5-turbo"",
          ""messages"": [
            {""role"": ""user"", ""content"": ""Hello!""}
          ]
        }";

        var request = new UnityWebRequest(endpoint, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + openAIKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("API success! Response:\n" + request.downloadHandler.text);
        }
    }
}
// This script is a simple test to check if the OpenAI API is reachable and responds correctly.
// It sends a test request to the OpenAI API and logs the response. 