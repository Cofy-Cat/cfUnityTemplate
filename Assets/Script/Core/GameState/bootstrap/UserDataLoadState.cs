using System.Collections.Generic;
using cfEngine.Core;
using cfEngine.Service;
using cfEngine.Service.Statistic;
using cfEngine.Util;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class UserDataLoadState : GameState
    {
        public override HashSet<GameStateId> Whitelist { get; } = new() { GameStateId.Initialization };

        public override GameStateId Id => GameStateId.UserDataLoad;

        private void RegisterSavables()
        {
            var USER_DATA = Game.Get<UserDataManager>();

#if CF_STATISTIC
            USER_DATA.Register(Game.Get<StatisticService>());
#endif
#if CF_INVENTORY
            USER_DATA.Register(Game.Get<InventoryService>());
#endif
        }
        
        protected override void StartContext(StateParam stateParam)
        {
            RegisterSavables();
            
            var userData = Game.Get<UserDataManager>();
            userData.LoadDataMap(Game.TaskToken).ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    var dataMap = t.Result;
                    userData.InitializeSavables(dataMap);
                    
                    StateMachine.TryGoToState(GameStateId.Initialization);
                }
            });
        }
    }
}