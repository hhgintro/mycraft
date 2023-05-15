using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using FactoryFramework;

public class TestGlobalPowerGrid : MonoBehaviour
{
    private CancellationTokenSource _cts;

    public GameObject PowerPolePrefab;
    public GameObject PowerTowerPrefab;
    public GameObject TurbinePrefab;
    public GameObject SolarPrefab;
    public GameObject CoalPlantPrefab;

    public int delay = 1000; // purely for visual

    private void Start()
    {
        _cts = new CancellationTokenSource();
        RunTests(_cts.Token);
    }

    async void RunTests(CancellationToken token)
    {
        try
        {
            await Wait(token);
            TestDirectConnection();
            await Wait(token);
            TestGraphUnion();
            await Wait(token);
            TestGraphDisjoint();
            await Wait(token);
            TestPowerTowerRadius();
            await Wait(token);
            TestCoalPlantProduction();
        } catch (System.OperationCanceledException) {
            Debug.Log("Tests Canceled");
        }
    }

    private void OnDestroy()
    {
        _cts.Cancel();
    }

    private async Task Wait(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await Task.Delay(delay);
        token.ThrowIfCancellationRequested();
    }

    private void TestDirectConnection()
    {
        GameObject powerPoleA = Instantiate(PowerPolePrefab, new Vector3(0f, 0f, 2.5f), Quaternion.identity);
        powerPoleA.name = "powerPoleA";
        GameObject powerPoleB = Instantiate(PowerPolePrefab, new Vector3(0f, 0f, -2.5f), Quaternion.identity);
        powerPoleB.name = "powerPoleB";

        PowerGridComponent connectionA = powerPoleA.GetComponent<PowerGridComponent>();
        PowerGridComponent connectionB = powerPoleB.GetComponent<PowerGridComponent>();

        connectionA.Connect(connectionB);
    }

    private void TestGraphUnion()
    {
        GameObject powerPoleC = Instantiate(PowerPolePrefab, new Vector3(3.5f, 0f, -2.5f), Quaternion.identity);
        powerPoleC.name = "powerPoleC";
        GameObject powerPoleD = Instantiate(PowerPolePrefab, new Vector3(3.5f, 0f, 2.5f), Quaternion.identity);
        powerPoleD.name = "powerPoleD";
        GameObject powerPoleE = Instantiate(PowerPolePrefab, new Vector3(7f, 0f, 2.5f), Quaternion.identity);
        powerPoleE.name = "powerPoleE";
        GameObject powerPoleF = Instantiate(PowerPolePrefab, new Vector3(10f, 0f, 2.5f), Quaternion.identity);
        powerPoleF.name = "powerPoleF";

        PowerGridComponent connectionC = powerPoleC.GetComponent<PowerGridComponent>();
        PowerGridComponent connectionD = powerPoleD.GetComponent<PowerGridComponent>();
        PowerGridComponent connectionE = powerPoleE.GetComponent<PowerGridComponent>();
        PowerGridComponent connectionF = powerPoleF.GetComponent<PowerGridComponent>();

        connectionC.Connect(connectionD);
        connectionD.Connect(connectionE);
        connectionE.Connect(connectionF);

        PowerGridComponent connectionA = GameObject.Find("powerPoleA").GetComponent<PowerGridComponent>();
        connectionC.Connect(connectionA);
        
    }

    private void TestGraphDisjoint()
    {
        PowerGridComponent connectionC = GameObject.Find("powerPoleC").GetComponent<PowerGridComponent>();
        Destroy(connectionC.gameObject);
    }

    private void TestPowerTowerRadius()
    {
        GameObject towerObj = Instantiate(PowerTowerPrefab, new Vector3(3.5f, 0f, -2.5f), Quaternion.identity);
        PowerGridComponent component = towerObj.GetComponent<PowerGridComponent>();
        component.Connect();
    }

    private void TestCoalPlantProduction()
    {
        PowerGridComponent connectionA = GameObject.Find("powerPoleA").GetComponent<PowerGridComponent>();
        PowerGridComponent CoalPlant = GameObject.Find("Power Plant").GetComponent<PowerGridComponent>();
        PowerGridComponent CoalMine = GameObject.Find("Drill Building").GetComponent<PowerGridComponent>();

        connectionA.Connect(CoalPlant);
        connectionA.Connect(CoalMine);

        // add in solar to power the miner until coal production starts
        GameObject solarPowerGenerator = Instantiate(SolarPrefab, new Vector3(-5f, 0f, -7.5f), Quaternion.identity);
        PowerGridComponent solarPGC = solarPowerGenerator.GetComponent<PowerGridComponent>();
        connectionA.Connect(solarPGC);
    }
}
