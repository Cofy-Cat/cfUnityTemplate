using System.Diagnostics;
using cfEngine.Info;
using cfEngine.Service;
using cfEngine.Serialize;

namespace cfEngine.Core
{
    public static partial class GameExtension
    {
        private static InfoManager[] _allInfo = new[]
        {
            new InventoryInfoManager(),
        };

        [Conditional("UNITY_EDITOR")]
        public static void InfoBuildByte()
        {
            var editorLayer = new InfoLayer(new EditorAssetStorage("Info"), JsonSerializer.Instance);

            foreach (var info in _allInfo)
            {
                editorLayer.RegisterInfo(info);
                info.DirectlyLoadFromExcel();
            }

            var runtimeLayer = new InfoLayer(new StreamingAssetStorage("Info"), JsonSerializer.Instance);

            foreach (var info in _allInfo)
            {
                runtimeLayer.RegisterInfo(info);
                info.SerializeIntoStorage();
            }
        }
    }
}