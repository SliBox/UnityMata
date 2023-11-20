using UnityEngine;
using UnityEditor;
using System.IO;

namespace SliBox.Editor
{
    public class AudioClipCutter : EditorWindow
    {
        private AudioClip audioClip;
        private float startTime = 0f;
        private float endTime = 1f;

        string ext;

        bool audioClipChanged;

        [MenuItem("SliBox/Audio Clip Cutter")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(AudioClipCutter)).Show();
        }

        void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            audioClipChanged = false;
            audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", audioClip, typeof(AudioClip), false);
            audioClipChanged = EditorGUI.EndChangeCheck();
            GetExtension();
            if (audioClip == null) return;



            if (ext != ".wav")
            {
                GUILayout.Label("Use only .wav file");
            }
            else
            {
                startTime = EditorGUILayout.Slider("Start Time", startTime, 0f, audioClip.length);
                endTime = EditorGUILayout.Slider("End Time", endTime, 0f, audioClip.length);

                if (GUILayout.Button("Cut Audio Clip"))
                {
                    CutAudioClip();
                }
            }

        }

        void GetExtension()
        {
            string path = AssetDatabase.GetAssetPath(audioClip);
            ext = Path.GetExtension(path);

        }

        void CutAudioClip()
        {
            if (audioClip != null && endTime > startTime)
            {
                string path = AssetDatabase.GetAssetPath(audioClip);

                AudioClip newClip = AudioClip.Create(
                    audioClip.name + "_cut",
                    (int)((endTime - startTime) * audioClip.frequency),
                    audioClip.channels,
                    audioClip.frequency,
                    false);

                float[] data = new float[(int)((endTime - startTime) * audioClip.frequency * audioClip.channels)];
                audioClip.GetData(data, (int)(startTime * audioClip.frequency * audioClip.channels));

                newClip.SetData(data, 0);

                AssetDatabase.CreateAsset(newClip, path.Replace(ext, "_cut" + ext));
                AssetDatabase.Refresh();
            }
        }
    }
}

