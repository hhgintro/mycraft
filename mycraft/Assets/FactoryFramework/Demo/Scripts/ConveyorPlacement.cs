using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FactoryFramework;
public class ConveyorPlacement : MonoBehaviour
{
    [Header("Placement Events")]
    public VoidEventChannel_SO startPlacementEvent;
    public VoidEventChannel_SO finishPlacementEvent;
    public VoidEventChannel_SO cancelPlacementEvent;

    [Header("Conveyor Setup")]
    public Conveyor conveyorPrefab;
    private Conveyor current;

    private Vector3 startPos;
    private float startHeight;
    private Vector3 endPos;
    private Vector3 flatEndPos;
    private Vector2 shiftMousePos;

    private Socket startSocket;
    private Socket endSocket;

    [Header("Visual Feedback Materials")]
    public Material originalFrameMat;
    public Material originalBeltMat;
    public Material greenGhostMat;
    public Material redGhostMat;

    [Header("Controls")]
    public KeyCode cancelKey = KeyCode.Escape;

    private enum State
    {
        None,
        Start,
        End
    }
    [SerializeField] private State state;

    private void OnEnable()
    {
        // listen to the cancel event to force cancel placement from elsewhere in the code
        cancelPlacementEvent.OnEvent += ForceCancel;
    }
    private void OnDisable()
    {
        // stop listening
        cancelPlacementEvent.OnEvent -= ForceCancel;
    }

    private void ForceCancel()
    {
        if (current != null)
        {
            Destroy(current.gameObject);
        }
        current = null;
        this.state = State.None;
    }

    private bool TryChangeState(State desiredState)
    {
        state = desiredState;
        return true;
    }

    public void StartPlacingConveyor() {
        //cancel any placement currently happening
        cancelPlacementEvent?.Raise();
        // instantiate a belt to place
        current = Instantiate(conveyorPrefab);
        if (TryChangeState(State.Start))
        {
            startSocket = null;
            endSocket = null;
        }
        // trigger event
        startPlacementEvent?.Raise();
    }

    private bool ValidLocation()
    {
        if (current == null) return false;
        
            foreach (Collider c in Physics.OverlapSphere(startPos, 1f))
            {
                if (c.tag == "Building" && c.gameObject != current.gameObject)
                {
                    // colliding something!
                    if (ConveyorLogisticsUtils.settings.SHOW_DEBUG_LOGS)
                        Debug.LogWarning($"Invalid placement: {current.gameObject.name} collides with {c.gameObject.name} at the start");
                    //ChangeMatrerial(redPlacementMaterial);
                    return false;
                }
            }
            foreach (Collider c in Physics.OverlapSphere(endPos, 1f))
            {
                if (c.tag == "Building" && c.gameObject != current.gameObject)
                {
                    // colliding something!
                    if (ConveyorLogisticsUtils.settings.SHOW_DEBUG_LOGS)
                        Debug.LogWarning($"Invalid placement: {current.gameObject.name} collides with {c.gameObject.name} at the end");
                    //ChangeMatrerial(redPlacementMaterial);
                    return false;
                }
            }

        //ChangeMatrerial(greenPlacementMaterial);
        return true;
    }
    void HandleStartState()
    {
        Debug.Assert(current != null, "Not currently placing a conveyor.");
        startSocket = null;
        Vector3 worldPos = Vector3.zero;
        Vector3 worldDir = Vector3.forward;
        Ray mousedownRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach (RaycastHit hit in Physics.RaycastAll(mousedownRay, 100f))
        {
            // skip objects/colliders in the conveyor we're currently placing
            if (hit.transform.root == current.transform) continue;
            // try to find an open socket
            if (hit.collider.gameObject.TryGetComponent(out Socket socket))
            {
                if (!socket.IsOpen())
                {
                    // Socket already Occupied
                    continue;
                }
                
                startSocket = socket;
                break;
            }
            
            if (hit.collider.gameObject.TryGetComponent(out Terrain t))
            {
                
                worldPos = hit.point;
                Vector3 camForward = Camera.main.transform.forward;
                camForward.y = 0f;
                camForward.Normalize();
                worldDir = camForward;
            }
        }
        // override placement if we found a valid socket
        if (startSocket)
        {
            worldPos = startSocket.transform.position;
            worldDir = startSocket.transform.forward;
        }

        startPos = worldPos;
        // setup the start and end vectors to solve for path building
        current.data.start = worldPos;
        current.data.startDir = worldDir;
        current.data.end = worldPos + worldDir;
        current.data.endDir = worldDir;
        // get the relative height
        if (startSocket == null)
        {
            startHeight = 0f; // Terrain.activeTerrain.SampleHeight(worldPos);
        }
        else
        {
            startHeight = startSocket.transform.position.y - Terrain.activeTerrain.SampleHeight(worldPos);
        }

        //ignore colliders from start and end points
        List<Collider> collidersToIgnore = new List<Collider>();
        // add colliders associated with the connected start socket
        if (startSocket != null)
        {
            collidersToIgnore.AddRange(startSocket.transform.root.GetComponentsInChildren<Collider>());
            collidersToIgnore.Remove(startSocket.transform.root.GetComponent<Collider>());
        } else
        {
            collidersToIgnore.Add(Terrain.activeTerrain.GetComponent<TerrainCollider>());
        }
        
        //if (collidersToIgnore.Count > 0)
        current.UpdateMesh(ignored: collidersToIgnore.ToArray(), startskip:1, endskip: 1);
        //else
        //    current.UpdateMesh();

        // startSocket != null prevents belt from starting disconnected
        if (current.ValidMesh) // && startSocket != null
        {
            current.SetMaterials(greenGhostMat, greenGhostMat);
            if (Input.GetMouseButtonDown(0))
            {
                TryChangeState(State.End);
            }
        }
        else 
            current.SetMaterials(redGhostMat, redGhostMat);

        
    }

