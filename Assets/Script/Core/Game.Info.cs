using System.Diagnostics;
using System.IO;
using cfEngine.Info;
using cfEngine.Service;
using cfEngine.Serialize;
using RPG.Info;
using UnityEditor;

namespace cfEngine.Core
{
    public static partial class GameExtension
    {
        [MenuItem("Cf Tools/Info/Build Byte Info", false, 0)]
        [Conditional("UNITY_EDITOR")]
        public static void InfoBuildByte()
        {
            var editorStorage = new EditorAssetStorage("Info");
            var encoder = new CofyDev.Xml.Doc.DataObjectEncoder();
            var editorInfos = new InfoManager[]
            {
                new InventoryInfoManager(new ExcelByteLoader<InventoryInfo>(CreateEditorStorage(nameof(InventoryInfo)), encoder)),
                new DialogueInfoManager(new ExcelByteLoader<DialogueInfo>(CreateEditorStorage(nameof(DialogueInfo)), encoder))
            };

            EditorAssetStorage CreateEditorStorage(string infoDirectory)
            {
                return new EditorAssetStorage(Path.Combine("Info", infoDirectory));
            }

            foreach (var info in editorInfos)
            {
                info.LoadInfo();
            }

            var streamingStorage = new StreamingAssetStorage("Info");
            var serializer = JsonSerializer.Instance;
            foreach (var infoManager in editorInfos)
            {
                var allValue = infoManager.GetAllValue();
                var serialized = serializer.Serialize(allValue);
                streamingStorage.Save(infoManager.infoDirectory, serialized);
            }
        }
    }
}