# 🛠️ Technical Specification Document
Project Name: Remora

Version: 1.0

Author: Namir Salaheddine

Date: June 2025

Platform: Unity (Meta Quest 2)



## Introduction

This document outlines the technical architecture and implementation details of Remora, a VR application that enables voice-based conversations with an AI-powered NPC using Unity and OpenAI APIs. It provides guidance for developers on how the system is structured, how its components interact, and how to extend or maintain the project.


## Technology Stack

Engine: Unity 6000.0.43f1

Target Platform: Meta Quest 2 

Language: C#

Input: Unity Microphone API, XR Toolkit

Audio: Unity AudioSource, OpenAI TTS (MP3)

AI APIs: OpenAI ChatGPT (text), Whisper (audio transcription), TTS (speech synthesis)

Networking: UnityWebRequest with HTTP POST (RESTful JSON/multipart)

File Format: WAV (for Whisper), MP3 (for TTS)

UI Framework: Unity UI Toolkit / Canvas-based UI



## System Architecture

### Subsystems:

UI Layer: Buttons, text bubble, feedback icons

Input Layer: Mic recording, button press detection

Networking Layer: API requests and response parsing

AI Layer: ChatGPT (text), Whisper (speech-to-text), TTS (text-to-speech)

NPC Layer: Audio playback, dialogue display, animation (future)

### Data Flow:

User speaks → Audio recorded → Saved as WAV → Sent to Whisper → Transcript → Sent to ChatGPT → Response → Sent to TTS → MP3 played & displayed as text



### 🔧Modules and Components

This section describes the core C# scripts responsible for orchestrating the AI-driven NPC interaction pipeline. These components are loosely coupled and interact through clearly defined function calls and Unity MonoBehaviour event handling.

#### 4.1 DialogueManager.cs
📌 Purpose: Acts as the central orchestrator for conversation logic. It bridges user inputs (text or voice) with AI-generated replies and handles the routing of messages between UI and audio subsystems.

🛠️ Key Responsibilities:

- Receives user inputs (from button clicks or Whisper transcription)

- Sends input to the OpenAIChat module for processing

- Receives generated responses from ChatGPT

- Triggers OpenAITTS to vocalize the reply

- Updates the UI (e.g., text bubble) to show what the NPC is saying

🔁 Typical flow:

```csharp
public void SendUserSpeech(string transcript)
{
    chat.SendPrompt(transcript); // Calls OpenAIChat
}
```
💬 Output:

- Calls OpenAITTS.Speak(response)

- Calls UIManager.ShowTextBubble(response)

🔗 Dependencies:

- OpenAIChat

- OpenAITTS

- UI (e.g., TextMeshPro or speech bubble handler)

#### 4.2 MicRecorder.cs
📌 Purpose: Manages audio input from the user’s microphone, handles WAV encoding, and sends voice input to OpenAI Whisper for transcription.

🛠️ Key Responsibilities:

- Starts and stops microphone recording

- Converts AudioClip into a WAV file using a custom WAV encoder

- Sends WAV file to OpenAI Whisper API for transcription

- Forwards the recognized text to DialogueManager

🎙️ Code snapshot:

```csharp
public void StopRecordingAndSend()
{
    Microphone.End(null);
    SaveWav(filePath, recordedClip);
    StartCoroutine(SendToWhisper(filePath));
}
```

📥 Output:

- Calls DialogueManager.SendUserSpeech(transcribedText)

🔗 Dependencies:

- Microphone API

- UnityWebRequest (POST to Whisper endpoint)

- DialogueManager

#### 4.3 OpenAIChat.cs
📌 Purpose: Manages communication with OpenAI's ChatGPT API to generate responses from the NPC based on user input.

🛠️ Key Responsibilities:

- Constructs a valid chat/completions request (system + user messages)

- Sends the request to OpenAI using UnityWebRequest

- Parses the assistant’s reply from the JSON response

- Sends the result back to DialogueManager