    void HandleEndState()
    {
        Debug.Assert(current != null, "Not currently placing a conveyor.");
        Vector3 worldPos = Vector3.zero;
        Vector3 worldDir = Vector3.forward;
        Ray mousedownRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach (RaycastHit hit in Physics.RaycastAll(mousedownRay, 100f))
        {
            if (hit.collider.transform.root == current.transform) continue;
            // want to specifically connect to a conveyor socket, not a belt bridge
            if (hit.collider.gameObject.TryGetComponent<InputSocket>(out InputSocket socket))
            {
                if (!socket.IsOpen())
                {
                    // Socket already Occupied
                    break;
                }
                worldPos = hit.collider.transform.position;
                worldDir = hit.collider.transform.forward;
                endSocket = socket;

                break;
            }
            if (hit.collider.gameObject.TryGetComponent<Terrain>(out Terrain t))
            {
                // handle height offset when holding shift
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // find the intersection of the camera mouse ray plane and the endPos->Vector.Up line
                    Vector3 planeNormal = Camera.main.transform.up;

                    Vector3 lineStart = flatEndPos;
                    Vector3 lineVector = Vector3.up;

                    float dotNumerator = Vector3.Dot((hit.point - lineStart), planeNormal);
                    float dotDenominator = Vector3.Dot(lineVector, planeNormal);

                    if (dotDenominator != 0.0f)
                    {
                        var length = dotNumerator / dotDenominator;
                        Vector3 vec = Vector3.up * length;
                        worldPos = lineStart + vec;
;                    } else
                    {
                        worldPos = flatEndPos;
                    }

                    worldPos.y = Mathf.Max(Terrain.activeTerrain.SampleHeight(worldPos), worldPos.y);
                } else
                {
                    worldPos = hit.point;
                    // stay same level if this is the terrain

                    worldPos.y = Terrain.activeTerrain.SampleHeight(worldPos) + startHeight;
                    
                }

                Vector3 camForward = Camera.main.transform.forward;
                camForward.y = 0f;
                camForward.Normalize();
                worldDir = camForward;
                // reset socket
                endSocket = null;

            }
            
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            flatEndPos = endPos;
            shiftMousePos = Input.mousePosition;
        }
        endPos = worldPos;
        current.data.end = worldPos;
        current.data.endDir = worldDir;
        List<Collider> collidersToIgnore = new List<Collider>();
        //add colliders associated with the connected start and end sockets
        //THIS IS NOT A GREAT WAY TO DO THIS - CONSIDER USING LAYERMASKS
        //if (startSocket == null)
        //    collidersToIgnore.AddRange(FindObjectsOfType<TerrainCollider>());
        if (startSocket != null)
            collidersToIgnore.AddRange(startSocket.GetComponentsInChildren<Collider>());
        if (endSocket != null)
            collidersToIgnore.AddRange(endSocket.GetComponentsInChildren<Collider>());
        // add self
        OutputSocket outputSocket = current.GetComponentInChildren<OutputSocket>();
        if (outputSocket != null)
            collidersToIgnore.Add(outputSocket.GetComponent<Collider>());
        InputSocket inputSocket = current.GetComponentInChildren<InputSocket>();
        if (inputSocket != null)
            collidersToIgnore.Add(inputSocket.GetComponent<Collider>());
        // add connected sockets
        if (startSocket)
            collidersToIgnore.Add(startSocket.GetComponent<Collider>());
        if (endSocket)
            collidersToIgnore.Add(endSocket.GetComponent<Collider>());

        current.UpdateMesh(
            startskip: 1, //startSocket != null ? 1 : 0, 
            endskip: 1,
            ignored: collidersToIgnore.Count > 0 ? collidersToIgnore.ToArray() : null
        );

        if (current.ValidMesh)
            current.SetMaterials(greenGhostMat, greenGhostMat);
        else
            current.SetMaterials(redGhostMat, redGhostMat);

        //coordinate
        MyCraft.Managers.Coordinates.DrawCoordinate(worldPos);

        if (Input.GetMouseButtonDown(0) && current.ValidMesh)
        {

            // change the sockets!
            if (startSocket != null)
            {
                //startSocket.Connect(current);
                current.ConnectToOutput(startSocket as OutputSocket);
            }
            if (endSocket != null)
            {
                //endSocket.Connect(current);
                current.ConnectToInput(endSocket as InputSocket);
            }
            // finalize the conveyor
            current.UpdateMesh(true);
            current.SetMaterials(originalFrameMat, originalBeltMat);
            current.AddCollider();

            // stop placing conveyor
            current = null;
            startSocket = null;
            endSocket = null;

            TryChangeState(State.None);
            finishPlacementEvent?.Raise();
        }
    }

    void HandleNoneState()
    {
        // right click to delete
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            foreach (RaycastHit hit in Physics.RaycastAll(ray, 100f))
            {
                if (hit.collider.transform.root.TryGetComponent<Conveyor>(out Conveyor conveyor))
                {
                    conveyor.Disconnect();
                    Destroy(conveyor.gameObject);
                    return;
                }
            }
        }
        return;
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(cancelKey))
        {
            if (current != null)
                Destroy(current.gameObject);
            current = null;
            startSocket = null;
            endSocket = null;
            state = State.None;
        }
        switch (state)
        {
            case State.None:
                HandleNoneState();
                break;
            case State.Start:
                HandleStartState();
                break;
            case State.End:
                HandleEndState();
                break;
        }
    }

}
