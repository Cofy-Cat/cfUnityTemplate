using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cfEngine.Core;
using cfEngine.Info;
using cfEngine.Serialize;
using cfEngine.Service;
using cfEngine.Util;
using cfEngine.Service.Auth;
using RPG.Info;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class InfoLoadState : GameState
    {
        public override HashSet<GameStateId> Whitelist { get; } = new() { GameStateId.Login };
        public override GameStateId Id => GameStateId.InfoLoad;

        protected override void StartContext(StateParam stateParam)
        {
            var infoLayer = Game.Current.GetInfo();
            
            RegisterInfos(infoLayer);

            var infoLoadTasks = infoLayer.InfoMap.Values.Select(info => info.LoadInfoAsync(Game.TaskToken));
            Task.WhenAll(infoLoadTasks).ContinueWith(t =>
            {
                StateMachine.TryGoToState(GameStateId.Login, new LoginState.Param()
                {
                    Platform = LoginPlatform.Local,
                    Token = new LoginToken()
                });
            }, Game.TaskToken, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void RegisterInfos(InfoLayer info)
        {
            var storage = new StreamingAssetStorage("Info");
            var serializer = JsonSerializer.Instance;
            info.RegisterInfo(new InventoryInfoManager(new SerializationLoader<InventoryInfo>(storage, serializer)));
            info.RegisterInfo(new DialogueInfoManager(new SerializationLoader<DialogueInfo>(storage, serializer)));
        }
    }
}