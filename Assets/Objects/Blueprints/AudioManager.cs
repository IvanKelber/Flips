using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[CreateAssetMenu(menuName="Audio Events/Manager")] 
public class AudioManager : ScriptableObject
{

    public List<AudioEvent> events;
    private Dictionary<string, AudioEvent> eventsMap = new Dictionary<string, AudioEvent>();

    public bool Enabled {
        get {
            return events.Count == eventsMap.Count;
        }
    }

    private void OnEnable() {
        foreach(AudioEvent audioEvent in events) {
            eventsMap.Add(audioEvent.Name, audioEvent);
        }
    }

    public void Refresh() {
        foreach(AudioEvent audioEvent in events) {
            if(!eventsMap.ContainsKey(audioEvent.Name)) {
                eventsMap.Add(audioEvent.Name, audioEvent);
            }
        }
    }

    [Range(0,1)]
    public float volume;

    public void Play(string eventName, AudioSource source) {
        if(eventsMap.ContainsKey(eventName)) {
            eventsMap[eventName].Play(source, volume);
        }
    }

    public IEnumerator PlayAndWait(string eventName, AudioSource source) {
        if(eventsMap.ContainsKey(eventName)) {
            yield return eventsMap[eventName].PlayAndWait(source, volume);  
        } else {
            yield return null;
        }
    }

    public void Reset(string eventName) {
        if(eventsMap.ContainsKey(eventName) && eventsMap[eventName] is IncreasingPitchAudioEvent) {
            (eventsMap[eventName] as IncreasingPitchAudioEvent).Reset();
        }
    }

}
