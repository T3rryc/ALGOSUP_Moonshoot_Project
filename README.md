# 🎯 Moonshot Project – ALGOSUP
## 🧠 Remora: AI-Powered VR Simulation for Social Communication Training
### Overview
Remora is a virtual reality simulation developed in Unity, designed to support neurodiverse individuals—particularly those on the autism spectrum—in practicing social interaction in a safe, immersive environment. The project centers around a realistic interaction with a friendly AI-powered NPC (shopkeeper), capable of holding natural voice-based conversations with the user.

This simulation leverages OpenAI technologies (ChatGPT, Whisper, and TTS) to create a full conversational pipeline: the user speaks, their voice is transcribed, a response is generated, and that reply is spoken aloud with synchronized text display. The experience mimics a real-world scenario—such as asking for help in a store—and aims to improve confidence and comfort in everyday interactions.

### Problematic to Solve
Many people with Autism Spectrum Disorder (ASD) face challenges in social settings—particularly in spontaneous verbal interactions like asking for assistance or managing a brief conversation with a stranger.

In France, ASD affects more than 700,000 individuals, yet field-ready tools for building social confidence remain limited. Existing solutions, such as the use of an AVS (Auxiliaire de Vie Scolaire), often require the constant presence of a trained human assistant, which is not scalable, not autonomous, and not always possible in daily life.

Remora proposes an alternative: a digital, AI-powered social "copilot" that enables the user to engage directly in simulated conversations without human intervention. Like glasses for visual impairments, this tool does not replace human interaction—it makes it more accessible and manageable.


### Why Focus on VR + Conversational AI?
ASD is a cognitive difference, not a flaw. But many individuals on the spectrum experience heightened stress or confusion during unstructured or socially nuanced interactions.

Traditional teaching methods often fall short in preparing them for the unpredictability of real-life encounters. VR provides the safe, repeatable, immersive space needed for experiential learning.

When combined with AI:

OpenAI Whisper converts the user’s voice into text

ChatGPT simulates realistic replies from an NPC shopkeeper

OpenAI TTS vocalizes the response with warmth and clarity

This real-time, AI-driven dialogue loop supports natural practice and builds verbal fluency without fear of judgment or mistakes.



### Why the Name Remora?
The name “Remora” references a fish known for attaching to larger animals like sharks or turtles, forming a helpful, symbiotic relationship. In the same way, this application acts as a digital "copilot"—nonintrusive, always available, and assisting the user as they navigate complex social situations.

It also subtly echoes the strong interest in animals often observed among neurodiverse individuals, reinforcing relatability and user comfort.


### What Does the Software Do?
Remora places the user inside a fully navigable virtual store environment. There, they can:

Ask questions or make requests by speaking aloud

See and hear the shopkeeper’s response (text bubble + voice)

Interact using a controller, hand tracking, or buttons as fallback

Learn from realistic back-and-forth conversation cycles

Build verbal confidence in a safe, low-pressure space

Unlike tools that require scripting or pre-recorded responses, all interactions are dynamically generated, meaning the NPC can answer flexibly and contextually like a real person.

### Project Building 
##  Requirements

- **Unity Editor**: 2022.3 LTS (recommended)  
- **Unity Hub** (to manage versions & modules)  
- **Visual Studio 2019/2022** with:
  - `.NET Desktop Development`
  - `Desktop Development with C++`
  - `Game Development with Unity`
- **Windows SDK** (≥ 10.0.19041.0)  
- **Git LFS** (if cloning from repo with large assets)  
- (Optional) VR Headset with OpenXR support (Quest/Quest 2/Pro, or SteamVR devices) 

## Environment Variables

Create a .env file in the root of the Remora folder with the following content:

    
    OPENAI_KEY=<YOUR API KEY>

On the scene MiniMarket.scene, Café.scene and TrainStation.scene, add your API key on the Inspector to
- OpenAIChat Object
- AIDebugger Object
- NPC Object
- MicrophoneController Object

Now you can run the project in the Unity Editor.

To build the project for deployment, go to File > Build Settings and select your target platform (e.g., Android for Meta Quest 2). Then click Build and Run.
Make sure to set the scene in build profiles --> scenes list, the first scene should be MainMenu.scene.

## Run VR Headset
   Requirements:
  - Meta Quest Link
  - Oculus App (for Quest 2)
  - USB-C cable (for wired connection)
  - VR Headset (e.g., Meta Quest 2)
  - Meta account 
  
To connect the VR to unity and the .exe files
- Connect to your Meta profile
- Make sure Meta Quest Link is enabled
- Enable Link in the Meta interface (A pop-up should appear to enable it otherwise, go to quick settings, a Button "Enable Link" should be visible, enable it and access to Unity and the .exe files)
- Run the editor or the .exe files from the PC.
- The VR headset should now be connected and ready for use.

Important Notes:
- Always ensure your VR headset is updated to the latest firmware.
- Make sure to connect both devices on the same network (5 GHz recommended).
- To Run Meta Quest 2 in Unity, You need to meet the minimum system requirements for VR.

| Minimum      | Recommended  |
| ----------- | ----------- |
| Processor - Intel i5-4590 / AMD Ryzen 5 1500X or greater|Processor - Intel i7/ AMD ryzen 7|
|NVIDIA GeForce GTX 970 / AMD Radeon 400 Series or greater*|Graphics Card - Nvidia RTX 20 Series / AMD Radeon RX 6000 Series|
|8GB RAM|16GB RAM DDR4|
|Windows 10,11|Windows 10,11|
|USB - Ports 1x USB port| USB - Ports 1x USB-C port|

* Check the list of graphics card compatibility to see the supported models on the [Meta Quest Website](https://www.meta.com/fr-fr/help/quest/140991407990979/?srsltid=AfmBOopBfN8S8Djd7y785RgtCMj0i_9Pv7tlW69iv4HjaMI75nKA4xzu)  
