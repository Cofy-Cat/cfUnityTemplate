using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cfEngine.Core;
using cfEngine.Info;
using cfEngine.Service;
using cfEngine.Util;
using cfUnityEngine.Auth;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class InfoLoadState : GameState
    {
        public override HashSet<GameStateId> Whitelist { get; } = new() { GameStateId.Login };
        public override GameStateId Id => GameStateId.InfoLoad;

        protected override void StartContext(StateParam stateParam)
        {
            var infoLayer = Game.Current.GetInfo();
            infoLayer.RegisterInfo(new InventoryInfoManager());

            var infoLoadTasks = infoLayer.InfoMap.Values.Select(info => info.LoadSerializedAsync(Game.TaskToken));
            Task.WhenAll(infoLoadTasks).ContinueWith(t =>
            {
                StateMachine.TryGoToState(GameStateId.Login, new LoginState.Param()
                {
                    Platform = LoginPlatform.Local,
                    Token = new LoginToken()
                });
            }, Game.TaskToken, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}