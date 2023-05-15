using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace FactoryFramework
{
    [DisallowMultipleComponent]
    public class SerializationReference : MonoBehaviour
    {
        protected Guid _guid = Guid.Empty;

        public Guid GUID
        {
            get
            {
                if (_guid.Equals(Guid.Empty))
                    _guid = Guid.NewGuid();
                return _guid;
            }
            set
            {
                _guid = value;
            }
        }


        [SerializeField, HideInInspector]
        public string resourcesPath = "";

#if UNITY_EDITOR
        static Regex rx = new Regex(@".*Resources/(.*)\.prefab");

        private void OnValidate()
        {
            if (PrefabUtility.IsPartOfAnyPrefab(this))
            {
                // find the path from resources to this obj
                MatchCollection matches = rx.Matches(AssetDatabase.GetAssetPath(this));
                if (matches.Count > 0)
                {
                    resourcesPath = matches[0].Groups[1].Value;
                }
            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SerializationReference))]
    public class SerializationReferenceEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            SerializationReference sRef = (SerializationReference)target;

            // Create a new VisualElement to be the root of our inspector UI
            VisualElement root = new VisualElement();

            // Add readonly labels
            var guidText = new TextField(label: "GUID");
            guidText.value = sRef.GUID.ToString();
            guidText.SetEnabled(false);
            root.Add(guidText);

            var pathText = new TextField(label: "Prefab Path");
            pathText.value = sRef.resourcesPath;
            pathText.SetEnabled(false);
            root.Add(pathText);
            // go to reference on click
            Button showPrefabBtn = new Button(() => EditorGUIUtility.PingObject(Resources.Load(sRef.resourcesPath)));
            showPrefabBtn.text = "Go To Prefab";
            root.Add(showPrefabBtn);

            // Attach a default inspector to the foldout
            //InspectorElement.FillDefaultInspector(root, serializedObject, this);

            // Return the finished inspector UI

            return root;
        }

    }
#endif
}