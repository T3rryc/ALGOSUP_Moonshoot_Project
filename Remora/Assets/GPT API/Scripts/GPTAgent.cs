using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Threading;
using System.Linq;
using System;

namespace TzarGPT
{
    public class GPTAgent : MonoBehaviour
    {
        /// <summary>
        /// Singleton to use if you have only one instance of GPTAgent
        /// </summary>
        public static GPTAgent Instance;

        [Tooltip("Replace this with your API key, get one at https://openai.com")]
        [SerializeField] string apiKey = "YOUR_API_KEY";
        public string API_KEY { get; private set; }
        [Tooltip("The URL endpoint to query, usually you'll need to query the completions endpoint. More info https://platform.openai.com/docs/introduction" +
            "\n\n AI Models: https://api.openai.com/v1/completions" +
            "\n ChatGPT Models: https://api.openai.com/v1/chat/completions")]
        [SerializeField] string API_URL = "https://api.openai.com/v1/completions";

        [Header("Prompt Settings")]
        [Tooltip("How many prompts should the prompt builder remember")]
        [SerializeField] int historyLength = 20;

        [Tooltip("Recorded prompts in this session. Use ClearHistory() to clear")]
        List<string> recordedPrompts = new List<string>();

        [Header("Chat AI Profile")]
        [Tooltip("When this is enabled the prompt history will be cleared and then re-initialized when changing the profile during runtime")]
        [SerializeField] bool updatePromptOnProfileChange = true;
        ChatProfile prevAiProfile;
        [SerializeField] ChatProfile aiProfile;
        readonly StringBuilder strBuilder = new StringBuilder();

        [TextAreaAttribute(1, 50)]
        [SerializeField] string fullPromptVisualizer = string.Empty;

        [System.Serializable]
        public class ProfileChangedEvent : UnityEvent<ChatProfile> { }
        [SerializeField] ProfileChangedEvent onProfileChange;

        // Public fields
        public ChatProfile AIProfile { get { return aiProfile; } }
        public string FullPromptVisualizer { get { return fullPromptVisualizer; } }
        public bool IsChatModel { get; private set; }


        private void Awake()
        {
            Instance = this;

            API_KEY = apiKey;

            prevAiProfile = aiProfile;

            // set isChatModel
            if (aiProfile.model.Contains("gpt"))
            {
                IsChatModel = true;
            }

            AddInitialPrompt();

            if (updatePromptOnProfileChange)
                StartCoroutine(nameof(CheckProfileChange));
        }

