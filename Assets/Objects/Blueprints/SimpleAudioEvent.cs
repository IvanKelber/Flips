using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    public AudioClip clip;

    [Range(0,1)]
    public float volume;

    public override void Play(AudioSource source, float masterVolume) {
        if(source == null) {
            Debug.LogWarning("Source is null");
            return;
        }
        if(clip == null) {
            Debug.LogWarning("AudioClip " + Name + " has no clip");
            return;
        }
        source.volume = volume * masterVolume;
        source.pitch = 1;
        source.clip = clip;
        source.Play();
    }
}
