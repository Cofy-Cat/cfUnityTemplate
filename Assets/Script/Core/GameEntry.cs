using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using cfEngine.Core;
using cfEngine.Info;
using cfEngine.IO;
using cfEngine.Logging;
using cfEngine.Pooling;
using cfEngine.Serialize;
using cfEngine.Util;
using cfEngine.Service.Auth;
using cfUnityEngine.GameState;
using cfUnityEngine.UI;
using cfUnityEngine.UI.UGUI;
using cfUnityEngine.UI.UIToolkit;
using cfUnityEngine.Util;
using RPG.Service.Dialogue;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class GameEntry : MonoBehaviour
{
    [SerializeField] 
    private UGUIRoot uiRoot;
    
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

    private void Awake()
    {
        Preprocess();
        
        DontDestroyOnLoad(gameObject);
        
        Log.SetLogger(new UnityLogger());
        Log.SetLogLevel(LogLevel.Debug);

        var game = new Game()
#if CF_STATISTIC
            .WithStatistic(new cfEngine.Service.Statistic.StatisticService())
#endif
            .WithInventory(new cfEngine.Service.Inventory.InventoryService())
#if CF_ADDRESSABLE
            .WithAsset(new cfUnityEngine.Asset.AddressableAssetManager())
#else
            .WithAsset(new ResourceAssetManager())
#endif
            .WithInfo(new InfoLayer(new StreamingAssetStorage("Info"), JsonSerializer.Instance))
            .WithPoolManager(new PoolManager())
            .WithUserData(new UserDataManager(new LocalFileStorage(Application.persistentDataPath), JsonSerializer.Instance))
            .WithAuthService(
                new AuthService.Builder()
                    .SetService(new LocalAuthService())
                    .RegisterPlatform(new LocalPlatform()).Build())
            .WithGameStateMachine(new GameStateMachine())
            .WithDialogue(new DialogueService())
            ;
        
        Game.SetCurrent(game);
        uiRoot.Initialize(game.GetAsset<Object>());
        GameObjectUtil.DontDestroyOnLoadIfRoot(uiRoot);
        UIRoot.SetCurrent(uiRoot);

        var gsm = Game.Current.GetGameStateMachine();
        gsm.OnAfterStateChange += OnStateChanged;

        gsm.TryGoToState(GameStateId.LocalLoad);
    }

    private void OnApplicationQuit()
    {
        var gsm = Game.Current.GetGameStateMachine();
        gsm.OnAfterStateChange -= OnStateChanged;
        Application.quitting -= OnApplicationQuit;

        if (UIRoot.Current != null)
        {
            UIRoot.Current.Dispose();
        }

        Game.Current.Dispose();

#if CF_REACTIVE_DEBUG
            var notDisposed = cfEngine.Rt._RtDebug.Instance.Collections;
            if (notDisposed.Count > 0)
            {
                foreach (var collectionRef in notDisposed.Values)
                {
                    if (collectionRef.TryGetTarget(out var collectionDebug))
                    {
                        Log.LogWarning("Not disposed collection: " + collectionDebug.__GetDebugTitle());
                    }
                }
            }
#endif
    }
    
    private static void OnStateChanged(StateChangeRecord<GameStateId> record)
    {
        Log.LogInfo($"Game state changed, {record.LastState.ToString()} -> {record.NewState.ToString()}");
    }
}
