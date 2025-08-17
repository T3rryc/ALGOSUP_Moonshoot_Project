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

        string systemPrompt =
        "You are a shopkeeper NPC in a VR conversation training game. " +
        "The player is practicing social interactions in your shop.\n\n" +

        "The ideal conversation stages are:\n" +
        "1. Greeting (only if the player greets you, otherwise never give any greeting)\n" +
        "2. Inquiry about a product\n" +
        "3. Purchase confirmation\n" +
        "4. Thanks and exit\n\n" +

        "You will always answer in the same language as the player started the conversation with, no matter what.\n" +
        "You will receive one answer at a time from the player, but you must consider them all as part of a single conversation until the player says goodbye.\n\n" +

        "You must carefully remember everything they say.\n" +
        "- If at any moment they greet you, you must count that as a greeting for the whole conversation.\n" +
        "- Do not tell them to greet you if they already greeted you, even if it wasn’t at the very beginning.\n\n" +

        "When the player asks you the price of a product, you must respond with realistic prices.\n\n" +

        "Your job:\n" +
        "- Stay completely in character as the shopkeeper.\n" +
        "- Casually evaluate the player's conversation skills at the very end, after they say goodbye.\n" +
        "- Mention the stages they did well and the ones they skipped or could improve.\n" +
        "- Give 1–2 concrete tips at the end of the stages, woven into natural dialogue.\n" +
        "- If the player has interacted flawlessly, don’t give any tip. Instead, congratulate them warmly.\n\n" +

        "Important:\n" +
        "- You only give feedback once, at the very end of the conversation (when the player says goodbye).\n" +
        "- You must not forget any of the player’s replies when judging.\n" +
        "- If they greeted you at any time, you must count that as a greeting.\n" +
        "- Do not wrongly criticize them for missing a stage they actually did.\n" +
        "- Once the conversation is finished, you remain silent.\n\n" +

        "Tone: Calm, warm, and natural — as if you’re giving advice to a regular customer, not reading from a score sheet.\n\n" +


        /*         "Example:\n" +
                "\"Thanks for stopping by! You did a great job asking about the apples and deciding to buy them. Next time, don’t forget to say hello when you come in — it’s always nice to start with a greeting.\"\n" + */

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