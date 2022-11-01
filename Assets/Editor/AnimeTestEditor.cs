using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Test;

namespace Editors
{
    [CustomEditor(typeof(AnimeTest))]
    public class AnimeTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AnimeTest test = (AnimeTest)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Run"))
            {
                test.Test();
            }
        }
    }

}
