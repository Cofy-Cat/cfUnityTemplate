using cfEngine.Core;
using cfEngine.Extension;
using cfEngine.Util;
using cfUnityEngine.UI;
using UnityEngine;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class LocalLoadState: GameState
    {
        public override GameStateId Id => GameStateId.LocalLoad;
        protected override void StartContext(StateParam param)
        {
            var uiPrefab = Game.Current.GetAsset<Object>().Load<GameObject>("Local/UIRoot");
            var ui = Object.Instantiate(uiPrefab).GetComponent<UIRoot>();

            ui.Register<LoadingUI>("Local/LoadingUI");
            ui.LoadPanel<LoadingUI>().ContinueWithSynchronized(t =>
            {
                t.Result.ShowPanel();
                StateMachine.ForceGoToState(GameStateId.InfoLoad);
            }, Game.TaskToken);
        }
    }
}