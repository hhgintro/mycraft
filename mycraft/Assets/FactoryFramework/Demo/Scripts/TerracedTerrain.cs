using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

#if UNITY_EDITOR
using UnityEditor;
// create a custom inspector just to have a button to regenerate on demand
[CustomEditor(typeof(TerracedTerrain))]
public class TerracedTerrainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        TerracedTerrain script = target as TerracedTerrain;
        if (GUILayout.Button("Regenerate")){script.Generate();}
    }
}
#endif

[RequireComponent(typeof(Terrain))]
public class TerracedTerrain : MonoBehaviour
{
    // seed is really just x,z offset in the noise
    public int seed = 0;
    public bool autoRandomSeed = true;
    public bool terrace = true;
    // number of points to use across the terrain
    public int resolution = 32;
    // max height of terrain
    public int height = 16;
    // number of terraces to force
    public int terraces = 4;
    // noise frequency factor
    public int frequency = 20;

    private Vector3[,] _coords;
    private Terrain _terrain;
    private TerrainData _terrainData;

    protected enum NoiseType
    {
        Perlin,
        cnoise,
        cellular,
        snoise,
        srnoise,
        srdnoise
    }
    [SerializeField] protected NoiseType noiseType;

    private void OnDrawGizmosSelected()
    {
        //if (_coords == null) return;
        //foreach(Vector3 coord in _coords)
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawSphere(CoordToTerrainPoint(coord), .25f);
        //}
        //// triangulate
        //for (int x = 0; x < resolution-1; x++)
        //{
        //    for (int z = 0; z< resolution -1; z++)
        //    {
        //        Gizmos.DrawLine(CoordToTerrainPoint(_coords[x, z]), CoordToTerrainPoint(_coords[x, z+1]));
        //        Gizmos.DrawLine(CoordToTerrainPoint(_coords[x, z]), CoordToTerrainPoint(_coords[x+1, z]));
        //        Gizmos.DrawLine(CoordToTerrainPoint(_coords[x+1, z]), CoordToTerrainPoint(_coords[x, z+1]));
        //    }
        //}
        
    }

    /// <summary>
    /// Convert grid of resolution x resolution to terrainData heightmap points
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    private Vector3 CoordToTerrainPoint(Vector3 vec)
    {
        Vector3 scaled = new Vector3(
            vec.x * (_terrainData.size.x) / resolution,
            vec.y * height,
            vec.z * (_terrainData.size.z) / resolution
            );
        return _terrain.transform.TransformPoint(scaled);
    }

    // Start is called before the first frame update
    void Start()
    {
        _terrain = GetComponent<Terrain>();
        _terrainData = _terrain.terrainData;
        _terrainData.size = new Vector3(_terrainData.size.x, height, _terrainData.size.z);

        Generate();
    }

    public void Generate()
    {
        _terrain = GetComponent<Terrain>();
        _terrainData = _terrain.terrainData;
        _terrainData.size = new Vector3(_terrainData.size.x, height, _terrainData.size.z);

        _terrainData.alphamapResolution = _terrainData.heightmapResolution;

        CreateCoords();
        UpdateTerrainHeight();
    }

    private void CreateCoords()
    {
        _coords = new Vector3[(resolution + 1), (resolution + 1)];

        var coordinates = new NativeArray<float2>(resolution*resolution, Allocator.Persistent);
        var heights = new NativeArray<float>(resolution*resolution, Allocator.Persistent);

        for (int i = 0; i < resolution * resolution; i++)
        {
            int x = i % resolution;
            int z = (int)(i / resolution);
            coordinates[i] = new float2(x, z);
        }

        var job = new HeightJob()
        {
            coordinates=coordinates,
            heights=heights,
            frequency=frequency,
            terraces=terraces,
            seed=seed,
            gridResolution=resolution,
            terrainResolution=(int)_terrainData.size.x,
            noiseType=noiseType,
            terrace = terrace
        };
        JobHandle jobHandle = job.Schedule(resolution * resolution, resolution/2);
        jobHandle.Complete();

        for (int i =0; i < resolution*resolution; i++)
        {
            int x = i % resolution;
            int z = (int)(i / resolution);
            float2 xz = job.coordinates[i];
            _coords[x, z] = new Vector3(xz.x, job.heights[i], xz.y);
        }

        coordinates.Dispose();
        heights.Dispose();

        if (autoRandomSeed)
            seed = UnityEngine.Random.Range(0, 10000);
    }

