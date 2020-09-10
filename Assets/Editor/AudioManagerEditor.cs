using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

[CustomEditor(typeof(AudioManager), true)]
public class AudioManagerEdtior : Editor
{

	[SerializeField] private AudioSource _previewer;

	int _choiceIndex = 0;


	public void OnEnable()
	{
		_previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
	}

	public void OnDisable()
	{
		DestroyImmediate(_previewer.gameObject);
	}

	private string[] GetEventNames(List<AudioEvent> events) {
		string[] names = new string[events.Count];
		for(int i = 0; i < events.Count; i++) {
			names[i] = events[i].Name;
		}
		return names;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		AudioManager manager = target as AudioManager;
		if(!manager.Enabled) {
			manager.Refresh();
		}
		_choiceIndex = EditorGUILayout.Popup("Preview Clip", _choiceIndex, GetEventNames(manager.events));

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
		if (GUILayout.Button("Preview"))
		{
			manager.Play(manager.events[_choiceIndex].Name, _previewer);

		}
		EditorGUI.EndDisabledGroup();

	}
}