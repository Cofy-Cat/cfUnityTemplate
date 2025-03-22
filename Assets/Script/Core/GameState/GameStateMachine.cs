using cfEngine.Service;
using cfEngine.Util;
using cfUnityEngine.GameState.Bootstrap;

namespace cfUnityEngine.GameState
{
    public enum GameStateId
    {
        LocalLoad,
        InfoLoad,
        Login,
        UserDataLoad,
        Initialization,
        UILoad,
        BootstrapEnd,
    }

    public abstract class GameState : State<GameStateId, GameState, GameStateMachine>
    {
    }

    public class GameStateMachine : StateMachine<GameStateId, GameState, GameStateMachine>, IService
    {
        public GameStateMachine() : base()
        {
            RegisterState(new LocalLoadState());
            RegisterState(new InfoLoadState());
            RegisterState(new LoginState());
            RegisterState(new UserDataLoadState());
            RegisterState(new InitializationState());
            RegisterState(new UILoadState());
            RegisterState(new BootstrapEndState());
        }
    }
}