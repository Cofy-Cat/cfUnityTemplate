using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using cfEngine.Core;
using cfEngine.Logging;
using cfEngine.Service;
using cfEngine.Util;
using RPG.Info;
using RPG.Script.Service;
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
    public class UserDataKey
    {
        public const string Dialogue = "Dialogue";
    }
    
    public interface IDialogueService: IService
    {
    }
    
    public class DialogueService: IDialogueService, IRuntimeSavable
    {
        private DialogueRecord currentDialogue { get; set; }
        
        public void Initialize(IReadOnlyDictionary<string, JsonObject> dataMap)
        {
            if (dataMap.TryGetValue(UserDataKey.Dialogue, out var data))
            {
                currentDialogue = data.GetValue<DialogueRecord>();
            }
            else
            {
                var firstDialogue = Game.Current.GetInfo().Get<DialogueInfoManager>().allValues.FirstOrDefault();
                if (!SanityCheck.WhenNull(firstDialogue, "No dialogue found in DialogueInfoManager"))
                {
                    currentDialogue = new DialogueRecord()
                    {
                        id = firstDialogue?.id,
                    };
                }
            }
        }

        public void Save(Dictionary<string, object> dataMap)
        {
            dataMap[UserDataKey.Dialogue] = currentDialogue;
        }

        public void Dispose()
        {
        }
    }
}