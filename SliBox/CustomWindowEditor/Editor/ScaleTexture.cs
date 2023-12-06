using System.IO;
using UnityEditor;
using UnityEngine;


namespace SliBox.Editor
{
    public class ScaleTexture : EditorWindow
    {

        public enum ScaleType
        {
            type1,
            type2,
        }

        public enum SaveType
        {
            replace,
            CustomFolder
        }

        [MenuItem("SliBox/Scale Texture")]
        public static void ShowWindow()
        {
            GetWindow<ScaleTexture>("Scale Texture");
        }
        Texture2D texture;
        public Object folder;
        ScaleType scaleType;
        SaveType saveType;
        float scaleRate;

        Vector2Int targetSize;

        int newWidthSize;
        int newHeightSize;



        private void OnGUI()
        {
            InputField();
            Button();
        }

        void InputField()
        {
            texture = (Texture2D)EditorGUILayout.ObjectField("Texute", texture, typeof(Texture2D), false);
            if (saveType == SaveType.CustomFolder)
            {
                folder = EditorGUILayout.ObjectField("Export Folder:", folder, typeof(DefaultAsset), false) as DefaultAsset;
            }
            else
            {
                GUILayout.Space(20);
            }
            GUILayout.BeginHorizontal();
            scaleType = (ScaleType)EditorGUILayout.EnumPopup("Scale Type", scaleType);
            saveType = (SaveType)EditorGUILayout.EnumPopup("Save Type", saveType);
            GUILayout.EndHorizontal();

            if (scaleType == ScaleType.type1) scaleRate = EditorGUILayout.FloatField("Rate", scaleRate);
            else if (scaleType == ScaleType.type2) targetSize = EditorGUILayout.Vector2IntField("Target Size", targetSize);

            if (folder == null) return;

            GUILayout.Label(
                "\n" +
                "\n");
        }

        void Button()
        {
            if (texture == null) return;
            if (
                (scaleType == ScaleType.type1 && scaleRate <= 0) ||
                (scaleType == ScaleType.type2 && (texture.width <= 0 || texture.height <= 0))
                ) return;

            if (folder != null && texture != null)
            {
                newWidthSize = scaleType == ScaleType.type1 ? Mathf.RoundToInt(texture.width * scaleRate) : targetSize.x;
                newHeightSize = scaleType == ScaleType.type1 ? Mathf.RoundToInt(texture.height * scaleRate) : targetSize.y;

                if (GUILayout.Button("Export New Size: " + newWidthSize + "*" + newHeightSize))
                {
                    SetReadWriteEnabled();
                    ScaleImage();
                    SetReadWriteDisable();
                    AssetDatabase.Refresh();
                }
            }
        }

        public void ScaleImage()
        {
            if (newWidthSize > 8192)
            {
                Debug.LogWarning("Can't Scale Big Size. Textute width: " + newWidthSize);
            }
            Texture2D newTex = new Texture2D(newWidthSize, newHeightSize, TextureFormat.RGBA32, false);
            for (int w = 0; w < newTex.width; w++)
            {
                for (int h = 0; h < newTex.height; h++)
                {
                    float pixelRateX = texture.width / (float)newWidthSize;
                    float pixelRateY = texture.height / (float)newHeightSize;
                    int x = Mathf.FloorToInt(w * pixelRateX + (pixelRateX * 0.5f));
                    int y = Mathf.FloorToInt(h * pixelRateY + (pixelRateY * 0.5f));
                    Color color = texture.GetPixel(x, y);
                    newTex.SetPixel(w, h, color);
                }
            }
            newTex.name = texture.name;
            string folderPath = saveType == SaveType.CustomFolder ? AssetDatabase.GetAssetPath(folder) : Path.GetDirectoryName(AssetDatabase.GetAssetPath(texture));
            SaveTextureToFolderPath(newTex, folderPath);
        }

        public static void SaveTextureToFolderPath(Texture2D texture, string path)
        {
            texture.Apply();
            string filePath = path + "/" + texture.name + ".png";
            File.WriteAllBytes(filePath, texture.EncodeToPNG());
            Debug.Log("Saved to " + filePath);
        }

        void SetReadWriteEnabled()
        {
            string texturePath = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.isReadable = true;
                AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
            }
            else
            {
                Debug.LogWarning("Failed to get TextureImporter.");
            }
        }

        void SetReadWriteDisable()
        {
            string texturePath = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.isReadable = false;
                AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
            }
            else
            {
                Debug.LogWarning("Failed to get TextureImporter.");
            }
        }
    }
}

