using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using FactoryFramework;

namespace FactoryFramework.Editor
{
    [CustomEditor(typeof(LogisticComponent))]
    public class LogisticComponentEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our inspector UI
            VisualElement myInspector = new VisualElement();

            // Add a simple label
            myInspector.Add(new Label("This is a custom inspector"));

            // Return the finished inspector UI
            return myInspector;
        }

    }
}