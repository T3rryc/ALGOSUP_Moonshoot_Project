using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
//using System.Diagnostics;
using UnityEngine.UI;
using TzarGPT.Extensions;

#if !UNITY_WEBGL

namespace TzarGPT
{
    public class MicrophoneRecorder : MonoBehaviour
    {
        public static MicrophoneRecorder Instance;

        [SerializeField] bool removeSilentParts = true;
        [SerializeField] float silenceThreshold = 0.01f;
        [SerializeField] float silenceOffset = 0.1f;

        private AudioClip recording;
        [SerializeField] Button recordButton;
        [SerializeField] WhisperAgent whisperAgent;
        [SerializeField] GPTDemoChat gptDemoChat;

        System.Diagnostics.Stopwatch recordingStopwatch = new System.Diagnostics.Stopwatch();
        string currentMicrophoneName;

        private string recordingPath;

        // length of recording in seconds, this will be the maximum recording length
        // the recording will get cropped to actual recording length
        [SerializeField] int maxRecordingLength = 30; 
        [SerializeField] float readOnlyCurrentThreshold;

        public bool IsRecording { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            recordingPath = Path.Combine(Application.temporaryCachePath, "recording.wav"); // temporary storage path

            // Set microphone to first microphone, you can implement microphone selection by using Microphone.devices array
            currentMicrophoneName = Microphone.devices[0];
            //Debug.Log("Mic name " + currentMicrophoneName);

        }

        public void SetRecordButtonVisibility(bool visible)
        {
            if (visible)
            {
                recordButton.gameObject.SetActive(true);
            }
            else
            {
                recordButton.gameObject.SetActive(false);
            }
        }

        public void StartRecording()
        {
            recordingStopwatch.Reset();
            recordingStopwatch.Start();

            IsRecording = true;

            recording = Microphone.Start(currentMicrophoneName, false, maxRecordingLength, AudioSettings.outputSampleRate); // start recording
                                                                                                           // Play a sound effect on recording start here
            Invoke(nameof(StopRecording), maxRecordingLength);
        }

        [SerializeField] bool testRecordingQualityOnly;

        public async void StopRecording()
        {
            recordingStopwatch.Stop();
            IsRecording = false;

            // Hide button and show it again when the answer has appeared
            if (!testRecordingQualityOnly)
                SetRecordButtonVisibility(false);

            // Play a sound effect on recording stop here

            CancelInvoke(nameof(StopRecording));

            Microphone.End(currentMicrophoneName); // stop recording

            // Get the recording time and crop
            //UnityEngine.Debug.Log("Recording time was " + recordingStopwatch.Elapsed.TotalSeconds);
            recording = recording.Crop(0f, (float)recordingStopwatch.Elapsed.TotalSeconds);

            // Remove silence from recording under a certain threshold, comment this out if it doesn't
            // work well for you or adjust values if necessary
            // This is used to optimize recording length to be sent to Whisper to be the shortest possible length
            if(removeSilentParts)
                recording = recording.RemoveSilentParts(silenceThreshold, (int)(silenceOffset * AudioSettings.outputSampleRate));

            SavWav.Save(recordingPath, recording); // save recording to temporary storage

            // Go forward with AI services
            if (!testRecordingQualityOnly)
                await ForwardRecordingToWhisper(recordingPath);

            recording = null; // clear recording from memory
        }

        async Task ForwardRecordingToWhisper(string recordingPath)
        {
            // Send the recording to OpenAI Whisper
            string whisperResult = await whisperAgent.UploadAudioAsync(recordingPath);

            // Check if whisper result is valid text
            whisperResult = whisperResult.Trim();
            if (whisperResult.Length <= 0)
            {
                return;
            }

            // Send the text result to ChatGPT for a response and display it in the text box
            string gptResponse = await gptDemoChat.GetResponseFromAIModel(whisperResult);

            // Show the button so we can record again
            SetRecordButtonVisibility(true);
        }

        public string GetRecordingPath()
        {
            return recordingPath;
        }
    }
}
#endif
