using cfEngine.Util;

namespace cfUnityEngine.GameState.Bootstrap
{
    public class BootstrapEndState: GameState
    {
        public override GameStateId Id => GameStateId.BootstrapEnd;
        protected override void StartContext(StateParam stateParam)
        {
        }
    }
}