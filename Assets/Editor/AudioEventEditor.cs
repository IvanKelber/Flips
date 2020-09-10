using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AudioEvent), true)]
public class AudioEventEditor : Editor
{

	[SerializeField] private AudioSource _previewer;

	public void OnEnable()
	{
		_previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
	}

	public void OnDisable()
	{
		DestroyImmediate(_previewer.gameObject);
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
		if (GUILayout.Button("Preview"))
		{
			((AudioEvent) target).Play(_previewer, 1);
		}
        IncreasingPitchAudioEvent ip = target as IncreasingPitchAudioEvent;
        if(ip != null) {
            if(GUILayout.Button("Reset")) {
                ip.Reset();
            }
        }
		EditorGUI.EndDisabledGroup();

	}
}