using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using cfEngine.Asset;
using cfEngine.Core;
using cfEngine.Info;
using cfEngine.IO;
using cfEngine.Logging;
using cfEngine.Pooling;
using cfEngine.Serialize;
using cfEngine.Service;
using cfEngine.Util;
using cfUnityEngine.GameState;
using cfUnityEngine.UI;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameEntry : MonoBehaviour
{
    [Conditional("UNITY_EDITOR")]
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void PreprocessInEditor()
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
    public static void Init()
    {
        Log.SetLogger(new UnityLogger());
        Log.SetLogLevel(LogLevel.Debug);

        var cts = new CancellationTokenSource();

        var gameBuilder = new GameBuilder()
#if CF_STATISTIC
            .WithService(new cfEngine.Service.Statistic.StatisticService(), nameof(cfEngine.Service.Statistic.StatisticService))
#endif
            .WithService(new cfEngine.Service.Inventory.InventoryService(),
                nameof(cfEngine.Service.Inventory.InventoryService))
#if CF_ADDRESSABLE
            .WithAsset(new AddressableAssetManager())
#else
            .WithAsset(new ResourceAssetManager())
#endif
            .WithInfo(new InfoLayer(new StreamingAssetStorage("Info"), JsonSerializer.Instance))
            .WithPool(new PoolManager())
            .WithUserData(new UserDataManager(new LocalFileStorage(Application.persistentDataPath), JsonSerializer.Instance));

        var auth = new LocalAuthService();
        auth.RegisterPlatform(new LocalPlatform());
        gameBuilder.WithAuth(auth);

        var gsm = new GameStateMachine();
        gsm.OnAfterStateChange += OnStateChanged;
        gameBuilder.WithGsm(gsm);
        
        Game.SetCurrent(gameBuilder.Build());
        
        Application.quitting += OnApplicationQuit;
        
        void OnApplicationQuit()
        {
            var gsm = Game.Get<GameStateMachine>();
            gsm.OnAfterStateChange -= OnStateChanged;
            Application.quitting -= OnApplicationQuit;
            
            cts.Cancel();
            if (UIRoot.Instance != null)
            {
                UIRoot.Instance.Dispose();
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
        
        gsm.TryGoToState(GameStateId.LocalLoad);
    }

    private static void OnStateChanged(StateChangeRecord<GameStateId> record)
    {
        Log.LogInfo($"Game state changed, {record.LastState.ToString()} -> {record.NewState.ToString()}");
    }

    [Conditional("UNITY_EDITOR")]
    public static void RegisterEditorPostBootstrapAction([NotNull] Action action)
    {
        RegisterPostBootstrapAction(action);
    }

    private static void RegisterPostBootstrapAction(Action action)
    {
        var gsm = Game.Get<GameStateMachine>();
        if (gsm.CurrentStateId > GameStateId.BootstrapEnd)
        {
            action?.Invoke();
            return;
        }
        
        gsm.OnAfterStateChange += OnBootstrapEnd;
        void OnBootstrapEnd(StateChangeRecord<GameStateId> record)
        {
            if (record.NewState != GameStateId.BootstrapEnd)
                return;
            
            gsm.OnAfterStateChange -= OnBootstrapEnd;
            
            action?.Invoke();
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
