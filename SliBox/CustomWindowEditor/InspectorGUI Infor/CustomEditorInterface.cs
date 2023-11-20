using System.Collections.Generic;
using UnityEngine;
namespace SliBox.CustomInspector.Interface
{

    public interface IUpdateOnInspector
    {
#if UNITY_EDITOR
        public void UpdateOnInspector();
#endif
    }
    public interface IEnableOnInspector
    {
#if UNITY_EDITOR
        public void EnableOnInspector();
#endif
    }

    public interface IDisableOnInspector
    {
#if UNITY_EDITOR
        public void DisableOnInspector();
#endif
    }

    public interface IOnInspectorChanged
    {
#if UNITY_EDITOR
        public void OnInspectorChanged();
#endif
    }

    public interface ISaveButton
    {

    }

    public interface IAutoSave
    {

    }
}
