using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using FactoryFramework;

namespace FactoryFramework.Editor
{
    [CustomEditor(typeof(Producer))]
    public class ProducerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our inspector UI
            VisualElement root = new VisualElement();

            // Add a simple label
            root.Add(new Label("This is a custom inspector"));

            // Attach a default inspector to the foldout
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            // Return the finished inspector UI
            return root;
        }

    }
}