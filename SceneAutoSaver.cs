#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[ExecuteInEditMode, InitializeOnLoad]
public static class SceneAutoSaver
{
    private static float timer = 1000;
    private static float resetTimer = 1000;

    static SceneAutoSaver()
    {
        EditorApplication.playModeStateChanged += SaveOnEnterPlaymode;
        EditorApplication.update += SaveOnInterval;
        EditorApplication.quitting += SaveScenes;
    }

    public static void SaveOnEnterPlaymode(PlayModeStateChange obj)
    {

        if (obj == PlayModeStateChange.ExitingEditMode)
        {
            Debug.Log("Autosave : On Enter Playmode");
            SaveScenes();
        }
    }

    public static void SaveScenes()
    {
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }

    public static void SaveOnInterval()
    {
        if(timer > 0) { timer -= Time.deltaTime; }

        else if (timer <= 0 && !EditorApplication.isPlaying)
        {
            Debug.Log("Autosave : Interval");
            SaveScenes();
            timer = resetTimer;
        }
    }
}
#endif