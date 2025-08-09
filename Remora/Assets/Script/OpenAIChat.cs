using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

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
        public string model = "gpt-3.5-turbo";
        public Message[] messages;
    }

        private string Escape(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
    }


    IEnumerator SendRequest()
    {
        string endpoint = "https://api.openai.com/v1/chat/completions";

        string systemPrompt =
            "You are a friendly shopkeeper in a virtual grocery store. " +
            "The player may ask you where to find things. " +
            "Here’s the store layout:\n\n" +
            "- Meat: right wall\n" +
            "- Cheese: right wall\n" +
            "- Seafood: right wall\n" +
            "- Pizza: right wall\n" +
            "- Vegetables: top-right\n" +
            "- Fruits: top-right\n" +
            "- Drinks: right wall\n" +
            "- Soap: top-right\n" +
            "- Ice cream: top-left\n" +
            "- Chips: bottom-center\n" +
            "- Cereal: bottom-center\n" +
            "- Pasta: left wall\n" +
            "- Insecticide: left wall\n" +
            "- Olive: left wall\n" +
            "- Biscuit: top-center\n" +
            "- The player enters from the bottom-left and faces you at the front.\n\n" +
            "You will answer clearly and kindly, like a human clerk.";

        MessageWrapper wrapper = new MessageWrapper();
        wrapper.messages = new Message[]
        {
            new Message { role = "system", content = systemPrompt },
            new Message { role = "user", content = userPrompt }
        };

        string jsonBody = @"{
        ""model"": ""gpt-3.5-turbo"",
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
        Debug.Log("Authorization header = Bearer " + openAIKey);
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