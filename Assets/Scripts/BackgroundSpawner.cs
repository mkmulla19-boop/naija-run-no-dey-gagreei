/*================================================================================
SCRIPT NAME: BackgroundSpawner.cs
TYPE: Single File, Self-Attaching Manager (Verified Coordinates)
================================================================================
*/


using UnityEngine;
using System.Collections.Generic;


public class BackgroundSpawner : MonoBehaviour
{
    [Header("Player & Track References")]
    public Transform playerTransform;
    public float segmentLength = 30.0f;
    public int segmentsOnScreen = 5;


    [Header("Tier 3 Void Filler Prefabs")]
    public GameObject[] leftBackgroundPrefabs;
    public GameObject[] rightBackgroundPrefabs;
    public GameObject groundExtensionPrefab;


    [Header("Outer Depth Coordinates")]
    public float leftXPosition = -18.0f;
    public float rightXPosition = 18.0f;


    private float spawnZ = 0.0f;
    private List<GameObject> activeBackgrounds = new List<GameObject>();
    private bool initialized = false;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoAttach()
    {
        if (FindFirstObjectByType<BackgroundSpawner>() == null)
        {
            GameObject trackManager = new GameObject("TrackManager");
            trackManager.AddComponent<BackgroundSpawner>();
            DontDestroyOnLoad(trackManager);
        }
    }


    private void Start()
    {
        TryInitialize();
    }


    private void Update()
    {
        if (!initialized)
        {
            TryInitialize();
            return;
        }


        if (playerTransform == null) return;


        if (playerTransform.position.z - segmentLength > spawnZ - (segmentsOnScreen * segmentLength))
        {
            SpawnBackgroundSegment();
            RemoveOldBackground();
        }
    }


    private void TryInitialize()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.Find("Player_Efe");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }


        if (playerTransform != null && !initialized)
        {
            CreateFallbackPrefabs();


            for (int i = 0; i < segmentsOnScreen; i++)
            {
                SpawnBackgroundSegment();
            }


            initialized = true;
            Debug.Log("[BackgroundSpawner] Verified coordinates locked: Player_Efe linked.");
        }
    }


    private void SpawnBackgroundSegment()
    {
        GameObject parentSegment = new GameObject("BackgroundSegment_" + spawnZ);


        // 1. Spawn Ground Plane Extension (Y = -0.1f sits perfectly under main road surface)
        if (groundExtensionPrefab != null)
        {
            GameObject ground = Instantiate(groundExtensionPrefab, new Vector3(0f, -0.1f, spawnZ), Quaternion.identity);
            ground.transform.SetParent(parentSegment.transform);
        }


        // 2. Spawn Far-Left Filler (X = -18, Y = 4.0f so 8m tall box rests on ground level)
        if (leftBackgroundPrefabs != null && leftBackgroundPrefabs.Length > 0)
        {
            int idx = Random.Range(0, leftBackgroundPrefabs.Length);
            if (leftBackgroundPrefabs[idx] != null)
            {
                Vector3 leftPos = new Vector3(leftXPosition, 4.0f, spawnZ);
                GameObject leftObj = Instantiate(leftBackgroundPrefabs[idx], leftPos, Quaternion.identity);
                leftObj.transform.SetParent(parentSegment.transform);
            }
        }


        // 3. Spawn Far-Right Filler (X = +18, Y = 4.0f so 8m tall box rests on ground level)
        if (rightBackgroundPrefabs != null && rightBackgroundPrefabs.Length > 0)
        {
            int idx = Random.Range(0, rightBackgroundPrefabs.Length);
            if (rightBackgroundPrefabs[idx] != null)
            {
                Vector3 rightPos = new Vector3(rightXPosition, 4.0f, spawnZ);
                GameObject rightObj = Instantiate(rightBackgroundPrefabs[idx], rightPos, Quaternion.Euler(0, 180, 0));
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


    private void CreateFallbackPrefabs()
    {
        if (leftBackgroundPrefabs == null || leftBackgroundPrefabs.Length == 0)
        {
            leftBackgroundPrefabs = new GameObject[] { CreatePrimitiveTemplate("LeftBlock", new Vector3(4f, 8f, 30f), new Color(0.3f, 0.3f, 0.35f)) };
        }


        if (rightBackgroundPrefabs == null || rightBackgroundPrefabs.Length == 0)
        {
            rightBackgroundPrefabs = new GameObject[] { CreatePrimitiveTemplate("RightBlock", new Vector3(4f, 8f, 30f), new Color(0.3f, 0.3f, 0.35f)) };
        }


        if (groundExtensionPrefab == null)
        {
            groundExtensionPrefab = CreatePrimitiveTemplate("GroundBlock", new Vector3(60f, 0.2f, 30f), new Color(0.25f, 0.2f, 0.15f));
        }
    }


    private GameObject CreatePrimitiveTemplate(string name, Vector3 scale, Color col)
    {
        GameObject template = GameObject.CreatePrimitive(PrimitiveType.Cube);
        template.name = name + "_Template";
        template.transform.localScale = scale;


        Collider c = template.GetComponent<Collider>();
        if (c != null) DestroyImmediate(c);


        Renderer r = template.GetComponent<Renderer>();
        if (r != null)
        {
            r.material = new Material(Shader.Find("Sprites/Default"));
            r.material.color = col;
        }


        template.SetActive(false);
        return template;
    }
}
