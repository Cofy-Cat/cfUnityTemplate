using cfEngine.Core;
using cfEngine.Extension;
using cfEngine.Logging;
using cfEngine.Util;
using cfUnityEngine.UI;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class LocalLoadState: GameState
    {
        public override GameStateId Id => GameStateId.LocalLoad;
        protected override void StartContext(StateParam param)
        {
            var ui = UIRoot.Current;
            var loadingUI = ui.Register(new LoadingUI(), "Local/LoadingUI");
            ui.PreloadPanel(loadingUI.id)
                .ContinueWithSynchronized(t =>
                {
                    if (t.IsFaulted && t.Exception != null)
                    {
                        Log.LogException(t.Exception);
                    }
                    else
                    {
                        ui.InstantiatePanel(loadingUI.id)
                            .ContinueWithSynchronized(task =>
                            {
                                if (task.IsCompletedSuccessfully)
                                {
                                    loadingUI.Show();
                                }
                            });
                        StateMachine.ForceGoToState(GameStateId.InfoLoad);
                    }
                }, Game.TaskToken);
        }
    }
}