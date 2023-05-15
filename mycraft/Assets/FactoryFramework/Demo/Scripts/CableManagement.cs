using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryFramework;

public class CableManagement : MonoBehaviour
{
    public bool debugControls;

    private PowerGridComponent _source;
    private PowerGridComponent _destination;
    private bool selectionMode = false;

    [Header("Event Channels to Handle State")]
    public VoidEventChannel_SO startPlacementEvent;
    public VoidEventChannel_SO finishPlacementEvent;
    public VoidEventChannel_SO cancelPlacementEvent;

    [Header("Controls")]
    public KeyCode CancelKey = KeyCode.Escape;

    DemoUIManager uiManager;

    private void Awake()
    {
        uiManager = GameObject.FindObjectOfType<DemoUIManager>();
    }

    private void OnEnable()
    {
        if (cancelPlacementEvent) cancelPlacementEvent.OnEvent += CancelPlacement;
    }

    private void OnDisable()
    {
        if (cancelPlacementEvent) cancelPlacementEvent.OnEvent -= CancelPlacement;
    }

    public void BeginConnectionProcess()
    {
        selectionMode = true;
        startPlacementEvent?.OnEvent?.Invoke();
        Reset();
    }

    private void Reset()
    {
        VisualDeselectBuilding(_source);
        VisualDeselectBuilding(_destination);
        _source = null;
        _destination = null;
    }

    private void CancelPlacement()
    {
        Reset();
        selectionMode = false;
    }

    private void Update()
    {
        if (!selectionMode) return;

        // set text for hints
        if (_source == null)
        {
            uiManager.SetText("Power Cable Connect Mode: Click two buildings to connect them");
        } else
        {
            uiManager.SetText("Power Cable Connect Mode: Click another building to connect to the same power grid");
        }
        

        if (Input.GetMouseButtonDown(0))
        {
            foreach(var hit in Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)))
            {
                if (hit.collider.TryGetComponent(out PowerGridComponent pgc))
                {
                    ToggleSelect(pgc);
                    finishPlacementEvent?.OnEvent?.Invoke();
                }
            }
        }
    }

    private void ToggleSelect(PowerGridComponent pgc)
    {
        // deselect if necessary
        if (_source == pgc)
        {
            _source = null;
            VisualDeselectBuilding(pgc);
            return;
        }
        if (_destination == pgc)
        {
            _destination = null;
            VisualDeselectBuilding(pgc);
            return;
        }

        // select otherwise
        VisualSelectBuilding(pgc);
        if (_source == null) _source = pgc;
        else if (_destination == null) _destination = pgc;

        if (_source != null && _destination != null)
        {
            ConnectComponents();
        }
    }

    private void ConnectComponents()
    {
        _source.Connect(_destination);

        selectionMode = false;
        Reset();
    }

    public void VisualSelectBuilding(PowerGridComponent pgc)
    {
        if (pgc == null) return;
        if (debugControls) Debug.Log($"Select Building {pgc.name}");
        MeshRenderer mr = pgc.GetComponent<MeshRenderer>();
        Material mat = mr.material;
        if (mat.HasFloat("_Highlight"))
            mat.SetFloat("_Highlight", 1);
    }

    public void VisualDeselectBuilding(PowerGridComponent pgc)
    {
        if (pgc == null) return;
        if (debugControls) if (pgc == null) return;
        if (debugControls) Debug.Log($"Deselect Building {pgc.name}");
        MeshRenderer mr = pgc.GetComponent<MeshRenderer>();
        Material mat = mr.material;
        if (mat.HasFloat("_Highlight"))
            mat.SetFloat("_Highlight", 0);
    }

    private void OnGUI()
    {
        if (!debugControls) return;
        

        GUILayout.BeginArea(new Rect(0, 0, 200, 500));
        if (selectionMode)
        {
            if (GUILayout.Button("Cancel"))
            {
                Reset();
            }
            GUILayout.Label("Click 2 power grid components to connect");
        }
        else
        {
            if (GUILayout.Button("Cables"))
            {
                BeginConnectionProcess();
            }
        }
        GUILayout.EndArea();
    }
}
