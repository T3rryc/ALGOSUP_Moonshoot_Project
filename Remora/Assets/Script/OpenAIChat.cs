using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // for scene detection

public class OpenAIChat : MonoBehaviour
{
    public string openAIKey = EnvLoader.Get("OPENAI_KEY");
    public string userPrompt = "Hello!";
    public string latestResponse;

    public void Ask(string input)
    {
        userPrompt = input;
        StartCoroutine(SendRequest());
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class MessageWrapper
    {
        public string model = "gpt-4.1";
        public Message[] messages;
    }

    private string Escape(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
    }

    IEnumerator SendRequest()
    {
        string endpoint = "https://api.openai.com/v1/chat/completions";

        // Choose prompt according to scene
        string sceneName = SceneManager.GetActiveScene().name;
        string systemPrompt = PromptLibrary.GetPrompt(sceneName);

        string jsonBody = @"{
            ""model"": ""gpt-4.1"",
            ""messages"": [
                {""role"": ""system"", ""content"": """ + Escape(systemPrompt) + @"""},
                {""role"": ""user"", ""content"": """ + Escape(userPrompt) + @"""}
            ]
        }";

        using UnityWebRequest request = new UnityWebRequest(endpoint, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + openAIKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            var result = JsonUtility.FromJson<ChatGPTResponse>(json);
            latestResponse = result.choices[0].message.content.Trim();
            Debug.Log("GPT: " + latestResponse);
        }
        else
        {
            Debug.LogError("OpenAI API Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class ChatGPTResponse
    {
        public Choice[] choices;

        [System.Serializable]
        public class Choice
        {
            public Message message;
        }

        [System.Serializable]
        public class Message
        {
            public string role;
            public string content;
        }
    }
}
