using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace FactoryFramework.Editor
{
    [CustomEditor(typeof(SerializeManager))]
    public class SerializeManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Save"))
            {
                (target as SerializeManager).Save();
            }
            if (GUILayout.Button("Load"))
            {
                (target as SerializeManager).Load();
            }
            DrawDefaultInspector();
        }
    }

}

