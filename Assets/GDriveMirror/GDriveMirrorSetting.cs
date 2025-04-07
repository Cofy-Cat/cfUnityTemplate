using UnityEngine;

namespace GDriveMirror
{
    [CreateAssetMenu(fileName = "New Mirror Setting", menuName = "GDriveMirror/Mirror Setting", order = 0)]
    public class GDriveMirrorSetting : ScriptableObject
    {
        public string serviceAccountCredentialJsonPath;
    }
}