/*
================================================================================
SCRIPT NAME: BackgroundSpawner.cs
LOCATION: Attach to "TrackManager" GameObject in Unity.
INSTRUCTIONS FOR OPENCODE & DEVELOPER:
1. Copy this entire code block and paste it into BackgroundSpawner.cs.
2. Select "TrackManager" in your Unity Hierarchy.
3. Assign your Player Transform to "Player Transform".
4. Set Left Background Prefabs Size to match your available left-side prefabs.
5. Set Right Background Prefabs Size to match your available right-side prefabs.
6. Drag low-poly background models (buildings, walls, trees) into the Elements.
7. Set Left X Position to -18 and Right X Position to 18.
8. Run the scene to verify background void coverage.
================================================================================
*/


using UnityEngine;
using System.Collections.Generic;


public class BackgroundSpawner : MonoBehaviour
{
    [Header("Player & Track References")]
    [Tooltip("Drag the main Player object or Camera target here")]
    public Transform playerTransform;


    [Tooltip("Length of each track segment along the Z-axis")]
    public float segmentLength = 30.0f;


    [Tooltip("Number of background segments active at one time")]
    public int segmentsOnScreen = 5;


    [Header("Tier 3 Void Filler Prefabs")]
    [Tooltip("List of background buildings/trees for the left side (X = -18)")]
    public GameObject[] leftBackgroundPrefabs;


    [Tooltip("List of background buildings/trees for the right side (X = +18)")]
    public GameObject[] rightBackgroundPrefabs;


    [Tooltip("Optional ground extension plane to prevent ground-level gaps")]
    public GameObject groundExtensionPrefab;


    [Header("Outer Depth Coordinates")]
    [Tooltip("X coordinate for far-left void filler structures")]
    public float leftXPosition = -18.0f;


    [Tooltip("X coordinate for far-right void filler structures")]
    public float rightXPosition = 18.0f;


    private float spawnZ = 0.0f;
    private List<GameObject> activeBackgrounds = new List<GameObject>();


    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("[BackgroundSpawner] Player Transform is missing! Assign it in the Inspector.");
            return;
        }


        // Initialize starting background segments
        for (int i = 0; i < segmentsOnScreen; i++)
        {
            SpawnBackgroundSegment();
        }
    }


    private void Update()
    {
        if (playerTransform == null) return;


        // Continuous spawner logic as player moves forward on Z-axis
        if (playerTransform.position.z - segmentLength > spawnZ - (segmentsOnScreen * segmentLength))
        {
            SpawnBackgroundSegment();
            RemoveOldBackground();
        }
    }


    private void SpawnBackgroundSegment()
    {
        GameObject parentSegment = new GameObject("BackgroundSegment_" + spawnZ);


        // 1. Ground Plane Extension Validation
        if (groundExtensionPrefab != null)
        {
            GameObject ground = Instantiate(groundExtensionPrefab, new Vector3(0, -0.1f, spawnZ), Quaternion.identity);
            ground.transform.SetParent(parentSegment.transform);
        }


        // 2. Left Array Validation & Instantiation (Fills Left Void)
        if (leftBackgroundPrefabs != null && leftBackgroundPrefabs.Length > 0)
        {
            int leftIndex = Random.Range(0, leftBackgroundPrefabs.Length);
            if (leftBackgroundPrefabs[leftIndex] != null)
            {
                Vector3 leftPos = new Vector3(leftXPosition, 0, spawnZ);
                GameObject leftObj = Instantiate(leftBackgroundPrefabs[leftIndex], leftPos, Quaternion.identity);
                leftObj.transform.SetParent(parentSegment.transform);
            }
        }


        // 3. Right Array Validation & Instantiation (Fills Right Void)
        if (rightBackgroundPrefabs != null && rightBackgroundPrefabs.Length > 0)
        {
            int rightIndex = Random.Range(0, rightBackgroundPrefabs.Length);
            if (rightBackgroundPrefabs[rightIndex] != null)
            {
                Vector3 rightPos = new Vector3(rightXPosition, 0, spawnZ);
                GameObject rightObj = Instantiate(rightBackgroundPrefabs[rightIndex], rightPos, Quaternion.Euler(0, 180, 0));
                rightObj.transform.SetParent(parentSegment.transform);
            }
        }


        activeBackgrounds.Add(parentSegment);
        spawnZ += segmentLength;
    }


    private void RemoveOldBackground()
    {
        if (activeBackgrounds.Count > 0)
        {
            Destroy(activeBackgrounds[0]);
            activeBackgrounds.RemoveAt(0);
        }
    }
}
