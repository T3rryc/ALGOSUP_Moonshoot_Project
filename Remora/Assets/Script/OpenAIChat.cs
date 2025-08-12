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
        "You are a shopkeeper NPC in a VR conversation training game. " +
        "The player is practicing social interactions in your shop.\n\n" +
        "The ideal conversation stages are:\n" +
        "1. Greeting\n" +
        "2. Inquiry about a product\n" +
        "3. Purchase confirmation\n" +
        "4. Thanks and exit\n\n" +

        "You will receive the full conversation transcript between you (the NPC) and the player.\n\n" +

        "Your job:\n" +
        "- Stay completely in character as the shopkeeper.\n" +
        "- Casually evaluate the player's conversation skills at the end, using friendly, helpful language.\n" +
        "- Mention the stages they did well and the ones they skipped or could improve.\n" +
        "- Give 1–2 concrete tips, woven into natural dialogue.\n\n" +

        "Tone: Encouraging, warm, and natural — as if you’re giving advice to a regular customer, not reading from a score sheet.\n\n" +

        "Example:\n" +
        "\"Thanks for stopping by! You did a great job asking about the apples and deciding to buy them. Next time, don’t forget to say hello when you come in — it’s always nice to start with a greeting.\"\n" +

        "Now, based on this conversation transcript:\n" +
        "{conversation_transcript}\n\n" +

        "Respond with your final NPC line.\n\n";


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