using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TzarGPT.Extensions
{
    public static class AudioExtensions
    {
        public static AudioClip Crop(this AudioClip sourceClip, float cropStart, float cropLength)
        {
            // Check if the source clip is not null
            if (sourceClip != null)
            {
                // Get the sample rate and bit depth of the source clip
                int frequency = sourceClip.frequency;
                int channels = sourceClip.channels;
                int bitDepth = sourceClip.samples / sourceClip.channels;

                // Calculate the start and end samples of the cropped AudioClip
                int cropStartSamples = Mathf.FloorToInt(cropStart * frequency);
                int cropEndSamples = Mathf.FloorToInt((cropStart + cropLength) * frequency);

                // Check if the end samples is greater than the total samples of the source clip
                if (cropEndSamples > sourceClip.samples)
                {
                    cropEndSamples = sourceClip.samples;
                }

                // Calculate the length of the cropped AudioClip in samples
                int cropLengthSamples = cropEndSamples - cropStartSamples;

                // Create a new float array to store the data of the cropped AudioClip
                float[] croppedData = new float[cropLengthSamples * channels];

                // Get the data of the source AudioClip
                float[] sourceData = new float[sourceClip.samples * channels];
                sourceClip.GetData(sourceData, 0);

                // Copy the data of the cropped AudioClip to the new float array
                System.Array.Copy(sourceData, cropStartSamples * channels, croppedData, 0, cropLengthSamples * channels);

                // Create a new AudioClip using the cropped data
                AudioClip croppedClip = AudioClip.Create(sourceClip.name + " (Cropped)", cropLengthSamples, channels, frequency, false);
                croppedClip.SetData(croppedData, 0);

                //// Play the cropped AudioClip
                //AudioSource.PlayClipAtPoint(croppedClip, transform.position);
                return croppedClip;
            }

            return null;
        }


        public static AudioClip RemoveSilentParts(this AudioClip audioClip, float threshold = 0.01f, int offset = 0)
        {
            // Get audio data
            float[] samples = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(samples, 0);
            //Debug.Log("Samples count in audio clip " + samples.Length);
            //Debug.Log("Output sample rate is " + AudioSettings.outputSampleRate);
            //Debug.Log("Audio clip sample rate is " + audioClip.frequency);

            // Create a buffer to hold non-silent samples
            List<float> buffer = new List<float>();

            int lastRecordedOffset = 0;

            // Iterate over the samples and add non-silent ones to the buffer
            for (int i = 0; i < samples.Length; i++)
            {
                if (Mathf.Abs(samples[i]) > threshold)
                {
                    // Apply offset if any
                    if (offset == 0)
                    {
                        //Debug.Log("Adding with offset 0");
                        buffer.Add(samples[i]);
                    }
                    else
                    {
                        // Keep track of the last recorded offset so we don't double recordings
                        //Debug.Log($"Adding offset value range {GetLowerOffset(i, offset, lastRecordedOffset)} to {(i + offset >= samples.Length ? samples.Length : i + offset)} for i = {i}");
                        // Go through each offset value before and after the current and add it no matter if it's silent or not
                        for (int x = GetLowerOffset(i, offset, lastRecordedOffset) /*(i - offset <= 0? 0 : i - offset)*/; x < (i + offset >= samples.Length ? samples.Length : i + offset); x++)
                        {
                            buffer.Add(samples[x]);
                            //lastRecordedOffset = x;
                        }

                        lastRecordedOffset = (i + offset >= samples.Length ? samples.Length : i + offset); // could add -1 to get the index
                                                                                                           //i = (i + offset >= samples.Length ? samples.Length : i + offset);
                        i = lastRecordedOffset;
                    }
                }
            }

            #region Debug
            //// Debug
            //float minDetected = float.MaxValue;
            //float maxDetected = 0f;

            //// Find min max
            //for (int i = 0; i < samples.Length; i++)
            //{
            //    if (Mathf.Abs(samples[i]) > maxDetected)
            //    {
            //        maxDetected = Mathf.Abs(samples[i]);
            //    }
            //}

            //for (int i = 0; i < samples.Length; i++)
            //{
            //    if (Mathf.Abs(samples[i]) < maxDetected && Mathf.Abs(samples[i]) != 0f)
            //    {
            //        minDetected = Mathf.Abs(samples[i]);
            //    }
            //}

            //Debug.Log("Min Detected " + minDetected);
            //DebugTextAccess.Instance.TextObjects[0].text = "Min Detected " + minDetected;
            //Debug.Log("Max Detected " + maxDetected);
            //DebugTextAccess.Instance.TextObjects[1].text = "Max Detected " + maxDetected;
            #endregion

            // If all samples are silent, return null
            if (buffer.Count == 0)
                return null;

            //Debug.Log("Reconstructed clip sample count " + buffer.Count);
            // Create a new AudioClip from the non-silent samples
            AudioClip reconstructedClip = AudioClip.Create("ReconstructedClip", buffer.Count /*/ audioClip.channels*/, audioClip.channels, audioClip.frequency, false);
            reconstructedClip.SetData(buffer.ToArray(), 0);

            return reconstructedClip;
        }

        static int GetLowerOffset(int i, int offset, int minAllowed)
        {
            // First check minAllowed
            if (i - offset <= minAllowed)
            {
                return minAllowed;
            }

            if (i - offset > minAllowed)
            {
                return i - offset;
            }

            // Later check if less than 0
            if (i - offset <= 0)
            {
                return 0;
            }

            if (i - offset > 0)
            {
                return i - offset;
            }

            return minAllowed;
        }

        public static AudioClip Merge(this AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
                return null;

            int length = 0;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                    length += clips[i].samples;
            }

            if (length == 0)
                return null;

            float[] data = new float[length];

            int position = 0;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] == null)
                    continue;

                float[] buffer = new float[clips[i].samples * clips[i].channels];
                clips[i].GetData(buffer, 0);

                for (int j = 0; j < buffer.Length; j++)
                {
                    data[position++] = buffer[j];
                }
            }

            AudioClip mergedClip = AudioClip.Create("MergedClip", length, 1, AudioSettings.outputSampleRate, false);
            mergedClip.SetData(data, 0);

            return mergedClip;
        }

        public static AudioClip Merge(this List<AudioClip> clips)
        {
            if (clips == null || clips.Count == 0)
                return null;

            int length = 0;
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i] != null)
                    length += clips[i].samples;
            }

            if (length == 0)
                return null;

            float[] data = new float[length];

            int position = 0;
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i] == null)
                    continue;

                float[] buffer = new float[clips[i].samples * clips[i].channels];
                clips[i].GetData(buffer, 0);

                for (int j = 0; j < buffer.Length; j++)
                {
                    data[position++] = buffer[j];
                }
            }

            AudioClip mergedClip = AudioClip.Create("MergedClip", length, 1, AudioSettings.outputSampleRate, false);
            mergedClip.SetData(data, 0);

            return mergedClip;
        }
    }

}