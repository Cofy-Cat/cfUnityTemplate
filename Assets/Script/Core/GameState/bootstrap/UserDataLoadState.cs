using System;
using System.Collections.Generic;
using cfEngine.Core;
using cfEngine.Logging;
using cfEngine.Util;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class UserDataLoadState : GameState
    {
        public override HashSet<GameStateId> Whitelist { get; } = new() { GameStateId.Initialization };

        public override GameStateId Id => GameStateId.UserDataLoad;

        private void RegisterSavables()
        {
            var USER_DATA = Game.Current.GetUserData();

#if CF_STATISTIC
            USER_DATA.Register(Game.Current.GetStatistic());
#endif
#if CF_INVENTORY
            USER_DATA.Register(Game.Current.GetInventory());
#endif
#if CF_DIALOGUE
            USER_DATA.Register(Game.Current.GetDialogue());
#endif
        }
        
        protected override void StartContext(StateParam stateParam)
        {
            RegisterSavables();
            
            var userData = Game.Current.GetUserData();
            userData.LoadDataMap(Game.TaskToken).ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    try
                    {
                        var dataMap = t.Result;
                        userData.InitializeSavables(dataMap);
                        
                        StateMachine.TryGoToState(GameStateId.Initialization);
                    }
                    catch (Exception e)
                    {
                        Log.LogException(e);
                    }
                }
                else
                {
                    Log.LogException(t.Exception);
                }
            });
        }
    }
}