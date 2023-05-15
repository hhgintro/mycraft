using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UIElements;

using FactoryFramework;

public class DemoUIManager : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;

    private Label msgLabel;

    public UnityAction<string> SetText;

    [SerializeField] private VisualTreeAsset resourceTreeAsset;

    private BuildingPlacement _buildingPlacement;
    private ConveyorPlacement _conveyorPlacement;
    private CableManagement _cablePlacement;

    private void Start()
    {
        _buildingPlacement = FindObjectOfType<BuildingPlacement>();
        _conveyorPlacement = FindObjectOfType<ConveyorPlacement>();
        _cablePlacement = FindObjectOfType<CableManagement>();

        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        VisualElement resource_container = _root.Q<VisualElement>("resources");

        VisualElement message_container = _root.Q<VisualElement>("messages");
        msgLabel = new Label();
        msgLabel.style.whiteSpace = WhiteSpace.Normal;
        message_container.Add(msgLabel);
        SetText += SetTextHandler;

        VisualElement placeables_container = _root.Q<VisualElement>("build-menu");
        var placeables = Resources.LoadAll<Placeable>("Buildings").ToList();
        placeables.Sort();
        foreach (Placeable def in placeables)
        {
            VisualElement elem = new VisualElement();
            elem.AddToClassList("build");

            elem.style.backgroundImage = new StyleBackground(def.icon);

            // figure out what to do on click
            elem.RegisterCallback<ClickEvent>((evt) =>
            {
                if (def.prefab == null)
                {
                    // the only null in our demo scene is the cable
                    _cablePlacement.BeginConnectionProcess();
                }
                else if (def.prefab.TryGetComponent(out Conveyor _))
                {
                    _conveyorPlacement.StartPlacingConveyor();
                } 
                else if (def.prefab.TryGetComponent(out Producer _))
                {
                    // in our demo, all producers require an ore deposit
                    _buildingPlacement.StartPlacingBuilding(def.prefab, true);
                }
                else if (def.prefab.TryGetComponent(out PowerGridComponent _) || def.prefab.TryGetComponent(out LogisticComponent _))
                {
                    // place building regularly
                    _buildingPlacement.StartPlacingBuilding(def.prefab, false);
                }
            });

            placeables_container.Add(elem);

            elem.RegisterCallback<MouseEnterEvent>((evt) =>
            {
                msgLabel.text = $"{def.name}";
            });
            
        }

        placeables_container.RegisterCallback<MouseLeaveEvent>((evt)=>
        {
            msgLabel.text = "";
        });

        // save load
        Button saveBtn = _root.Q<Button>("save");
        saveBtn.RegisterCallback<ClickEvent>((evt) =>
        {
            FindObjectOfType<SerializeManager>().Save();
        });
        Button loadBtn = _root.Q<Button>("load");
        loadBtn.RegisterCallback<ClickEvent>((evt) =>
        {
            FindObjectOfType<SerializeManager>().Load();
        });

    }

    private void OnDisable()
    {
        SetText -= SetTextHandler;
    }

    private void SetTextHandler(string txt)
    {
        msgLabel.text = txt;
    }


}
