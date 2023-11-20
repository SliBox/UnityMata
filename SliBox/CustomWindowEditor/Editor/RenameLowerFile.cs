using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SliBox.Editor
{
    public class RenameLowerFile : EditorWindow
    {
        [MenuItem("SliBox/Custom File Renamer")]

        static void Init()
        {
            RenameLowerFile window = (RenameLowerFile)GetWindow(typeof(RenameLowerFile));
            window.Show();
        }

        public Object folder;

        bool drawExtensionList;
        List<string> extensions;

        private void OnEnable()
        {
            extensions = new List<string>();
        }

        void OnGUI()
        {
            GUILayout.Label("Custom File Renamer", EditorStyles.boldLabel);

            folder = EditorGUILayout.ObjectField("Folder", folder, typeof(Object), true);
            InputExtensions();

            if (folder != null)
            {
                if (GUILayout.Button("Rename Files"))
                {
                    RenameFiles();
                }
            }

        }

        void InputExtensions()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Extensions"))
            {
                drawExtensionList = !drawExtensionList;
            }
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                if (extensions.Count > 0) extensions.RemoveAt(extensions.Count - 1);
            }
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                extensions.Add("");
            }
            EditorGUILayout.EndHorizontal();

            if (!drawExtensionList) return;
            for (int i = 0; i < extensions.Count; i++)
            {
                extensions[i] = EditorGUILayout.TextField("Element " + i, extensions[i]);
            }
        }

        void RenameFiles()
        {
            if (folder != null && folder is DefaultAsset)
            {
                string folderPath = AssetDatabase.GetAssetPath(folder);

                if (Directory.Exists(folderPath))
                {
                    string[] files = Directory.GetFiles(folderPath);

                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        string ext = Path.GetExtension(file).ToLower();

                        bool containExtension = false;
                        foreach (string extension in extensions)
                        {
                            if (ext.Contains(extension))
                            {
                                containExtension = true;
                                break;
                            }
                        }

                        if (containExtension || (extensions.Count == 0 && !ext.Contains("meta")))
                        {
                            string newName = Path.GetFileNameWithoutExtension(fileName).ToLower().Replace(" ", "_") + ext;

                            string newPath = Path.Combine(folderPath, newName);
                            File.Move(file, newPath);

                            AssetDatabase.Refresh();
                        }
                    }

                    Debug.Log("Files renamed successfully.");
                }
                else
                {
                    Debug.LogWarning("Selected object is not a valid folder.");
                }
            }
            else
            {
                Debug.LogWarning("Please select a valid folder.");
            }
        }
    }
}

