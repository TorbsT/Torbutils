using System.Collections;
using System.Collections.Generic;
using TorbuTils.Audi;
using UnityEditor;
using UnityEngine;

namespace TorbuTils.Audio.Edit
{
    [CustomEditor(typeof(SoundObject))]
    public class SoundEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Get a reference to the serialized object
            serializedObject.Update();

            // Draw all default fields except the ones we customize
            SerializedProperty property = serializedObject.GetIterator();
            property.NextVisible(true); // Skip the script field

            while (property.NextVisible(false))
            {
                if (property.name == "useMultipleClips" || property.name == "singleClip" || property.name == "multipleClips")
                    continue;

                EditorGUILayout.PropertyField(property, true);
            }

            // Custom toggle and conditional fields
            var audioClipToggle = (SoundObject)target;

            audioClipToggle.UseMultipleClips = EditorGUILayout.Toggle("Use Multiple Clips", audioClipToggle.UseMultipleClips);

            if (audioClipToggle.UseMultipleClips)
            {
                SerializedProperty multipleClipsProperty = serializedObject.FindProperty("multipleClips");
                EditorGUILayout.PropertyField(multipleClipsProperty, new GUIContent("Multiple Clips"), true);
            }
            else
            {
                SerializedProperty singleClipProperty = serializedObject.FindProperty("singleClip");
                EditorGUILayout.PropertyField(singleClipProperty, new GUIContent("Single Clip"));
            }

            // Apply changes to serialized properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}