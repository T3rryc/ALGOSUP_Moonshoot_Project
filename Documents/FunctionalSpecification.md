# 🧩 Functional Specification Document
Project Name: Remora 

Version: 1.0

Author: Namir Salaheddine

Date: June 2025

Platform: Unity for Meta Quest 2


## Introduction

### 1.1 Purpose
This document outlines the functional specifications of the Remora VR project — a simulation designed to provide a safe, interactive environment for users, particularly neurodiverse individuals, to practice social interactions using a voice-enabled, AI-powered NPC.

### 1.2 Target Audience
This simulation is designed for individual users who require support in practicing everyday social communication, and for educators, therapists, or researchers evaluating virtual tools for behavioral training.

### 1.3 Scope
This version of Remora is focused on simulating a single retail store interaction involving one shopkeeper NPC and a user. It supports both text-based and voice-based interaction using natural language processing and speech synthesis via OpenAI services.



## System Overview

### 2.1 Architecture Summary

Engine: Unity (VR-capable)

Hardware Target: Meta Quest 2 

Language Processing: OpenAI ChatGPT API

Text-to-Speech: OpenAI TTS API

Speech-to-Text: OpenAI Whisper API

Input: VR controller, or microphone

Output: Audio speech, 3D UI text bubble



## Functional Requirements

### 3.1 Environment Setup

Name: VR Store Scene
Description: A simple, stylized virtual store containing product shelves, signage, and walkable space with locomotion or teleportation support.
Status: Implemented ✅

### 3.2 Shopkeeper NPC

Name: AI-Powered NPC
Description: A stationary humanoid character representing a shopkeeper. NPC responds to user questions using voice and text bubbles.
Status: Implemented ✅

### 3.3 Chat-Based Dialogue System

Name: Button-Based Prompt System
Description: Users can click predefined options like “Ask for help” to trigger a chat prompt to ChatGPT.
Status: Implemented ✅

### 3.4 Voice Interaction System

Name: Microphone Input + Transcription
Description: The system records voice input from the user, converts it to a .wav file, and sends it to OpenAI Whisper for transcription.
Status: Implemented ✅

Name: Whisper Integration
Description: The audio file is sent via HTTP to OpenAI’s Whisper API. The returned text is forwarded to the chat system.
Status: Implemented ✅

### 3.5 Conversational AI

Name: ChatGPT Integration
Description: The input (button text or Whisper transcription) is sent to OpenAI’s ChatGPT API with pre-defined prompt context (e.g. “You are a helpful shopkeeper”).
Status: Implemented ✅

### 3.6 Response Rendering

Name: NPC TTS Voice
Description: ChatGPT’s response is sent to OpenAI TTS API and returned as an mp3. The result is played aloud using Unity’s AudioSource.
Status: Implemented ✅

Name: Text Bubble Display
Description: The spoken message is also rendered above the NPC in a floating UI bubble.
Status: Implemented ✅

### 3.7 Feedback and State UI

Name: Listening Indicator
Description: A UI element (e.g. an icon or animation) indicates when the system is recording or waiting for a response.
Status: Implemented ✅


## Non-Functional Requirements

### 4.1 Performance

Whisper, ChatGPT, and TTS round-trip latency should remain under ~5 seconds

### 4.2 Accessibility

Button interface remains available as fallback to voice

UI text uses large, readable fonts and simple language

### 4.3 Network

Requires continuous internet access for API communication

### 4.4 Data Privacy

No user voice/audio is stored locally unless explicitly allowed

API keys are securely stored and not embedded in public builds

### 4.5 Compatibility
Must run on Unity 2021.3 LTS or later



## User Flows

### 5.1 Button-Based Conversation

User enters store

User selects a prompt from UI

System sends prompt to ChatGPT

NPC speaks response via TTS

NPC shows text bubble of response

### 5.2 Voice-Based Conversation

User speaks (via mic hotkey or auto-trigger)

Audio is recorded and saved as .wav

File is sent to Whisper API for transcription

Transcribed text is sent to ChatGPT

Response is voiced via TTS and shown as text



## Current Limitations

No facial animation or lip sync

No persistent memory across sessions

No error handling for poor or missing mic input

Only one NPC and one interaction context supported



