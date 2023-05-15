using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using FactoryFramework;

namespace FactoryFramework.Editor
{
    [CustomEditor(typeof(PowerGridComponent))]
    public class PowerGridComponentEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            PowerGridComponent current = (PowerGridComponent)target;

            // Create a new VisualElement to be the root of our inspector UI
            VisualElement root = new VisualElement();

            // Attach a default inspector to the foldout
            InspectorElement.FillDefaultInspector(root, serializedObject, this);


            if (Application.isPlaying)
            {
                // container for connect button
                VisualElement connectContainer = new VisualElement();
                //connectContainer.style.width = Length.Percent(100);
                connectContainer.style.flexDirection = FlexDirection.Row;
                connectContainer.style.alignItems = Align.Stretch;
                connectContainer.style.justifyContent = Justify.SpaceBetween;

                ObjectField powerGridComponentField = new ObjectField("Connect To");
                powerGridComponentField.style.flexGrow = 1f;
                powerGridComponentField.allowSceneObjects = true;
                powerGridComponentField.objectType = typeof(PowerGridComponent);
                powerGridComponentField.RegisterCallback<ChangeEvent<PowerGridComponent>>(evt =>
                {
                    if (evt.newValue == current)
                    {
                        powerGridComponentField.SetValueWithoutNotify(evt.previousValue);
                    }
                });
                connectContainer.Add(powerGridComponentField);

                Button connectButton = new Button();
                connectButton.style.backgroundColor = new StyleColor(new Color(69 / 255f, 110 / 255f, 54 / 255f));
                connectButton.text = "Connect";
                connectButton.RegisterCallback<ClickEvent>(evt =>
                {
                    current.Connect((PowerGridComponent)powerGridComponentField.value);
                });
                connectContainer.Add(connectButton);

                root.Add(connectContainer);
            } else
            {
                Label l = new Label("Open this in Play Mode to see full inspector!");
                l.style.backgroundColor = new StyleColor(new Color(107 / 255f, 44 / 255f, 40 / 255f));
                root.Add(l);
            }

            // Return the finished inspector UI
            return root;
        }

    }
}