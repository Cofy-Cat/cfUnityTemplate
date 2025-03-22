using System.Threading;
using cfEngine.Asset;
using cfEngine.Core;
using cfEngine.Info;
using cfEngine.Service;
using cfEngine.Pooling;
using cfUnityEngine.GameState;
using Object = UnityEngine.Object;

namespace cfEngine.Service
{
    public partial class ServiceName
    {
        public static readonly string Info = "Info";
        public static readonly string Asset = "Asset";
        public static readonly string Pool = "Pool";
        public static readonly string Gsm = "Gsm";
        public static readonly string Auth = "Auth";
        public static readonly string UserData = "UserData";
    }
}

namespace cfEngine.Core
{
    public class GameBuilder : Game
    {
        public GameBuilder WithInfo(InfoLayer info)
        {
            Register(info, ServiceName.Info);
            return this;
        }

        public GameBuilder WithAsset(AssetManager<Object> asset)
        {
            Register(asset, ServiceName.Asset);
            return this;
        }

        public GameBuilder WithPool(PoolManager pool)
        {
            Register(pool, ServiceName.Pool);
            return this;
        }

        public GameBuilder WithGsm(GameStateMachine gsm)
        {
            Register(gsm, ServiceName.Gsm);
            return this;
        }

        public GameBuilder WithAuth(AuthService auth)
        {
            Register(auth, ServiceName.Auth);
            return this;
        }

        public GameBuilder WithUserData(UserDataManager userData)
        {
            Register(userData, ServiceName.UserData);
            return this;
        }
        
        public GameBuilder WithService(IService service, string name)
        {
            Register(service, name);
            return this;
        }

        public Game Build() => this;
    }
}