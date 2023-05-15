using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using FactoryFramework;

namespace FactoryFramework.Editor
{
    [CustomEditor(typeof(Conveyor))]
    public class ConveyorBeltEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            Conveyor conv = (Conveyor)target;

            // Create a new VisualElement to be the root of our inspector UI
            VisualElement root = new VisualElement();


            var foldout = new Foldout();
            root.Add(foldout);

            // Attach a default inspector to the foldout
            InspectorElement.FillDefaultInspector(foldout, serializedObject, this);

            VisualElement connectBox = new VisualElement();
            connectBox.AddToClassList("unity-proprty-field");
            //connectBox.style.backgroundColor = new StyleColor();
            //connectBox.style.width = new StyleLength(Length.Percent(100));
            connectBox.style.height = 150;
            //connectBox.style.borderBottomColor = (Color)new Color32(36, 36, 36, 255);
            //connectBox.style.borderBottomWidth = 1;
            //connectBox.style.borderTopColor = (Color)new Color32(36, 36, 36, 255);
            //connectBox.style.borderTopWidth = 1;

            ObjectField start = new ObjectField("Connect From");
            start.objectType = typeof(OutputSocket);
            ObjectField end = new ObjectField("Connect To");
            end.objectType = typeof(InputSocket);
            connectBox.Add(start);
            connectBox.Add(end);
            Button connectBtn = new Button();
            connectBtn.text = "Connect";
            connectBtn.clicked += () =>
            {
                conv.ConnectToInput(end.value as InputSocket);
                conv.ConnectToOutput(start.value as OutputSocket);
                
                conv.UpdateMesh(true);
                //conv.SetMaterials(originalFrameMat, originalBeltMat);
                conv.AddCollider();

                EditorUtility.SetDirty(conv);
                EditorUtility.SetDirty(end.value);
                EditorUtility.SetDirty(start.value);
            };
            root.Add(connectBtn);

            root.Add(connectBox);

            Button clearBtn = new Button();
            clearBtn.text = "Clear";
            clearBtn.clicked += () =>
            {
                conv.Reset();
                EditorUtility.SetDirty(conv);
            };
            root.Add(clearBtn);


            // Return the finished inspector UI
            return root;
        }

    }
}