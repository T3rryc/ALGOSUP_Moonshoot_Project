using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    public Button[] choiceButtons;

    public OpenAIChat aiChat;

    public OpenAITTS tts; // Drag this in from the Inspector

    private int state = 0;

    private bool isWaitingForAI = false;

    void Start()
    {
        ShowGreeting();
    }

    void ShowGreeting()
    {
        state = 1;
        dialogueText.text = "Hi there! Can I help you with something?";
        SetChoices("Yes, please", "No, thank you");
    }

    void SetChoices(string option1, string option2, string option3 = "")
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].gameObject.SetActive(false); // Hide by default
        }

        choiceButtons[0].gameObject.SetActive(true);
        choiceButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = option1;
        choiceButtons[0].onClick.AddListener(() => OnChoice(0));

        choiceButtons[1].gameObject.SetActive(true);
        choiceButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = option2;
        choiceButtons[1].onClick.AddListener(() => OnChoice(1));

        if (!string.IsNullOrEmpty(option3) && choiceButtons.Length > 2)
        {
            choiceButtons[2].gameObject.SetActive(true);
            choiceButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = option3;
            choiceButtons[2].onClick.AddListener(() => OnChoice(2));
        }
    }

    private bool isWaitingForResponse = false;

    void OnChoice(int choiceIndex)
    {
        if (isWaitingForResponse)
            return;

        switch (state)
        {
            case 1:
                if (choiceIndex == 0)
                    ShowRequest();
                else
                    EndDialogue("Alright, let me know if you need anything!");
                break;

            case 2:
                isWaitingForResponse = true;

                string prompt = choiceIndex switch
                {
                    0 => "Where is the dairy section?",
                    1 => "Where are the drinks?",
                    2 => "Where are the fruits?",
                    _ => "Can you help me?"
                };

                StartCoroutine(SendPromptToAI(prompt, goToFarewell: true));
                break;

            case 3:
                if (choiceIndex == 0)
                {
                    ShowRequest();
                }
                else
                {
                    isWaitingForResponse = true;
                    StartCoroutine(SendPromptToAI("Thank you, goodbye!", goToFarewell: false, endAfter: true));
                }
                break;
        }
    }

    void ShowRequest()
    {
        state = 2;
        dialogueText.text = "Sure! What are you looking for?";
        SetChoices("Dairy", "Drinks", "Fruits");
    }

    void Respond(string response)
    {
        state = 3;
        dialogueText.text = response + "\n\nAnything else I can help with?";
        SetChoices("Yes", "No");
    }

    void EndDialogue(string message)
    {
        dialogueText.text = message;
        foreach (var btn in choiceButtons)
            btn.gameObject.SetActive(false);
    }

    IEnumerator SendPromptToAI(string prompt, bool goToFarewell = false, bool endAfter = false)
    {
        foreach (var btn in choiceButtons)
            btn.gameObject.SetActive(false);

        aiChat.latestResponse = "";
        aiChat.Ask(prompt);

        yield return new WaitUntil(() => !string.IsNullOrEmpty(aiChat.latestResponse));

        dialogueText.text = aiChat.latestResponse;
        tts.Speak(aiChat.latestResponse);

        if (endAfter)
        {
            foreach (var btn in choiceButtons)
                btn.gameObject.SetActive(false);
            isWaitingForResponse = false;
            yield break;
        }

        if (goToFarewell)
        {
            state = 3;
            SetChoices("Ask something else", "End conversation");
        }

        isWaitingForResponse = false;
    }

    IEnumerator WaitForAIResponse()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(aiChat.latestResponse));
        dialogueText.text = aiChat.latestResponse;

        state = 3;
        SetChoices("Ask something else", "End conversation");
        isWaitingForAI = false;
    }

    // 🆕 Called by MicRecorder.cs
    public void SendUserSpeech(string recognizedText)
    {
        Debug.Log("🎧 Recognized speech: " + recognizedText);
        StartCoroutine(SendPromptToAI(recognizedText, goToFarewell: true));
    }
}
