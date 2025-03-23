using System.Collections.Generic;
using System.Text.Json.Nodes;
using cfEngine.Core;
using cfEngine.Logging;
using cfEngine.Service;
using RPG.Service.Dialogue;

namespace cfEngine.Core 
{
    public static partial class ServiceName
    {
        public const string Dialogue = "Dialogue";
    }
    
    public static partial class GameExtension
    {
        public static Game WithDialogue(this Game game, IDialogueService dialogueService)
        {
            game.Register(dialogueService, ServiceName.Dialogue);
            return game;
        }
        
        public static DialogueService GetDialogue(this Game game) => game.GetService<DialogueService>(ServiceName.Dialogue);
    }
}

namespace RPG.Service.Dialogue
{
    public interface IDialogueService: IService
    {
    }
    
    public class DialogueService: IDialogueService, IRuntimeSavable
    {
        public void Initialize(IReadOnlyDictionary<string, JsonObject> dataMap)
        {
            Log.LogInfo("DialogueService initialized");
        }

        public void Save(Dictionary<string, object> dataMap)
        {
            Log.LogInfo("DialogueService saved");
        }

        public void Dispose()
        {
        }
    }
}