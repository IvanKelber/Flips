using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioEvent : ScriptableObject
{
    public string Name;

    public abstract void Play(AudioSource source, float masterVolume);
    public virtual IEnumerator PlayAndWait(AudioSource source, float masterVolume) {
        Play(source, masterVolume);
        yield return new WaitForSeconds(source.clip.length);
    }
}
