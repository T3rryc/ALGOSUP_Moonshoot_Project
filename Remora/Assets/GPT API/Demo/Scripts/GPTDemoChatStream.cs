using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace TzarGPT
{
    public class GPTDemoChatStream : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] TextMeshProUGUI chatTMP;
        [SerializeField] GPTAgent gptAgent;

        private void OnEnable()
        {
            GPTAgent.OnStreamChunkReceived += GPTAgent_OnStreamChunkReceived;
        }

        private void OnDisable()
        {
            GPTAgent.OnStreamChunkReceived -= GPTAgent_OnStreamChunkReceived;
        }

        string streamingTextBase;

        private void GPTAgent_OnStreamChunkReceived(string streamedText)
        {
            chatTMP.text = streamingTextBase + streamedText;
        }

        private void Start()
        {
            inputField.ActivateInputField();
            // Display the initial prompt if any
            if(gptAgent.FullPromptVisualizer.Length > 0)
                chatTMP.text = FormatChatGPTTextWithRole(gptAgent.FullPromptVisualizer);
        }

        public void GetStreamFromAIModel(string overrideInput = "")
        {
            string prompt = overrideInput.Length == 0 ? inputField.text : overrideInput;

            // Format the input text for the text field and add it
            chatTMP.text += "\n\n" + "User: " + prompt + "\nAssistant: ";

            // Set the text base to the current text value so we can append the streamed text properly
            streamingTextBase = chatTMP.text;
            // Stream response
            _ = GPTAgent.Instance.GetChatResponseStream(prompt, ChatRole.User);

            // Streamed chunks are added from the subscribed event GPTAgent_OnStreamChunkReceived

            // Clear the input field text
            inputField.text = string.Empty;
            inputField.ActivateInputField();

            return;
        }



        private void Update()
        {
            // Use Return key to submit
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (inputField.text.Length > 0)
                {
                    SubmitInput();
                }
            }

            //chatTMP.text = gptAgent.streamedText;
        }

        public void SubmitInput()
        {
            GetStreamFromAIModel();
        }

        string FormatChatGPTTextWithRole(string[] responseArray)
        {
            return "\n\n" + $"{CapitalizedFirstChar(responseArray[0])}: {responseArray[1]}";
        }

        string FormatChatGPTTextWithRole(string jsonString)
        {
            //jsonString = "{" + jsonString + "}";
            ChatResponse.Message msg = JsonConvert.DeserializeObject<ChatResponse.Message>(jsonString);
            return "\n\n" + $"{CapitalizedFirstChar(msg.role)}: {msg.content}";
        }

        string CapitalizedFirstChar(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
