using System;
using System.Collections.Generic;
using cfEngine.Info;

namespace RPG.Info
{
    public class DialogueInfoManager: ConfigInfoManager<string, DialogueInfo>
    {
        public override string infoKey => nameof(DialogueInfoManager);
        public override string infoDirectory => nameof(DialogueInfo);
        protected override Func<DialogueInfo, string> keyFn => x => x.id;
    }

    [Serializable]
    public class DialogueInfo
    {
        public string id;
        public List<DialogueBranchInfo> branch;
    }

    public class DialogueBranchInfo
    {
        public string condition;
        public string nextDialogue;
    }
}