💬 Request example:

```json
{
  "model": "gpt-4o",
  "messages": [
    { "role": "system", "content": "You are a helpful shopkeeper..." },
    { "role": "user", "content": "Where can I find bread?" }
  ]
}
```
🧠 Code logic:

```csharp
public void SendPrompt(string userInput)
{
    // Build JSON and send POST
    // On success, call dialogueManager.OnAIReply(response);
}
```
📤 Output:

- Forwards raw reply text to TTS + UI

🔗 Dependencies:

- UnityWebRequest

- DialogueManager


#### 4.4 OpenAITTS.cs
📌 Purpose: Handles Text-to-Speech functionality using OpenAI’s /v1/audio/speech endpoint. Converts AI responses into audio and plays them back via AudioSource.

🛠️ Key Responsibilities:

- Prepares a JSON payload with voice settings and the input text

- Sends a POST request to OpenAI's TTS endpoint

- Receives MP3 audio data and saves it locally

- Loads the MP3 as an AudioClip and plays it back on the NPC

📦 Sample payload:

```json
{
  "model": "tts-1",
  "voice": "coral",
  "input": "Of course! The apples are over here.",
  "response_format": "mp3"
}
```
🔊 Key method:

```csharp
public void Speak(string text)
{
    StartCoroutine(SendTTSRequest(text));
}
```
📤 Output:

- Plays audio via audioSource.Play()

- Optionally triggers a text bubble update in parallel

🔗 Dependencies:

- UnityWebRequest

- AudioSource

- File I/O (WAV or MP3 saving/loading)


🧩 Module Integration Diagram (optional for visuals):

User (Voice or Button)

 ⬇

MicRecorder.cs → Whisper API

 ⬇

DialogueManager.cs

 ⬇

OpenAIChat.cs → ChatGPT

 ⬇

DialogueManager.cs

 ⬇

OpenAITTS.cs → TTS API

 ⬇

AudioSource.Play()




## File Handling

Audio captured as AudioClip → converted to .wav (16-bit PCM, 44.1kHz mono)

WAV encoded using custom class (WavUtility or similar)

TTS audio received as MP3 → saved to StreamingAssets or Application.persistentDataPath



## API Request Formats

### 6.1 Whisper

🧠 Speech-to-Text (STT) System — Whisper API Integration
Module: MicRecorder.cs

#### Overview
The Whisper integration enables the user to speak naturally into a microphone. Their voice is recorded as a WAV file and sent to OpenAI’s /v1/audio/transcriptions endpoint. The resulting transcription is automatically passed to the dialogue system, allowing real-time, voice-driven interaction with the NPC.



#### Microphone Recording — Capturing User Speech

The user initiates recording by calling StartRecording(), which uses Unity’s Microphone API:

```csharp
csharp
public void StartRecording()
{
    recordedClip = Microphone.Start(null, false, 5, 44100);
    Debug.Log("🎙️ Started recording...");
}
```

Records from the default microphone

Captures 5 seconds at 44.1 kHz, mono

The recorded clip is stored in a local AudioClip reference

Recording is stopped manually by calling:

```csharp
public void StopRecordingAndSend()
{
    Microphone.End(null);
    // ...
}
```

#### WAV File Encoding

Once recording stops, the AudioClip is serialized to a WAV file. This is done using a helper method SaveWav:

```csharp
void SaveWav(string filePath, AudioClip clip)
{
    var samples = new float[clip.samples];
    clip.GetData(samples, 0);

    byte[] wav = WavUtility.FromAudioClip(clip, samples, clip.channels, clip.frequency);
    File.WriteAllBytes(filePath, wav);
    Debug.Log("✅ WAV saved: " + filePath);
}
```

Extracts raw float samples from the clip

Converts the audio to WAV format

Saves the file at: Application.persistentDataPath/recorded.wav

Note: This WAV file is required by the Whisper API.

#### Whisper API Request — Sending Audio for Transcription

