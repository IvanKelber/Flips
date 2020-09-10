using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Audio Events/Increasing Pitch")]
public class IncreasingPitchAudioEvent : PitchAdjustAudioEvent
{
   
    [SerializeField, Range(-.2f,.2f)]
    private float increment;
    private float pitch;

    private void OnEnable() {
        pitch = 1 + modulation;
    }

    public override void Play(AudioSource source, float masterVolume) {

        source.clip = clip;
        source.volume = volume * masterVolume;
        source.pitch = pitch;
        source.Play();
        pitch += increment;
    }

    public void Reset() {
        pitch = 1 + modulation;
    }
}
