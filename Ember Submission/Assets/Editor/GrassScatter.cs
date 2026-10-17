#if UNITY_EDITOR
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GrassScatter : EditorWindow
{
    public GameObject[] grassPrefabs;     // Grass01A, Grass01B, Grass01C
    public Transform floor;               // CircularArenaGround
    public Transform spawnPointsRoot;     // SpawnPoints (only used if spawnClearRadius > 0)
    public Transform treasure;            // Treasure (only used if treasureClearRadius > 0)

    public float spacing = 0.8f;          // grid cell size in metres. Smaller = denser
    public float jitter = 0.2f;           // max random offset from cell centre, in metres
    public float edgeInset = 0.5f;
    public float spawnClearRadius = 0f;   // 0 = grass everywhere, including under the arches
    public float treasureClearRadius = 0f;
    public float scaleMin = 1.0f;
    public float scaleMax = 1.5f;
    public int seed = 12345;

    private const string ParentName = "GrassScatter";
    private const int MaxTufts = 25000;

    [MenuItem("Tools/Grass Scatter")]
    static void Open() => GetWindow<GrassScatter>("Grass Scatter");

    void OnGUI()
    {
        SerializedObject so = new SerializedObject(this);
        EditorGUILayout.PropertyField(so.FindProperty("grassPrefabs"), true);
        EditorGUILayout.PropertyField(so.FindProperty("floor"));
        EditorGUILayout.PropertyField(so.FindProperty("spawnPointsRoot"));
        EditorGUILayout.PropertyField(so.FindProperty("treasure"));
        EditorGUILayout.PropertyField(so.FindProperty("spacing"));
        EditorGUILayout.PropertyField(so.FindProperty("jitter"));
        EditorGUILayout.PropertyField(so.FindProperty("edgeInset"));
        EditorGUILayout.PropertyField(so.FindProperty("spawnClearRadius"));
        EditorGUILayout.PropertyField(so.FindProperty("treasureClearRadius"));
        EditorGUILayout.PropertyField(so.FindProperty("scaleMin"));
        EditorGUILayout.PropertyField(so.FindProperty("scaleMax"));
        EditorGUILayout.PropertyField(so.FindProperty("seed"));
        so.ApplyModifiedProperties();

        EditorGUILayout.Space();

        if (GUILayout.Button("Scatter Grass"))
            Scatter();

        if (GUILayout.Button("Clear Grass"))
            ClearExisting();
    }

    void Scatter()
    {
        if (grassPrefabs == null || grassPrefabs.Length == 0 || floor == null)
        {
            EditorUtility.DisplayDialog("Grass Scatter",
                "Assign at least one grass prefab and the floor object first.", "OK");
            return;
        }

        if (spacing < 0.2f)
        {
            EditorUtility.DisplayDialog("Grass Scatter", "Spacing must be at least 0.2 m.", "OK");
            return;
        }

        Renderer floorRenderer = floor.GetComponent<Renderer>();
        if (floorRenderer == null)
        {
            EditorUtility.DisplayDialog("Grass Scatter", "The floor object has no Renderer.", "OK");
            return;
        }

        // Cylinder floor: radius from the rendered bounds (65 scale -> 32.5 m).
        Bounds b = floorRenderer.bounds;
        float radius = Mathf.Min(b.extents.x, b.extents.z) - edgeInset;
        Vector3 centre = new Vector3(b.center.x, 0f, b.center.z);
        float topY = b.max.y;

        int estimate = Mathf.RoundToInt(Mathf.PI * radius * radius / (spacing * spacing));
        if (estimate > MaxTufts)
        {
            EditorUtility.DisplayDialog("Grass Scatter",
                $"That would place about {estimate} tufts, which is too many. Increase Spacing.", "OK");
            return;
        }

        ClearExisting();

        GameObject parent = new GameObject(ParentName);
        parent.transform.SetParent(floor.parent, false);
        parent.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        Undo.RegisterCreatedObjectUndo(parent, "Scatter grass");

        // Keep the grass out of the NavMesh bake.
        NavMeshModifier modifier = parent.AddComponent<NavMeshModifier>();
        modifier.ignoreFromBuild = true;
        modifier.applyToChildren = true;

        System.Random rng = new System.Random(seed);
        int placed = 0;
        int steps = Mathf.CeilToInt(radius / spacing);

        for (int ix = -steps; ix <= steps; ix++)
        {
            for (int iz = -steps; iz <= steps; iz++)
            {
                Vector3 cell = centre + new Vector3(ix * spacing, 0f, iz * spacing);

                // jitter within the cell, then keep only points inside the floor
                float jx = ((float)rng.NextDouble() * 2f - 1f) * jitter;
                float jz = ((float)rng.NextDouble() * 2f - 1f) * jitter;
                Vector3 pos = cell + new Vector3(jx, 0f, jz);
                pos.y = topY;

                if (FlatDistance(pos, centre) > radius)
                    continue;

                if (IsBlocked(pos))
                    continue;

                GameObject prefab = grassPrefabs[rng.Next(grassPrefabs.Length)];
                if (prefab == null)
                    continue;

                GameObject tuft = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent.transform);
                tuft.transform.SetPositionAndRotation(pos, Quaternion.Euler(0f, rng.Next(0, 360), 0f));
                tuft.transform.localScale *= Mathf.Lerp(scaleMin, scaleMax, (float)rng.NextDouble());
                placed++;
            }
        }

        EditorSceneManager.MarkSceneDirty(parent.scene);
        Debug.Log($"Grass Scatter: placed {placed} tufts on a floor of radius {radius + edgeInset:0.0} m " +
                  $"(spacing {spacing} m).");
    }

    bool IsBlocked(Vector3 pos)
    {
        if (spawnClearRadius > 0f && spawnPointsRoot != null)
        {
            foreach (Transform sp in spawnPointsRoot)
            {
                if (FlatDistance(pos, sp.position) < spawnClearRadius)
                    return true;
            }
        }

        if (treasureClearRadius > 0f && treasure != null &&
            FlatDistance(pos, treasure.position) < treasureClearRadius)
            return true;

        return false;
    }

    static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    void ClearExisting()
    {
        GameObject old = GameObject.Find(ParentName);
        if (old != null)
            Undo.DestroyObjectImmediate(old);
    }
}
#endif