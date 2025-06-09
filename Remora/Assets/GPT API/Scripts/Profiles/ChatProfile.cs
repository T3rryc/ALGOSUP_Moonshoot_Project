using System;
using UnityEngine;

namespace TzarGPT
{
    [CreateAssetMenu(fileName = "ChatProfile", menuName = "ScriptableObjects/ChatProfile", order = 1)]
    public class ChatProfile : ScriptableObject
    {
        [SerializeField]
        [TextAreaAttribute(1,10)]
        [Header("You can leave notes for yourself here on how this profile behaves")]
        string profileNotes;
        [Header("Chat AI Settings")]
        [Tooltip("gpt-3.5-turbo\ngpt-3.5-turbo-0301\n\ntext-ada-001\ntext-babbage-001\ntext-curie-001\ntext-davinci-003\n\ncode-davinci-002\ncode-cushman-001")]
        public string model = "text-curie-001";
        [Tooltip("The maximum number of tokens to generate. Requests can use up to" +
            " 2048 or 4000 tokens shared between prompt and completion. The exact limit " +
            "varies by model. (One token is roughly 4 characters for normal English text)")]
        public int maxTokens = 15;
        [Tooltip("What sampling temperature to use, between 0 and 2. Higher values like 0.8 will" +
            " make the output more random, while lower values like 0.2 will make it more focused and deterministic. " +
            "We generally recommend altering this or top_p but not both.")]
        public double temperature = 0.9;
        [Tooltip("An alternative to sampling with temperature, called nucleus sampling, where the model considers the " +
            "results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered. " +
            "We generally recommend altering this or temperature but not both.")]
        public double topP = 1;
        [Tooltip("How much to penalize new tokens based on their existing frequency in the " +
            "text so far. Decreases the model's likelihood to repeat the same line verbatim.")]
        public double frequencyPenalty = 0.6;
        [Tooltip("How much to penalize new tokens based on whether they appear in the text " +
            "so far. Increases the model's likelihood to talk about new topics.")]
        public double presencePenalty = 0.6;
        [Tooltip("Generates multiple completions server-side, and displays only the best. Streaming " +
            "only works when set to 1. Since it acts as a multiplier on the number of completions, this " +
            "parameter can eat into your token quota very quickly - use with caution!")]
        public int bestOf = 1;
        [Tooltip("Up to four sequences where the API will stop generating further tokens. The returned" +
            " text will not contain the stop sequence.")]
        public string[] stopSequences = new string[0];
        [Tooltip("The initial prompt to set the tone/persona of the AI. Check different profiles to see how it's used " +
            "in different situations. When using GPT 3.5 models, the role used for the initial prompt will be System.")]
        [TextAreaAttribute(5,50)]
        public string initialPrompt = string.Empty;

        [Header("Autoparser")]
        [SerializeField]
        [TextAreaAttribute(5,15)]
        [Tooltip("You can paste the json directly from OpenAI's examples and click parse to fill the profile values")]
        string jsonDataToParse;

        public void ParseProfileDataFromJSON()
        {
            ProfileData jsonData = JsonUtility.FromJson<ProfileData>(jsonDataToParse);

            model = jsonData.model;
            initialPrompt = jsonData.prompt;
            temperature = jsonData.temperature;
            maxTokens = jsonData.max_tokens;
            topP = jsonData.top_p;
            frequencyPenalty = jsonData.frequency_penalty;
            presencePenalty = jsonData.presence_penalty;
            stopSequences = jsonData.stop;

            //jsonDataToParse = string.Empty;
        }
    }
}