The coroutine SendToWhisper handles the POST request to OpenAI:

```csharp
private IEnumerator SendToWhisper(string path)
{
    byte[] audioData = File.ReadAllBytes(path);

    WWWForm form = new WWWForm();
    form.AddBinaryData("file", audioData, "recording.wav", "audio/wav");
    form.AddField("model", "whisper-1");

    UnityWebRequest www = UnityWebRequest.Post("https://api.openai.com/v1/audio/transcriptions", form);
    www.SetRequestHeader("Authorization", "Bearer " + apiKey);

    yield return www.SendWebRequest();
}
```
Request details:

Endpoint: /v1/audio/transcriptions

Content-Type: multipart/form-data

Fields:

file (WAV)

model = whisper-1

Authorization: Bearer {apiKey}

The response is a JSON object that includes the transcribed text.

#### Response Handling and Callback

After the response is received:

```csharp
string json = www.downloadHandler.text;
WhisperResponse parsed = JsonUtility.FromJson<WhisperResponse>(json);
```
Then the parsed text is forwarded into the dialogue system:

```csharp
if (!string.IsNullOrEmpty(parsed.text))
{
    dialogueManager.SendUserSpeech(parsed.text); // Launches the AI response
}
```
This triggers the same process as a button-activated prompt.

#### Output Example

Voice input: “Where can I find the apples?”
↓
Whisper returns: "Where can I find the apples?"
↓
DialogueManager sends this to ChatGPT → NPC replies via TTS


#### Class Structure

Public Methods:

StartRecording()

StopRecordingAndSend()

Private Coroutine:

SendToWhisper(string path)

Nested Class:

WhisperResponse for JSON parsing

Dependencies:

AudioClip, Microphone API, UnityWebRequest

WavUtility (custom or third-party utility for WAV export)

—

🛡️ Security Note

The API key is stored in a serialized string field

The WAV file is stored locally but deleted/overwritten after each use (or should be)

Recording requires mic permission on Android/Quest 2

—

### 6.2 TTS

🗣️ Text-to-Speech (TTS) System — Technical Description
Module: OpenAITTS.cs

#### Overview
The TTS system is responsible for transforming the NPC's response (generated by ChatGPT) into spoken audio using OpenAI’s /v1/audio/speech endpoint. The resulting speech is played back through an AudioSource component in Unity, making the interaction feel more lifelike and accessible.


#### Entry Point: Calling the Speak() Method

The public method Speak(string text) is used to trigger voice synthesis. It accepts a string (typically a ChatGPT reply) and starts an asynchronous coroutine to handle the TTS process.

Example:
```csharp
public void Speak(string text)
{
    StartCoroutine(SendTTSRequest(text));
}
```

### Composing the TTS Request (JSON Body)

Within the coroutine SendTTSRequest, the text is formatted as a JSON payload. The payload includes the following fields:

model: "tts-1"

input: the raw text to convert

voice: one of OpenAI’s supported voices (e.g. "coral")

instructions: optional styling (e.g., tone or mood)

response_format: "mp3"

Snippet:
```csharp
string jsonBody = $@"
{{
    ""model"": ""tts-1"",
    ""input"": ""{inputText}"",
    ""voice"": ""coral"",
    ""instructions"": ""Speak in a cheerful and positive tone."",
    ""response_format"": ""mp3""
}}";
```	
This JSON string is then encoded as a byte array:
```csharp

byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
```

### Sending the HTTP Request

A UnityWebRequest is created to POST the payload to OpenAI’s TTS endpoint:
```csharp
string url = "https://api.openai.com/v1/audio/speech";

UnityWebRequest request = new UnityWebRequest(url, "POST");
request.uploadHandler = new UploadHandlerRaw(bodyRaw);
request.downloadHandler = new DownloadHandlerBuffer();
request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
request.SetRequestHeader("Content-Type", "application/json");
```

🛡️ CertificateHandler is overridden in development to bypass SSL issues:

