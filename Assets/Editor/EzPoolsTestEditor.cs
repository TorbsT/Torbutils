using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Test;

namespace Editors
{
    [CustomEditor(typeof(EzPoolsTest))]
    public class EzPoolsTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EzPoolsTest test = (EzPoolsTest)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Depool"))
            {
                //test.Depool();
            }
            if (GUILayout.Button("Enpool"))
            {
                //test.Enpool();
            }
        }
    }

}
