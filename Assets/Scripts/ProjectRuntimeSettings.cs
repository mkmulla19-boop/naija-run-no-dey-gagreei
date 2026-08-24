using UnityEngine;

public static class ProjectRuntimeSettings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Apply()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }
}