```csharp
request.certificateHandler = new BypassCertificate();
```

#### Handling the Response

Once the server responds, the received MP3 binary is written to disk:

```csharp
byte[] audioData = request.downloadHandler.data;
string path = Path.Combine(Application.persistentDataPath, "speech.mp3");
File.WriteAllBytes(path, audioData);
```


#### Playing the Synthesized Audio

The saved MP3 is then loaded into Unity using UnityWebRequestMultimedia:

```csharp
UnityWebRequest wwwAudio = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG);
```
If successful, the AudioClip is assigned to the NPC's AudioSource and played:

```csharp
AudioClip clip = DownloadHandlerAudioClip.GetContent(wwwAudio);
audioSource.clip = clip;
audioSource.Play();
```

#### Error Logging and Debugging

If the API call fails, error codes and response text are logged:

```csharp
if (request.result != UnityWebRequest.Result.Success)
{
    Debug.LogError($"TTS Request Error: {request.responseCode} - {request.error}");
    Debug.LogError("Server response: " + request.downloadHandler.text);
}
```

#### Security Considerations

The API key is injected via a serialized field and passed in the Authorization header:

```csharp
request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
```
A local daily rate limiter can be added to restrict overuse (see RateLimiter.cs in other parts of the system). The certificate bypass should be disabled in production builds.

## UI / Scene Management / Prompt Handling

### Scene Transitions
- Implement smooth transitions between different VR scenes (e.g., store, café, train station).
- Use Unity's SceneManager for loading and unloading scenes.

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); // Works only in build
    }
}
```

### Scene Reset
- Implement a mechanism to reset the current scene, allowing users to restart interactions without returning to the main menu.
- Use SceneManager.LoadScene with the current scene name to reload it.

```csharp
    public void ResetScene()
    {
        // Get the current scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
```

### VR Checker
- Implement a VRChecker class to ensure the user is in a VR environment before allowing certain interactions.
- Use Unity's XR API to check for VR device presence.

```csharp
     void Start()
    {
        if (XRSettings.isDeviceActive)
        {
            // Headset is connected
            vrRig.SetActive(true);
            nonVrRig.SetActive(false);
        }
        else
        {
            // No headset found
            vrRig.SetActive(false);
            nonVrRig.SetActive(true);
        }
    }
```

### Prompt Handling

- Implement a system for handling user prompts and responses within the VR environment.
- Use a combination of voice recognition and button inputs to capture user intent.
- Send user prompts to the ChatGPT API for processing and receive responses for NPC interactions.

```csharp
  public static string GetPrompt(string sceneName)
    {
        switch (sceneName)
        {
            case "MiniMarket":
                return Shopkeeper;
            case "Café":
                return Cafe;
            case "TrainStation":
                return TrainStation;
            default:
                return Shopkeeper; // fallback
        }
    }
```

## Security
A local rate-limiting mechanism enforces a daily cap on API queries to prevent abuse of the shared key.

A feature for interact to the NPC can use the microphone to record their own voice and send it to the Whisper API for transcription, allowing for personalized interactions. The user has also use button prompts to interact with the NPC, which sends predefined text to the ChatGPT API for generating responses.
The data isn't stored permanently, and the application does not collect any personal information from users.

Users are encouraged to use their own OpenAI API key if they exceed the allowed daily limit or intend to use the project extensively.

For this project, the key is hidden for local development and testing purposes. In production, it should be securely stored and accessed via environment variables or secure vaults. The file EnvLoader.cs is designed to load the API key from a secure location, such as a local file or environment variable, to avoid hardcoding sensitive information in the source code. the file with the key is on .env file, which is not included in the repository to prevent accidental exposure with the .gitignore file.


## Extensibility

Dialogue context can be expanded with memory (future)

Additional NPCs can reuse DialogueManager with different prompts

UI and scene templates can be swapped for other environments (school, home, etc.)

Localization framework can be added for multilingual use

