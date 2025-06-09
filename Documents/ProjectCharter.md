# 🧾 Project Charter

## Remora — Virtual Reality Simulation: AI-Powered Social Conversation Trainer

Date: June 2025
Version: 1.0
Project Name: Remora
Author: Namir Salaheddine
Platform: Unity for Meta Quest 2 / PC VR
Project Type: Virtual Reality Simulation

## Project Overview
Remora is a virtual reality simulation designed to offer users an immersive environment in which they can engage in realistic, low-pressure social interactions with a shopkeeper NPC. The shopkeeper’s responses are generated using OpenAI’s ChatGPT, and user speech is transcribed via OpenAI Whisper. The goal is to provide a safe and repeatable social training tool, particularly for neurodiverse individuals, such as those on the autism spectrum, who may experience challenges in real-world social situations.

## Project Purpose
The purpose of this project is to explore the integration of conversational AI into a VR context to simulate everyday human interaction in a structured and non-threatening setting. The intention is to support the development of social communication skills through interaction with a friendly and context-aware virtual agent in a controlled virtual space.

## Project Objectives

Design and implement a small retail store environment in virtual reality.

Enable user interaction with an NPC through both voice and text input.

Use OpenAI ChatGPT to dynamically generate context-sensitive responses.

Use OpenAI Text-to-Speech (TTS) to vocalize the NPC’s replies.

Display NPC responses in a speech bubble above the character.

Incorporate button-based prompts as an alternate interaction method.

Record user voice input via microphone and transcribe it using Whisper API.

Ensure stable performance on Meta Quest 2 hardware.

Structure the application to allow for future extension to additional scenarios or locations.

## Project Scope

### In Scope:

3D store layout compatible with VR interaction and locomotion

NPC shopkeeper prefab with animation, audio playback, and UI integration

UI components for initiating conversation (e.g., “Ask a question”)

API communication with OpenAI ChatGPT, Whisper, and TTS services

Voice capture and audio preprocessing pipeline for speech-to-text

Feedback UI elements to indicate when the system is listening or replying

Out of Scope (Current Version):

Facial animation or lip sync beyond audio playback

Long-term conversational memory or emotional context retention

Multiplayer or multi-NPC interactions

Voice cloning or advanced personalization of the NPC

Key Deliverables

Unity project with VR-ready simulation scene

NPC character with full voice interaction loop

Scripted microphone recording system and audio file generation

Whisper API integration for voice-to-text

ChatGPT integration for conversational generation

TTS integration for synthesized speech playback

Setup and configuration documentation for deployment and usage

## Milestones

| Milestone                          | Status     |
|------------------------------------|------------|
| VR Store Environment Setup         | Complete ✅ |
| ChatGPT Dialogue Integration       | Complete ✅ |
| TTS Voice Synthesis                | Complete ✅ |
| Dialogue Display (Text Bubble)     | Complete ✅ |
| Voice Input (Mic + Whisper)        | Complete ✅ |
| Documentation                      | Complete ✅ |

## Stakeholders

Project Author: Namir Salaheddine

Intended Users: Neurodiverse individuals practicing conversational skills

ALGOSUP: School who supervise the project

## Assumptions and Constraints

### Assumptions:

The target device has a working microphone and stable internet access.

OpenAI API credentials are available and within usage limits.

Users are able to engage with VR input methods (controllers or hand tracking).

### Constraints:

Performance limitations on standalone VR hardware (Meta Quest 2).

Latency introduced by API response times.

User privacy considerations related to audio input and AI processing.

### Success Criteria

The system allows users to initiate and complete basic conversations with the NPC using voice.

The dialogue generated is coherent, friendly, and contextually relevant.

Text and speech feedback are synchronized and clear.

The system performs reliably across multiple conversation turns without critical failures.

