using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TzarGPT
{
    [CustomEditor(typeof(ChatProfile))]
    public class ChatProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (ChatProfile)target;

            GUILayoutOption[] guiOptions = new GUILayoutOption[]
            { 
                GUILayout.Height(30),
                GUILayout.Width(120),
                GUILayout.MinHeight(30),
                GUILayout.MinWidth(120),
            };

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Fill Profile Values", guiOptions))
            {
                script.ParseProfileDataFromJSON();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

           
        }
    }
}