using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TzarGPT
{
    public class DemoChat : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] TextMeshProUGUI chatTMP;
        [SerializeField] GPTAgent gptAgent;

#pragma warning disable CS0414
        [TextAreaAttribute(1, 50)]
        [SerializeField] string notes = "NOTE! You need to manually handle stop sequences and append them programmatically or write them in the prompt." +
            " Check how stop sequences are handled in this script in GetResponseFromAIModel() for more info. If you are not using stop sequences, just send the input";
#pragma warning restore CS0414

        private void Start()
        {
            inputField.ActivateInputField();
            // Display the initial prompt
            UpdateTextBoxOnProfileChange();
        }

        public async void GetResponseFromAIModel()
        {
            // Append the stop sequences automatically so we don't have to write them
            // NOTE! You need to manually handle stop sequences and append them programmatically or write them in the prompt
            string formattedText = $"{gptAgent.AIProfile.stopSequences[0]} {inputField.text} {gptAgent.AIProfile.stopSequences[1]}";

            // Format the input text for the text field and add it
            chatTMP.text += "\n" + inputField.text;

            // Await the response
            string response = await GPTAgent.Instance.GetAIResponse(formattedText);

            // Format the response for the text field and add it
            chatTMP.text += "\n" + response;

            // Clear the input field text
            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }

        public void UpdateTextBoxOnProfileChange()
        {
            chatTMP.text = gptAgent.FullPromptVisualizer;
        }

        private void Update()
        {
            // Use Return key to submit
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (inputField.text.Length > 0)
                {
                    GetResponseFromAIModel();
                }
            }
        }

        #region Simple Examples
        async void GetStringResponse()
        {
            string aiResponse = await gptAgent.GetAIResponse("Who are you?");
        }

        async void GetObjectResponse()
        {
            AIResponse aiResponseObject = await gptAgent.GetAIResponseObject("Who are you?");

            // Access string responses inside object
            string strResponse = aiResponseObject.choices[0].text;
        }

        async void GetResponsesUsingSingleton()
        {
            string aiResponse = await GPTAgent.Instance.GetAIResponse("Who are you?");
            AIResponse aiResponseObject = await GPTAgent.Instance.GetAIResponseObject("Who are you?");
        }
        #endregion
    }
}
