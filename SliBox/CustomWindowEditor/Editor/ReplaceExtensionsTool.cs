using UnityEngine;
using UnityEditor;
using System.IO;

namespace SliBox.Editor
{
    public class ReplaceExtensionsTool : EditorWindow
    {

        [MenuItem("SliBox/Replace Extensions Tool")]
        public static void ShowWindow()
        {
            GetWindow<ReplaceExtensionsTool>("Replace Extensions");
        }
        public Object folder;

        public string from, to;

        private void OnGUI()
        {
            InputField();
            Button();
        }

        void InputField()
        {
            folder = EditorGUILayout.ObjectField("Folder:", folder, typeof(DefaultAsset), false) as DefaultAsset;

            if (folder == null) return;

            GUILayout.Label(
                "\n" +
                "Input field...  Exa:  json -> txt");
            from = EditorGUILayout.TextField("From", from);
            to = EditorGUILayout.TextField("To", to);

            if (from == "" || to == "") return;
        }

        void Button()
        {
            if (GUILayout.Button("Replace Extensions"))
            {
                ReplaceExtensions();
            }
        }

        private void ReplaceExtensions()
        {
            string folderPath = AssetDatabase.GetAssetPath(folder);

            if (!Directory.Exists(folderPath))
            {
                Debug.LogError("Selected folder is not valid.");
                return;
            }

            string[] files = Directory.GetFiles(folderPath, "*." + from, SearchOption.AllDirectories);

            foreach (string filePath in files)
            {
                Debug.Log(filePath);
                string newFilePath = filePath.Replace(from, to);
                File.Move(filePath, newFilePath);
                Debug.Log($"Changed extension: {filePath} -> {newFilePath}");
            }

            AssetDatabase.Refresh();
        }
    }
}


