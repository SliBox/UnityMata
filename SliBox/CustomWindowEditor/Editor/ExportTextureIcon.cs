using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SliBox.Editor
{
    public class ExportTextureIcon : EditorWindow
    {

        [MenuItem("SliBox/Export Texture Icon")]
        public static void ShowWindow()
        {
            GetWindow<ExportTextureIcon>("Export Texture Icon");
        }

        enum ExportType
        {
            Single,
            Multiple,

        }


        public Texture2D texture;
        public SpriteMetaData[] spriteSheetMetaDataArray;
        public Vector2 scale, scrollPos;
        public Sprite icon;
        public Sprite[] icons;
        public bool[] choose;
        Sprite[] loadSpriteSheetAllIcon;

        int pageDigit;
        static int pageRenderHorizontal = 2;
        static int pageRenderVertical = 2;

        bool openSetRenderSheetSize;

        public string importPath, iconImportPath, exportPath;
        ExportType exportType;
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            ChooseType();
            EditorGUILayout.Space();
            ExportButton();
            GUILayout.EndVertical();
            EditorGUI.BeginChangeCheck();
            Import();
            bool importChanged = EditorGUI.EndChangeCheck();

            GUILayout.EndHorizontal();
            GUILayout.Label(exportPath, EditorStyles.helpBox);

            EnableSheetSizeWindow();
            if (importChanged)
            {
                OnChangeTexture();
                OnChangeIcon();
            }


            RenderMutible();


            ExportPath();

        }

        void ExportButton()
        {
            if (GUILayout.Button("Export"))
            {

                TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(exportType == ExportType.Single ? icon.texture : texture));
                ti.isReadable = true;
                ti.SaveAndReimport();
                if (exportType == ExportType.Multiple)
                {
                    for (int i = 0; i < loadSpriteSheetAllIcon.Length; i++)
                    {
                        if (choose[i])
                        {
                            ExportSingleIconToFolder(icons[i]);
                        }
                    }
                }
                else
                {
                    ExportSingleIconToFolder(icon);
                }
                ti.isReadable = false;
                ti.SaveAndReimport();
            }
        }

        void ChooseType()
        {
            if (GUILayout.Button(exportType == ExportType.Multiple ? "Multiple" : "Single"))
            {
                exportType = exportType == ExportType.Multiple ? ExportType.Single : ExportType.Multiple;
            }
        }

        void PageMoveController()
        {

            GUILayout.BeginHorizontal("box");
            if (texture == null) return;
            if (GUILayout.Button(pageDigit > 0 ? "Back" : "", GUILayout.Width(70)))
            {
                if (pageDigit > 0) pageDigit--;
            }
            GUILayout.Label("Page: " + (pageDigit + 1), EditorStyles.helpBox);
            bool showNextButton = pageDigit < (loadSpriteSheetAllIcon.Length) / (pageRenderHorizontal * pageRenderVertical);
            if (GUILayout.Button(showNextButton ? "Next" : "", GUILayout.Width(70)))
            {
                if (showNextButton) pageDigit++;
            }

            GUILayout.EndHorizontal();
        }

        void ExportPath()
        {
            if (exportType == ExportType.Multiple)
            {
                if (texture != null) exportPath = System.IO.Path.GetDirectoryName(importPath) + "/" + texture.name + "_Export_Icon";
            }
            else
            {
                if (icon != null) exportPath = System.IO.Path.GetDirectoryName(iconImportPath);
            }
        }

        void Import()
        {
            if (exportType == ExportType.Multiple)
            {
                texture = (Texture2D)EditorGUILayout.ObjectField("Texture", texture, typeof(Texture2D), false);

            }
            else
            {
                icon = (Sprite)EditorGUILayout.ObjectField("Sprite", icon, typeof(Sprite), false);
            }
        }


        void EnableSheetSizeWindow()
        {
            if (exportType == ExportType.Single) return;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(openSetRenderSheetSize ? "Set Sheet Size" : "Close", GUILayout.Width(EditorGUIUtility.singleLineHeight * 7f)))
            {
                openSetRenderSheetSize = !openSetRenderSheetSize;

            }
            if (openSetRenderSheetSize)
            {
                SetSheetSizeWindow();
            }



            GUILayout.EndHorizontal();

            if (!openSetRenderSheetSize)
            {
                GUILayout.Space(42);
            }
            EditorGUILayout.Space();
        }

        void SetSheetSizeWindow()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("-") && pageRenderVertical > 1)
            {
                pageRenderVertical--;
            }

            GUILayout.BeginHorizontal();


            if (GUILayout.Button("-") && pageRenderHorizontal > 1)
            {
                pageRenderHorizontal--;
            }
            GUILayout.Label("Width: " + pageRenderHorizontal + "    Heigh: " + pageRenderVertical, EditorStyles.helpBox);
            if (GUILayout.Button("+") && pageRenderHorizontal < 3)
            {
                pageRenderHorizontal++;
            }

            GUILayout.EndHorizontal();
            if (GUILayout.Button("+") && pageRenderVertical < 3)
            {
                pageRenderVertical++;
            }
            GUILayout.EndVertical();
        }

        void RenderMutible()
        {
            if (exportType == ExportType.Multiple)
            {
                SetCheckBox();
                if (texture == null) return;
                if (icons != null && texture != null)
                {
                    PageMoveController();
                    for (int i = 0; i < pageRenderVertical; i++)
                    {
                        EditorGUILayout.BeginHorizontal("Box");
                        for (int j = 0; j < pageRenderHorizontal; j++)
                        {
                            EditorGUILayout.BeginVertical("Box");
                            int element = pageDigit * pageRenderHorizontal * pageRenderVertical + (i * pageRenderHorizontal + j);
                            if (element >= 0 && element < loadSpriteSheetAllIcon.Length)
                            {

                                if (loadSpriteSheetAllIcon[element].name != texture.name)
                                {
                                    GUILayout.BeginHorizontal();
                                    choose[element] = EditorGUILayout.Toggle(choose[element]);
                                    GUILayout.Label(loadSpriteSheetAllIcon[element].name, EditorStyles.boldLabel);

                                    GUILayout.EndHorizontal();
                                    icons[element] = (Sprite)EditorGUILayout.ObjectField("Icon " + element, icons[element], typeof(Sprite), false);
                                    icons[element] = (Sprite)loadSpriteSheetAllIcon[element];


                                }
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();

                    }
                }



            }
        }


        void SetCheckBox()
        {
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Check All"))
            {
                for (int i = 0; i < choose.Length; i++)
                {
                    choose[i] = true;
                }
            }
            else if (GUILayout.Button("Uncheck All"))
            {
                for (int i = 0; i < choose.Length; i++)
                {
                    choose[i] = false;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("", EditorStyles.helpBox);
        }

        void OnChangeTexture()
        {
            pageDigit = 0;
            importPath = AssetDatabase.GetAssetPath(texture);
            loadSpriteSheetAllIcon = AssetDatabase.LoadAllAssetsAtPath(importPath).OfType<Sprite>().ToArray();
            foreach (var obj in loadSpriteSheetAllIcon)
            {
                Debug.Log(obj.name);
            }
            icons = new Sprite[loadSpriteSheetAllIcon.Length];
            choose = new bool[loadSpriteSheetAllIcon.Length];
        }

        void OnChangeIcon()
        {
            iconImportPath = AssetDatabase.GetAssetPath(icon);
        }



        void ExportSingleIconToFolder(Sprite sprite)
        {
            if (!Directory.Exists(exportPath))
            {
                Debug.Log("Create Folder : " + exportPath);
                Directory.CreateDirectory(exportPath);
            }

            if (!sprite)
                return;

            var tex = sprite.texture;
            var r = sprite.textureRect;
            var subtex = Texture2DExtensions.CropTexture(tex, (int)r.x, (int)r.y, (int)r.width, (int)r.height);
            var data = subtex.EncodeToPNG();
            var outPath = $"{exportPath}/{sprite.name + "_Icon"}.png";
            File.WriteAllBytes(outPath, data);
            Debug.Log($"Wrote to '{outPath}'");
            AssetDatabase.Refresh();
        }





        public class CheckFixIcon
        {
            public Sprite icon;
            public bool tick;
        }
    }

    static class Texture2DExtensions
    {
        /**
         * CropTexture
         * 
         * Returns a new texture, composed of the specified cropped region.
         */
        public static Texture2D CropTexture(this Texture2D pSource, int left, int top, int width, int height)
        {
            if (left < 0)
            {
                width += left;
                left = 0;
            }
            if (top < 0)
            {
                height += top;
                top = 0;
            }
            if (left + width > pSource.width)
            {
                width = pSource.width - left;
            }
            if (top + height > pSource.height)
            {
                height = pSource.height - top;
            }

            if (width <= 0 || height <= 0)
            {
                return null;
            }

            Color[] aSourceColor = pSource.GetPixels(0);

            //*** Make New
            Texture2D oNewTex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            //*** Make destination array
            int xLength = width * height;
            Color[] aColor = new Color[xLength];

            int i = 0;
            for (int y = 0; y < height; y++)
            {
                int sourceIndex = (y + top) * pSource.width + left;
                for (int x = 0; x < width; x++)
                {
                    aColor[i++] = aSourceColor[sourceIndex++];
                }
            }

            //*** Set Pixels
            oNewTex.SetPixels(aColor);
            oNewTex.Apply();

            //*** Return
            return oNewTex;
        }
    }
}

