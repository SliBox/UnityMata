using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SliBox.CustomInspector.Interface;
using System.Linq;
using System;

namespace SliBox.CustomEditor.Editor
{

    [UnityEditor.CustomEditor(typeof(MonoBehaviour), true)]
    public class CustomInspectorGUI : UnityEditor.Editor
    {
        MonoBehaviour targetClass;
        public IEnableOnInspector enableOnInspector;
        public IDisableOnInspector disableOnInspector;
        public IUpdateOnInspector updateOnInspector;
        public IOnInspectorChanged onInspectorChanged;
        public ISaveButton saveChanged;
        public IAutoSave autoSaveChanged;

        public void OnEnable()
        {
            targetClass = (MonoBehaviour)target;
            enableOnInspector = targetClass is IEnableOnInspector ? (IEnableOnInspector)targetClass : null;
            disableOnInspector = targetClass is IDisableOnInspector ? (IDisableOnInspector)targetClass : null;
            try
            {
                if (enableOnInspector != null && !Application.isPlaying)
                {
                    enableOnInspector.EnableOnInspector();
                }
            }
            catch
            {
                Debug.LogError("Enable Inspector GUI Error");
            }
            

            updateOnInspector = targetClass is IUpdateOnInspector ? (IUpdateOnInspector)targetClass : null;
            onInspectorChanged = targetClass is IOnInspectorChanged ? (IOnInspectorChanged)targetClass :null;
            saveChanged = targetClass is ISaveButton ? (ISaveButton)targetClass : null;
            autoSaveChanged = targetClass is IAutoSave ? (IAutoSave)targetClass : null;
        }

        private void OnDisable()
        {
            if (disableOnInspector != null) disableOnInspector.DisableOnInspector();
            if (autoSaveChanged != null) SaveChanged();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            

            EditorGUILayout.Space();
            try
            {
                if (!Application.isPlaying)
                {
                    if (updateOnInspector != null) updateOnInspector.UpdateOnInspector();
                    if (EditorGUI.EndChangeCheck())
                    {
                        onInspectorChanged.OnInspectorChanged();
                        
                    }
                }
            }
            catch (Exception e)
            {
                GUILayout.Label("\nUpdate Inspector GUI Error\n" + e.Message);
            }

            

            EditorGUILayout.Space();
            SaveChangedButton();
        }

        void SaveChangedButton()
        {
            if (saveChanged != null)
            {
                if (GUILayout.Button("Save Changed"))
                {
                    SaveChanged();
                }
            }
        }

        void SaveChanged()
        {
            if(targetClass.gameObject.scene != null)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(targetClass.gameObject.scene);
            }
        }


    }

}