        #region AI Models
        /// <summary>
        /// Get an object response from the AI model
        /// </summary>
        /// <param name="input">The prompt to send</param>
        /// <returns>The whole object response. You can see the response format here https://platform.openai.com/docs/api-reference/making-requests </returns>
        public async Task<AIResponse> GetAIResponseObject(string input)
        {
            // Go through prompt builder
            string prompt = GetPrompt(input);

            var request = new UnityWebRequest(API_URL, "POST");

            // Clear the stringBuilder
            strBuilder.Clear();

            strBuilder.Append("{");
            strBuilder.AppendFormat("\"prompt\": \"{0}\",", prompt.Replace("\"", "\\\""));
            strBuilder.AppendFormat("\"model\": \"{0}\",", aiProfile.model);
            strBuilder.AppendFormat("\"max_tokens\": {0},", aiProfile.maxTokens);
            strBuilder.AppendFormat("\"temperature\": {0},", aiProfile.temperature);
            strBuilder.AppendFormat("\"top_p\": {0},", aiProfile.topP);
            strBuilder.AppendFormat("\"frequency_penalty\": {0},", aiProfile.frequencyPenalty);
            strBuilder.AppendFormat("\"presence_penalty\": {0},", aiProfile.presencePenalty);
            strBuilder.AppendFormat("\"best_of\": {0}", aiProfile.bestOf);
            if (aiProfile.stopSequences.Length > 0)
                strBuilder.AppendFormat(",\"stop\": {0}", JsonConvert.SerializeObject(aiProfile.stopSequences));
            strBuilder.Append("}");

            // Get the final JSON string
            string formattedJSON = strBuilder.ToString();
            // Replace newlines
            formattedJSON = formattedJSON.Replace("\n", "\\n");

            //Debug.Log("Replaced N Json:\n"+ formattedJSON);

            byte[] jsonToSend = new UTF8Encoding().GetBytes(formattedJSON);

            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + API_KEY);

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                return null;
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                AIResponse aiResponse = JsonConvert.DeserializeObject<AIResponse>(jsonResponse);
                // Add the response to the prompt builder memory
                AddPrompt(aiResponse.choices[0].text);
                return aiResponse;
            }
        }

        /// <summary>
        /// Get a string response from the AI model
        /// </summary>
        /// <param name="input">The prompt to send</param>
        /// <returns>Always the first response of the query</returns>
        public async Task<string> GetAIResponse(string input)
        {
            AIResponse aiResponse = await GetAIResponseObject(input);
            return aiResponse.choices[0].text;
        }
        #endregion

        #region GPT Models
        /// <summary>
        /// Get an object response from the AI GPT models
        /// </summary>
        /// <param name="input">The prompt to send</param>
        /// <returns>The whole object response. You can see the response format here https://platform.openai.com/docs/api-reference/making-requests </returns>
        public async Task<ChatResponse> GetChatResponseObject(string input, ChatRole chatRole)
        {
            // Go through prompt builder
            string prompt = GetPrompt(input, chatRole);

            var request = new UnityWebRequest(API_URL, "POST");

            // Clear the stringBuilder
            strBuilder.Clear();

            strBuilder.Append("{");
            strBuilder.AppendFormat("\"messages\": {0},", prompt); //.Replace("\"\"", "\"")
            strBuilder.AppendFormat("\"model\": \"{0}\",", aiProfile.model);
            strBuilder.AppendFormat("\"max_tokens\": {0},", aiProfile.maxTokens);
            strBuilder.AppendFormat("\"temperature\": {0},", aiProfile.temperature);
            strBuilder.AppendFormat("\"top_p\": {0},", aiProfile.topP);
            strBuilder.AppendFormat("\"frequency_penalty\": {0},", aiProfile.frequencyPenalty);
            strBuilder.AppendFormat("\"presence_penalty\": {0}", aiProfile.presencePenalty);
            //strBuilder.AppendFormat("\"best_of\": {0}", aiProfile.bestOf);
            if (aiProfile.stopSequences.Length > 0)
                strBuilder.AppendFormat(",\"stop\": {0}", JsonConvert.SerializeObject(aiProfile.stopSequences));
            strBuilder.Append("}");

            // Get the final JSON string
            string formattedJSON = strBuilder.ToString();
            //// Replace newlines
            //formattedJSON = formattedJSON.Replace("\n", "\\n");

            //Debug.Log("String builder result: ");
            //Debug.Log(formattedJSON);

            byte[] jsonToSend = new UTF8Encoding().GetBytes(formattedJSON);

            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + API_KEY);

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text);
                Debug.LogError(request.error);
                Debug.LogError(errorResponse.error.message);
                return null;
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                ChatResponse aiResponse = JsonConvert.DeserializeObject<ChatResponse>(jsonResponse);
                // Add the response to the prompt builder memory
                AddPrompt(JsonConvert.SerializeObject(aiResponse.choices[0].message));
                return aiResponse;
            }
        }

        /// <summary>
        /// Get only the response content without the role
        /// </summary>
        /// <param name="input">The input text</param>
        /// <param name="chatRole">The Chat Role you are sending it as</param>
        /// <returns>Always the first content response of the query</returns>
        public async Task<string> GetChatResponse(string input, ChatRole chatRole)
        {
            ChatResponse aiResponse = await GetChatResponseObject(input, chatRole);
            return aiResponse.choices[0].message.content;
        }

        public delegate void StreamChunksHandler(string chunkData);
        public static event StreamChunksHandler OnStreamChunkReceived;
        public string streamedText;

        public async Task GetChatResponseStream(string input, ChatRole chatRole)
        {
            // Go through prompt builder
            string prompt = GetPrompt(input, chatRole);

            // Clear the stringBuilder
            strBuilder.Clear();

            strBuilder.Append("{");
            strBuilder.AppendFormat("\"messages\": {0},", prompt); //.Replace("\"\"", "\"")
            strBuilder.AppendFormat("\"model\": \"{0}\",", aiProfile.model);
            strBuilder.AppendFormat("\"max_tokens\": {0},", aiProfile.maxTokens);
            strBuilder.AppendFormat("\"temperature\": {0},", aiProfile.temperature);
            strBuilder.AppendFormat("\"top_p\": {0},", aiProfile.topP);
            strBuilder.AppendFormat("\"frequency_penalty\": {0},", aiProfile.frequencyPenalty);
            strBuilder.AppendFormat("\"presence_penalty\": {0},", aiProfile.presencePenalty);
            strBuilder.AppendFormat("\"stream\": true");
            //strBuilder.AppendFormat("\"best_of\": {0}", aiProfile.bestOf);
            if (aiProfile.stopSequences.Length > 0)
                strBuilder.AppendFormat(",\"stop\": {0}", JsonConvert.SerializeObject(aiProfile.stopSequences));
            strBuilder.Append("}");

            // Get the final JSON string
            string formattedJSON = strBuilder.ToString();
            //Debug.Log(formattedJSON);

            byte[] payload = Encoding.UTF8.GetBytes(formattedJSON);

            using (var request = new UnityWebRequest(API_URL, UnityWebRequest.kHttpVerbPOST))
            {
                UploadHandlerRaw uploadHandler = new UploadHandlerRaw(payload);
                uploadHandler.contentType = "application/json";
                request.uploadHandler = uploadHandler;
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Authorization", "Bearer " + API_KEY);

                var asyncOperation = request.SendWebRequest();

                do
                {
                    List<string> dataList = new List<string>();
                    string responseText = request.downloadHandler.text;
                    //responseText = "{" + responseText + "}";
                    //Debug.Log(responseText);

                    string[] lines = responseText.Split('\n').Where(line => line != "").ToArray();

                    foreach (string line in lines)
                    {
                        var value = "{" + line + "}";

                        //if (value.Contains("[DONE]"))
                        //{
                        //    // Invoke the complete callback
                        //    // onComplete?.Invoke();
                        //    break;
                        //}

                        try
                        {
                            ChatStreamResponse csr = JsonConvert.DeserializeObject<ChatStreamResponse>(value);
                            if (csr != null && csr.data.choices.Count > 0)
                            {
                                string content = csr.data.choices[0].delta.content;
                                dataList.Add(content);

                                //if (!dataList.Contains(content))
                                //{
                                //    OnStreamChunkReceived?.Invoke(content);
                                //    dataList.Add(csr);
                                //}
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("Error deserializing JSON: " + ex.Message);
                        }
                    }

                    // return the current joined string
                    OnStreamChunkReceived?.Invoke(string.Join("", dataList));

                    // Invoke the response callback
                    // onResponse?.Invoke(dataList);

                    await Task.Yield();
                }
                while (!asyncOperation.isDone);

                Debug.Log("Stream Ended");
                // Invoke the complete callback
                // onComplete?.Invoke();
            }
        }

        /// <summary>
        /// Get the chat response in a string[2] format which contains the role
        /// </summary>
        /// <param name="input"></param>
        /// <param name="chatRole"></param>
        /// <returns>A string array with length of 2 where index 0 is the role and index 1 is the response message content</returns>
        public async Task<string[]> GetChatResponseWithRole(string input, ChatRole chatRole)
        {
            ChatResponse aiResponse = await GetChatResponseObject(input, chatRole);

            string[] result = new string[2];
            result[0] = aiResponse.choices[0].message.role;
            result[1] = aiResponse.choices[0].message.content;

            return result;
        }
        #endregion

        #region Prompt Builder
        void AddInitialPrompt()
        {
            // Add the initial prompt if we have it
            if (aiProfile.initialPrompt.Length != 0)
            {
                if (IsChatModel)
                    AddPrompt(FormatPrompt(aiProfile.initialPrompt, ChatRole.System));
                else
                    AddPrompt(aiProfile.initialPrompt);
            }
        }

        // Chat message request sample
        // Role can be System, User, Assistant
        // messages=[
        //{"role": "system", "content": "You are a helpful assistant."},
        //{"role": "user", "content": "Who won the world series in 2020?"},
        //{ "role": "assistant", "content": "The Los Angeles Dodgers won the World Series in 2020."},
        //{ "role": "user", "content": "Where was it played?"}
        //]

        /// <summary>
        /// Add a pre-serialized json prompt like this: {"role": "system", "content": "You are a helpful assistant."}
        /// If you'd like to serialize a string to a prompt use the FormatPrompt() function
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="chatRole"></param>
        public void AddPrompt(string prompt, ChatRole chatRole = ChatRole.None)
        {
            if (recordedPrompts.Count > historyLength)
            {
                recordedPrompts.RemoveAt(0);
            }

            fullPromptVisualizer += prompt;
            recordedPrompts.Add(prompt);
        }

        /// <summary>
        /// Returns a formatted json prompt ready to be used with OpenAI api
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="chatRole"></param>
        /// <returns></returns>
        string FormatPrompt(string prompt, ChatRole chatRole)
        {
            // create a new object with the given properties
            var obj = new
            {
                role = chatRole.ToString().ToLowerInvariant(),
                content = prompt
            };

            // convert the object to a JSON string with string escape handling
            string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            });

            return jsonString;
        }

        string GetPrompt(string newPrompt)
        {
            if(newPrompt.Length > 0)
                AddPrompt(newPrompt);

            string result = string.Empty;

            // Build the whole string
            foreach (string prompt in recordedPrompts)
            {
                result += prompt;
            }

            //Debug.Log("Prompt Built: ");
            //Debug.Log(result);

            return result;
        }

        string GetPrompt(string newPrompt, ChatRole chatRole)
        {
            if(chatRole == ChatRole.None)
            {
                Debug.LogError("Chat Role must be set when creating GPT 3 requests!");
                return string.Empty;
            }

            // create a new object with the given properties
            var obj = new
            {
                role = chatRole.ToString().ToLowerInvariant(),
                content = newPrompt
            };

            // convert the object to a JSON string with string escape handling
            string jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            });

            // Add the json string after creating it
            if (newPrompt.Length > 0)
                AddPrompt(jsonString, chatRole);

            string result = "[";

            // Build the whole string
            foreach (string prompt in recordedPrompts)
            {
                result += $"{prompt},"; // "{" + prompt + "},";
            }

            result = result.Remove(result.Length - 1, 1);
            result += "]";

            //Debug.Log("Prompt Built: ");
            //Debug.Log(result);

            return result;
        }
        /// <summary>
        /// Clears prompt history
        /// </summary>
        /// <param name="reAddInitialPrompt">Should the initial prompt be added as first string after clearing?</param>
        public void ClearHistory(bool reAddInitialPrompt = false)
        {
            fullPromptVisualizer = string.Empty;
            recordedPrompts.Clear();

            if (reAddInitialPrompt)
            {
                AddInitialPrompt();
            }
        }
        #endregion

        #region Profile Changed
        IEnumerator CheckProfileChange()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);

                if (aiProfile != prevAiProfile)
                {
                    // Update prompt builder with new profile
                    ClearHistory(true);
                    onProfileChange?.Invoke(aiProfile);
                    prevAiProfile = aiProfile;
                }
            }
        }
        #endregion
    }
}