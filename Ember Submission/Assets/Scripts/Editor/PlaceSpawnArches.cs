using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PlaceSpawnArches
{
    private const string ArchName = "SpawnArch";

    [MenuItem("Ember/Place Arch On Spawn Points")]
    public static void Place()
    {
        GameObject archAsset = Selection.activeObject as GameObject;

        if (archAsset == null || !EditorUtility.IsPersistent(archAsset))
        {
            EditorUtility.DisplayDialog("Spawn arches",
                "Select the arch FBX or prefab in the Project window first.", "OK");
            return;
        }

        GameObject spawnRoot = GameObject.Find("SpawnPoints");

        if (spawnRoot == null)
        {
            EditorUtility.DisplayDialog("Spawn arches",
                "Could not find a GameObject called SpawnPoints in the open scene.", "OK");
            return;
        }

        int placed = 0;

        foreach (Transform spawnPoint in spawnRoot.transform)
        {
            if (spawnPoint.Find(ArchName) != null)
            {
                continue;
            }

            GameObject arch = (GameObject)PrefabUtility.InstantiatePrefab(archAsset, spawnPoint);
            arch.name = ArchName;
            arch.transform.localPosition = Vector3.zero;
            arch.transform.localRotation = Quaternion.identity;
            arch.transform.localScale = Vector3.one;
            Undo.RegisterCreatedObjectUndo(arch, "Place spawn arch");
            placed++;
        }

        EditorSceneManager.MarkSceneDirty(spawnRoot.scene);
        Debug.Log("Placed " + placed + " spawn arch(es).");
    }
}