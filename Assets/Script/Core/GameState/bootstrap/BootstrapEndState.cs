using cfEngine.Util;
using cfUnityEngine.UI;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class BootstrapEndState: GameState
    {
        public override GameStateId Id => GameStateId.BootstrapEnd;
        protected override void StartContext(StateParam stateParam)
        {
            UIRoot.Current.GetPanel(LoadingUI.PANEL_ID).Hide();
        }
    }
}