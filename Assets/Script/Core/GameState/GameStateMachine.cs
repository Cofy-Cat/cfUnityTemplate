using cfEngine.Service;
using cfEngine.Util;
using cfUnityEngine.GameState.Bootstrap;

namespace cfEngine.Core
{
    using cfUnityEngine.GameState;
    public static partial class ServiceName
    {
        public const string GameStateMachine = "Gsm";
    }

    public static partial class GameExtension
    {
        public static Game WithGameStateMachine(this Game game, GameStateMachine service)
        {
            game.Register(service, ServiceName.GameStateMachine);
            return game;
        }

        public static GameStateMachine GetGameStateMachine(this Game game) => game.GetService<GameStateMachine>(ServiceName.GameStateMachine);
    }
}

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
