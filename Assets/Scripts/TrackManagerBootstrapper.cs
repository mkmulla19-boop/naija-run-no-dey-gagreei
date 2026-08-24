using UnityEngine;

/*
TrackManagerBootstrapper.cs
Wires the user-dictated BackgroundSpawner with zero Inspector work:
- creates the TrackManager GameObject
- waits for Stage1Bootstrap's Player_Efe, assigns Player Transform
- builds low-poly background fillers FROM CODE (no downloads, no prefabs)
- sets Left X = -18, Right X = +18 exactly
*/

public class TrackManagerBootstrapper : MonoBehaviour
{
    private float timer;
    private bool wired;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Register()
    {
        if (Object.FindObjectOfType<TrackManagerBootstrapper>() == null)
        {
            var go = new GameObject("TrackManager");
            go.AddComponent<TrackManagerBootstrapper>();
            Object.DontDestroyOnLoad(go);
        }
    }

    private void Update()
    {
        if (wired) return;

        timer += Time.unscaledDeltaTime;
        if (timer < 0.5f) return;
        timer = 0f;

        GameObject player = GameObject.Find("Player_Efe");
        if (player == null) return;

        BackgroundSpawner sp = gameObject.AddComponent<BackgroundSpawner>();
        sp.playerTransform = player.transform;
        sp.segmentLength = 30f;
        sp.segmentsOnScreen = 5;
        sp.leftXPosition = -18f;
        sp.rightXPosition = 18f;

        sp.leftBackgroundPrefabs = new GameObject[]
        {
            MakeBlocker("LeftBg_WallA", new Vector3(7f, 10f, 9f), new Color(0.55f, 0.52f, 0.48f)),
            MakeBlocker("LeftBg_WallB", new Vector3(9f, 13f, 8f), new Color(0.45f, 0.40f, 0.34f))
        };

        sp.rightBackgroundPrefabs = new GameObject[]
        {
            MakeBlocker("RightBg_WallA", new Vector3(8f, 11f, 9f), new Color(0.50f, 0.46f, 0.42f)),
            MakeBlocker("RightBg_WallB", new Vector3(6f, 14f, 8f), new Color(0.42f, 0.38f, 0.33f))
        };

        sp.groundExtensionPrefab = MakeGround();

        wired = true;
        enabled = false;
        Debug.Log("[TrackManagerBootstrapper] BackgroundSpawner wired: Player assigned, filler arrays built from code, Left X=-18 Right X=+18");
    }

    private GameObject MakeBlocker(string name, Vector3 size, Color color)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.tag = "Untagged";
        BoxCollider col = box.GetComponent<BoxCollider>();
        if (col != null) Destroy(col);
        box.transform.position = new Vector3(0f, size.y / 2f, -2000f);
        box.transform.localScale = size;
        Renderer r = box.GetComponent<Renderer>();
        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        Material m = new Material(sh);
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
        else m.color = color;
        r.material = m;
        return box;
    }

    private GameObject MakeGround()
    {
        GameObject slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        slab.name = "BgGroundExtension";
        BoxCollider col = slab.GetComponent<BoxCollider>();
        if (col != null) Destroy(col);
        slab.transform.position = new Vector3(0f, -0.65f, -2000f);
        slab.transform.localScale = new Vector3(80f, 1f, 30f);
        Renderer r = slab.GetComponent<Renderer>();
        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        Material m = new Material(sh);
        Color dirt = new Color(0.35f, 0.28f, 0.20f);
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", dirt);
        else m.color = dirt;
        r.material = m;
        return slab;
    }
}
