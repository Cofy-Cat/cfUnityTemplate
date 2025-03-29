using System;
using System.Diagnostics;
using cfEngine.Core;
using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class GameEntry
{
    [Conditional("UNITY_EDITOR")]
    private void Preprocess()
    {
        try
        {
            GameExtension.InfoBuildByte();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
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