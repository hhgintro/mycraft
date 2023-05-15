using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using FactoryFramework;

public class MouseHoverTooltip : MonoBehaviour
{
    // requires some type of collider on this obj.
    // will not use RequiredComponent because one
    // gets added programatically in some cases in this demo
    [SerializeField] string message = "Message Goes Here.";
    DemoUIManager uiManager;

    private void Awake()
    {
        uiManager = GameObject.FindObjectOfType<DemoUIManager>();
        
    }

    protected virtual string DisplayMessage()
    {
        return message;
    }

    private void OnMouseEnter()
    {
        uiManager?.SetText(DisplayMessage());

        if (TryGetComponent(out PowerGridComponent pgc))
        {
            pgc.ShowRadiusVisual();
        }
    }
    private void OnMouseExit()
    {
        uiManager?.SetText("");

        if (TryGetComponent(out PowerGridComponent pgc))
        {
            pgc.HideRadiusVisual();
        }
    }
}
