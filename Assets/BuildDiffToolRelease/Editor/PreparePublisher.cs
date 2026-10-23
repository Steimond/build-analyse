using UnityEditor;

namespace VectraCode.Release
{
    public static class PreparePublisher
    {
        public static void Open()
        {
            EditorApplication.delayCall += () => EditorApplication.ExecuteMenuItem("Tools/Asset Store/Uploader");
        }
    }
}
