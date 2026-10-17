using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Person 3 - Manages multiple Ember Shrines over the course of the game.
/// Spawns a set number of shrines at random valid positions near the
/// treasure, and respawns a replacement elsewhere after a delay once
/// one is collected.
/// </summary>
public class ShrineSpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject shrinePrefab;
    public Transform treasureTarget;

    [Header("Spawn Rules")]
    public int maxActiveShrines = 2;
    public float minDistanceFromTreasure = 3f;
    public float maxDistanceFromTreasure = 10f;
    public float respawnDelay = 8f;
    public float floatHeight = 1.5f; // how high above the ground the shrine floats

    private List<GameObject> activeShrines = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < maxActiveShrines; i++)
        {
            SpawnShrine();
        }
    }

    private void SpawnShrine()
    {
        if (shrinePrefab == null || treasureTarget == null)
        {
            Debug.LogWarning("ShrineSpawner: missing shrinePrefab or treasureTarget reference.");
            return;
        }

        Vector3 spawnPos = GetRandomPositionNearTreasure();
        GameObject shrine = Instantiate(shrinePrefab, spawnPos, Quaternion.identity);

        EmberShrine shrineScript = shrine.GetComponent<EmberShrine>();
        if (shrineScript != null)
        {
            shrineScript.Initialize(this);
        }

        activeShrines.Add(shrine);
    }

    private Vector3 GetRandomPositionNearTreasure()
    {
        // Try several random angles/distances, checking each is on the
        // actual walkable NavMesh so shrines never spawn off the arena.
        for (int attempt = 0; attempt < 20; attempt++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(minDistanceFromTreasure, maxDistanceFromTreasure);
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * distance;
            Vector3 candidate = treasureTarget.position + offset;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                return hit.position + Vector3.up * floatHeight;
            }
        }

        // Fallback if no valid point found after 20 tries - spawn near treasure itself.
        return treasureTarget.position + Vector3.up * floatHeight;
    }

    /// <summary>
    /// Called by an EmberShrine instance when Kael collects it.
    /// </summary>
    public void NotifyShrineCollected(GameObject shrine)
    {
        activeShrines.Remove(shrine);
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnShrine();
    }
}