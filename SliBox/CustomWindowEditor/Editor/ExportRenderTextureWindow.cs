using UnityEngine;
using UnityEditor;
using System.IO;

namespace SliBox.Editor
{
    public class ExportRenderTextureWindow : EditorWindow
    {
        private RenderTexture renderTexture;
        public Object folder;
        private string textureName = "ExportedTexture.png";

        [MenuItem("SliBox/Export Render Texture")]
        static void Init()
        {
            ExportRenderTextureWindow window = (ExportRenderTextureWindow)EditorWindow.GetWindow(typeof(ExportRenderTextureWindow));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Export Render Texture to Texture", EditorStyles.boldLabel);

            renderTexture = (RenderTexture)EditorGUILayout.ObjectField("Render Texture", renderTexture, typeof(RenderTexture), true);
            folder = EditorGUILayout.ObjectField("Export Folder", folder, typeof(Object), false);
            textureName = EditorGUILayout.TextField("Texture Name", textureName);

            if (GUILayout.Button("Export Texture"))
            {
                ExportTexture();
            }
        }

        void ExportTexture()
        {
            if (renderTexture == null)
            {
                Debug.LogWarning("Render Texture is not assigned.");
                return;
            }

            // Tạo Texture2D mới
            Texture2D exportedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, true);

            RenderTexture.active = renderTexture;
            exportedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            exportedTexture.Apply();

            byte[] bytes = exportedTexture.EncodeToPNG();

            string folderPath = AssetDatabase.GetAssetPath(folder);
            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.LogError("Export Folder is not assigned or not valid.");
                return;
            }

            string fullPath = Path.Combine(Application.dataPath, folderPath);
            string filePath = Path.Combine(folderPath, textureName);

            Directory.CreateDirectory(fullPath);

            File.WriteAllBytes(filePath, bytes);
            AssetDatabase.Refresh();
            Debug.Log("Texture exported to: " + filePath);
        }
    }
}

