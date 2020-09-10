using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Audio Events/Pitch Adjust")]
public class PitchAdjustAudioEvent : SimpleAudioEvent
{
    [Range(-1,1)]
    public float modulation;

    protected void OnEnable() {
    }

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
        source.pitch = 1 + modulation;
        source.clip = clip;
        source.Play();
    }
}
