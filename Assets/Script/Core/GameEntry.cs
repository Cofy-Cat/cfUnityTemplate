using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using cfEngine.Asset;
using cfEngine.Core;
using cfEngine.Core.Layer;
using cfEngine.IO;
using cfEngine.Logging;
using cfEngine.Pooling;
using cfEngine.Serialize;
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
            Game.InfoBuildByte();
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
        var initParam = new Game.InitParam()
        {
            taskToken = cts.Token,
            info = new InfoLayer(new StreamingAssetStorage("Info"), JsonSerializer.Instance),

#if CF_ADDRESSABLE
            asset = new AddressableAssetManager(),
#else
            asset = new ResourceAssetManager(),
#endif
            pool = new PoolManager(),
            gsm = new GameStateMachine(),
            userData = new UserDataManager(new LocalFileStorage(Application.persistentDataPath), JsonSerializer.Instance),
            auth = new LocalLoginHandler()
        };
        
        Game.MakeInstance(initParam);
        
        Game.Auth.RegisterPlatform(new LocalPlatform());
        
        Game.Gsm.OnAfterStateChange += OnStateChanged;
        Application.quitting += OnApplicationQuit;
        
        void OnApplicationQuit()
        {
            Game.Gsm.OnAfterStateChange -= OnStateChanged;
            Application.quitting -= OnApplicationQuit;
            
            cts.Cancel();
            if (UIRoot.Instance != null)
            {
                UIRoot.Instance.Dispose();
            }
            Game.Dispose();

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
        
        Game.Gsm.TryGoToState(GameStateId.LocalLoad);
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
        if (Game.Gsm.CurrentStateId > GameStateId.BootstrapEnd)
        {
            action?.Invoke();
            return;
        }
        
        Game.Gsm.OnAfterStateChange += OnBootstrapEnd;
        void OnBootstrapEnd(StateChangeRecord<GameStateId> record)
        {
            if (record.NewState != GameStateId.BootstrapEnd)
                return;
            
            Game.Gsm.OnAfterStateChange -= OnBootstrapEnd;
            
            action?.Invoke();
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
