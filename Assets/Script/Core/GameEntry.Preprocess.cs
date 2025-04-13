using System;
using System.Diagnostics;
using cfEngine.Core;
using cfUnityEngine.GoogleDrive;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class GameEntry
{
    [Conditional("UNITY_EDITOR")]
    private void Preprocess()
    {
        try
        {
#if CF_GOOGLE_DRIVE
            if (GDriveMirrorSetting.GetSetting().refreshOnEnterPlayMode)
            {
                EditorUtility.DisplayProgressBar("GDriveMirror", "Refreshing files...", 0.1f);
                GDriveMirror.instance.Refresh();
            }
#endif
            EditorUtility.DisplayProgressBar("GameEntry", "Building Info", 0.2f);
            GameExtension.InfoBuildByte();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
        EditorUtility.ClearProgressBar();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    [Conditional("UNITY_EDITOR")]
    private static void CreateEditorGameEntry()
    {
        if(!FindAnyObjectByType<GameEntry>())
        {
            var gameEntry = Resources.Load<GameEntry>("Local/GameEntry");
            if (gameEntry == null)
            {
                Debug.LogError("GameEntry prefab not found in Resources/Local");
                return;
            }
            Instantiate(gameEntry);
        }
    }

}