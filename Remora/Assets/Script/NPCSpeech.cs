using UnityEngine;

public class NPCSpeech : MonoBehaviour
{
    public AudioSource npcAudioSource;
    public AudioClip testClip; // Assign in Inspector or load dynamically
    
    void Start()
    {
        GetComponent<NPCSpeech>().Speak("Hello, welcome to the shop!");
    }


    public void Speak(string text)
    {
        Debug.Log("NPC is saying: " + text);

        if (npcAudioSource != null && testClip != null)
        {
            npcAudioSource.Stop();
            npcAudioSource.clip = testClip; // replace with generated clip
            npcAudioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip missing.");
        }
    }
}
