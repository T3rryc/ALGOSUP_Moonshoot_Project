using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace TzarGPT
{
    public class GPTDemoChat : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] TextMeshProUGUI chatTMP;
        [SerializeField] GPTAgent gptAgent;

        private void Start()
        {
            inputField.ActivateInputField();
            // Display the initial prompt if any
            if(gptAgent.FullPromptVisualizer.Length > 0)
                chatTMP.text = FormatChatGPTTextWithRole(gptAgent.FullPromptVisualizer);
        }

        public async Task<string> GetResponseFromAIModel(string overrideInput = "")
        {
            string prompt = overrideInput.Length == 0 ? inputField.text : overrideInput;

            // Format the input text for the text field and add it
            chatTMP.text += "\n\n" + "User: " + prompt;

            // Await the response
            string[] response = await GPTAgent.Instance.GetChatResponseWithRole(prompt, ChatRole.User);

            // Add the response to the text field
            chatTMP.text += FormatChatGPTTextWithRole(response);

            // Clear the input field text
            inputField.text = string.Empty;
            inputField.ActivateInputField();

            return response[0];
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
        }

        public void SubmitInput()
        {
            _ = GetResponseFromAIModel();
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
