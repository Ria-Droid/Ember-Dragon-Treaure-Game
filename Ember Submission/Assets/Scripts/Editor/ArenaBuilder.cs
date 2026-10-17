#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEditor.AI;

public class ArenaBuilder : EditorWindow
{
    private const float ArenaRadius = 24f;
    private const int SpawnPointCount = 6;

    [MenuItem("Ember/Build Starter Arena")]
    public static void BuildArena()
    {
        GameObject root = new GameObject("Arena");

        //Our Ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "ArenaGround";
        ground.transform.SetParent(root.transform);
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(3f, 1f, 3f); // Unity plane = 10x10 units at scale 1

        GameObjectUtility.SetStaticEditorFlags(ground, StaticEditorFlags.NavigationStatic);

        //The Treasure marker
        GameObject treasure = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        treasure.name = "Treasure";
        treasure.transform.SetParent(root.transform);
        treasure.transform.position = new Vector3(0f, 0.75f, 0f);
        treasure.transform.localScale = new Vector3(1.2f, 0.75f, 1.2f);

        Renderer treasureRenderer = treasure.GetComponent<Renderer>();
        Material treasureMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        treasureMat.color = new Color(0.95f, 0.64f, 0.25f); // ember orange
        treasureRenderer.sharedMaterial = treasureMat;

        //The Spawn points
        GameObject spawnRoot = new GameObject("SpawnPoints");
        spawnRoot.transform.SetParent(root.transform);

        for (int i = 0; i < SpawnPointCount; i++)
        {
            float angle = (360f / SpawnPointCount) * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * ArenaRadius;

            GameObject spawnPoint = new GameObject($"SpawnPoint{i + 1}");
            spawnPoint.transform.SetParent(spawnRoot.transform);
            spawnPoint.transform.position = pos;
            spawnPoint.transform.LookAt(treasure.transform);
        }

        GameObject rockRoot = new GameObject("RockObstacles");
        rockRoot.transform.SetParent(root.transform);

        Vector3[] rockOffsets =
        {
            new Vector3(5f, 0f, 3f),
            new Vector3(-6f, 0f, -2f),
            new Vector3(2f, 0f, -7f),
        };

        Material rockMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        rockMat.color = new Color(0.26f, 0.24f, 0.32f);

        foreach (Vector3 offset in rockOffsets)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rock.name = "RockObstacle";
            rock.transform.SetParent(rockRoot.transform);
            rock.transform.position = offset + Vector3.up * 0.5f;
            rock.transform.localScale = new Vector3(
                Random.Range(1.2f, 2f), Random.Range(1f, 1.8f), Random.Range(1.2f, 2f));
            rock.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            rock.GetComponent<Renderer>().sharedMaterial = rockMat;

            GameObjectUtility.SetStaticEditorFlags(rock, StaticEditorFlags.NavigationStatic);
        }

        GameObject mountainRoot = new GameObject("MountainBackdrop");
        mountainRoot.transform.SetParent(root.transform);

        Material mountainMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mountainMat.color = new Color(0.16f, 0.15f, 0.22f);

        int mountainCount = 14;
        float mountainRadius = ArenaRadius + 12f;
        for (int i = 0; i < mountainCount; i++)
        {
            float angle = (360f / mountainCount) * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * mountainRadius;

            GameObject mountain = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            mountain.name = "MountainPeak";
            mountain.transform.SetParent(mountainRoot.transform);
            float height = Random.Range(8f, 16f);
            mountain.transform.position = pos + Vector3.up * (height * 0.5f - 1f);
            mountain.transform.localScale = new Vector3(Random.Range(4f, 7f), height, Random.Range(4f, 7f));
            mountain.GetComponent<Renderer>().sharedMaterial = mountainMat;

            DestroyImmediate(mountain.GetComponent<Collider>());
        }

        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();

        Selection.activeGameObject = root;
        Debug.Log("Arena built: ground, treasure, 6 spawn points, rock obstacles, mountain backdrop, NavMesh baked. " +
                   "Drag SpawnPoints' children into WaveManager, and Treasure into treasureTarget fields.");
    }
}
#endif