    private void UpdateTerrainHeight()
    {
        float[,] heights = _terrainData.GetHeights(0,0,_terrainData.heightmapResolution, _terrainData.heightmapResolution);
        float[,,] alphas = _terrainData.GetAlphamaps(0, 0, _terrainData.alphamapResolution, _terrainData.alphamapResolution);
        for (int tx = 0; tx < heights.GetLength(0); tx++)
        {
            for (int tz = 0; tz < heights.GetLength(1); tz++)
            {
                (float y, float steepness) = GetHeightAndAlphaAt(tx, tz);
                heights[tz, tx] = y;

                alphas[tz, tx, 0] = (steepness > 0f) ? 0f : 1f;
                alphas[tz, tx, 1] = (steepness > 0f) ? 1f : 0f;
            }
        }
        _terrainData.SetHeights(0, 0, heights);
        _terrainData.SetAlphamaps(0, 0, alphas);
    }

    private (float,float) GetHeightAndAlphaAt(float tx, float tz)
    {
       
        var maxAbsolute = _terrainData.heightmapResolution;
        var maxRelative = resolution;
        //avg 4 closest coords
        tx = tx / maxAbsolute * maxRelative;
        tz = tz / maxAbsolute * maxRelative;
        Vector2 vec = new Vector2(tx, tz);
        var coord1 = _coords[Mathf.FloorToInt(tx), Mathf.FloorToInt(tz)];
        var coord2 = _coords[Mathf.FloorToInt(tx), Mathf.CeilToInt(tz)];
        var coord3 = _coords[Mathf.CeilToInt(tx), Mathf.FloorToInt(tz)];
        var coord4 = _coords[Mathf.CeilToInt(tx), Mathf.CeilToInt(tz)];

        float distA = Vector2.Distance(vec, new Vector2(Mathf.FloorToInt(tx), Mathf.FloorToInt(tz)));
        float distB = Vector2.Distance(vec, new Vector2(Mathf.CeilToInt(tx), Mathf.CeilToInt(tz)));

        Plane p = (distA < distB) ? new Plane(coord1, coord2, coord3) : new Plane(coord4, coord3, coord2);
        Vector3 point = p.ClosestPointOnPlane(new Vector3(tx, 0f, tz));
        return (point.y, (p.normal == Vector3.up ||coord1.y < 0 || coord2.y < 0 || coord3.y < 0 || coord4.y < 0) ? 0f : 1f);
    }

    struct HeightJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float2> coordinates;
        public NativeArray<float> heights;

        [ReadOnly] public int gridResolution;
        [ReadOnly] public int terrainResolution;
        [ReadOnly] public int frequency;
        [ReadOnly] public int seed;
        [ReadOnly] public int terraces;
        [ReadOnly] public NoiseType noiseType;
        [ReadOnly] public bool terrace;

        public void Execute(int i)
        {
            float _x = (float)coordinates[i].x / frequency + seed;
            float _z = (float)coordinates[i].y / frequency + seed;

            float y = 0f;

            switch (noiseType)
            {
                case NoiseType.Perlin:
                    y = Mathf.PerlinNoise(_x, _z); break;
                case NoiseType.cellular:
                    y = noise.cellular(new float2(_x, _z)).x; break;
                case NoiseType.cnoise:
                    y = noise.cnoise(new float2(_x, _z)); break;
                case NoiseType.snoise:
                    y = noise.snoise(new float2(_x, _z)); break;
                case NoiseType.srnoise:
                    y = noise.srnoise(new float2(_x, _z)); break;
                case NoiseType.srdnoise:
                    y = noise.srdnoise(new float2(_x, _z)).x; break;
            }

            if (terrace)
                y = (Mathf.Floor(y * terraces)) / terraces;
            heights[i] = y;
        }
    }
}

