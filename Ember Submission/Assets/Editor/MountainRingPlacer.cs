using UnityEngine;
using UnityEditor;

public class MountainRingPlacer : EditorWindow
{
    public GameObject[] mountainPrefabs;
    public Transform arenaCenter;
    public int count = 12;
    public float radius = 30f;
    public float radiusJitter = 4f;
    public float scaleMin = 0.8f;
    public float scaleMax = 1.4f;

    [MenuItem("Tools/Mountain Ring Placer")]
    static void Open() => GetWindow<MountainRingPlacer>("Mountain Ring Placer");

    void OnGUI()
    {
        SerializedObject so = new SerializedObject(this);
        EditorGUILayout.PropertyField(so.FindProperty("mountainPrefabs"), true);
        EditorGUILayout.PropertyField(so.FindProperty("arenaCenter"));
        EditorGUILayout.PropertyField(so.FindProperty("count"));
        EditorGUILayout.PropertyField(so.FindProperty("radius"));
        EditorGUILayout.PropertyField(so.FindProperty("radiusJitter"));
        EditorGUILayout.PropertyField(so.FindProperty("scaleMin"));
        EditorGUILayout.PropertyField(so.FindProperty("scaleMax"));
        so.ApplyModifiedProperties();

        if (GUILayout.Button("Place Mountains") && mountainPrefabs.Length > 0)
            PlaceMountains();
    }

    void PlaceMountains()
    {
        GameObject parent = new GameObject("MountainBackdrop");
        if (arenaCenter != null) parent.transform.position = arenaCenter.position;

        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i;
            float r = radius + Random.Range(-radiusJitter, radiusJitter);
            Vector3 pos = parent.transform.position + Quaternion.Euler(0, angle, 0) * (Vector3.forward * r);

            GameObject prefab = mountainPrefabs[Random.Range(0, mountainPrefabs.Length)];
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent.transform);
            instance.transform.position = pos;
            instance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            float s = Random.Range(scaleMin, scaleMax);
            instance.transform.localScale *= s;
            instance.name = "MountainPeak";
        }
    }
}