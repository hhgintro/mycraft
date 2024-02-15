using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FactoryFramework;

/// <summary>
/// Change a mesh renderer material's color to match the power draw efficiency
/// yellow = not enough power
/// red = no power
/// disappear on full power
/// </summary>
public class PowerEfficiencyToColor : MonoBehaviour
{
    private LogisticComponent _logisticComponent;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _logisticComponent = GetComponentInParent<LogisticComponent>();
        _meshRenderer = GetComponent<MeshRenderer>();

        var _pgc = GetComponentInParent<PowerGridComponent>();
        if (_pgc == null)
        {
            // disable this if you dont use power
            gameObject.SetActive(false);
        }
    }

    public void SetOwner(LogisticComponent owner) { _logisticComponent = owner; }

    private void Update()
    {
		float efficiency = (_logisticComponent)?_logisticComponent.PowerEfficiency:1f;
        // if we're at max efficiency dont display the sumbol
        if (efficiency == 1f)
        {
            _meshRenderer.enabled = false;
            return;
        }

        Color c = Color.clear;
        if (efficiency == 0)        c = Color.red;
        else if (efficiency < 1)    c = Color.yellow;

        _meshRenderer.material.color = c;
        _meshRenderer.enabled = true;
    }
}
