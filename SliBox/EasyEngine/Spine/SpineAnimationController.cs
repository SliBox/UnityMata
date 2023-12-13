using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Spine;
using UnityEngine;
using UnityEditor;

namespace SliBoxEngine.SpineAnim
{

    public class SpineAnimationController : MonoBehaviour
    {
        [HideInInspector] public int reviewAnim;
        [HideInInspector] public float reviewAnimSlide;
        #region Inspector Editor

        public int AnimIndexGUIPopup(string label, int index)
        {
            int returnIndex = 0;
#if UNITY_EDITOR
            UnityEditor.EditorGUILayout.Space();
            returnIndex = UnityEditor.EditorGUILayout.Popup(label, index, AnimationNames);
#endif
            return returnIndex;
        }

        public virtual void EnableOnInspector()
        {
            GetAnimNames();
        }

        public virtual void UpdateOnInspector()
        {

        }

        #endregion

        #region Get Animation State
        SkeletonAnimation _skeletonAnimation;

        public virtual SkeletonAnimation SkeletonAnimation
        {
            get
            {
                if (_skeletonAnimation == null)
                {
                    _skeletonAnimation = GetComponent<SkeletonAnimation>();
                }
                return _skeletonAnimation;
            }
        }

        SkeletonGraphic _skeletonGraphic;

        public virtual SkeletonGraphic SkeletonGraphic
        {
            get
            {
                if (_skeletonGraphic == null)
                {
                    _skeletonGraphic = GetComponent<SkeletonGraphic>();
                }
                return (_skeletonGraphic);
            }
        }

        public SkeletonDataAsset SkeletonDataAsset
        {
            get
            {
                if (_skeletonAnimation != null) return _skeletonAnimation.SkeletonDataAsset;
                else if (_skeletonGraphic != null) return _skeletonGraphic.SkeletonDataAsset;
                else if (SkeletonAnimation != null) return SkeletonAnimation.skeletonDataAsset;
                else if (SkeletonGraphic != null) return SkeletonGraphic.skeletonDataAsset;
                else return null;
            }
        }

        public Spine.AnimationState AnimationState
        {
            get
            {
                if (SkeletonAnimation != null)
                {
                    return SkeletonAnimation.AnimationState;
                }
                else if (SkeletonGraphic != null)
                {
                    return SkeletonGraphic.AnimationState;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Anim Names

        private string[] _animationNames;

        public string[] AnimationNames
        {
            get
            {
                if (_animationNames == null || _animationNames.Length == 0)
                {
                    GetAnimNames();
                }
                return _animationNames;
            }
        }


        public void GetAnimNames()
        {
            if (SkeletonDataAsset == null)
            {
                _animationNames = new string[] { "None" };
            }

            else
            {
                SkeletonData m_skeletonData = SkeletonDataAsset.GetSkeletonData(false);

                _animationNames = new string[m_skeletonData.Animations.Items.Length];
                for (int checkIndex = 0; checkIndex < _animationNames.Length; checkIndex++)
                {
                    _animationNames[checkIndex] = m_skeletonData.Animations.Items[checkIndex].Name;
                }
            }
        }
        #endregion

        #region Play Anim

        public float GetDuration(int index)
        {
            return SkeletonDataAsset.GetSkeletonData(true).Animations.Items[index].Duration;
        }
        public float PlayAnim(int animationIndex, bool loop = false)
        {
            return AnimationState.SetAnimation(0, SkeletonDataAsset.GetSkeletonData(true).Animations.Items[animationIndex], loop).Animation.Duration;
        }
        public float PlayAnim(int trackIndex, int animationIndex, bool loop = false)
        {
            return AnimationState.SetAnimation(trackIndex, SkeletonDataAsset.GetSkeletonData(true).Animations.Items[animationIndex], loop).Animation.Duration;
        }

        public float PlayAnim(string animName, bool loop = false)
        {
            return AnimationState.SetAnimation(0, animName, loop).Animation.Duration;
        }
        public float PlayAnim(int trackIndex, string animName, bool loop = false)
        {
            return AnimationState.SetAnimation(trackIndex, animName, loop).Animation.Duration;
        }


        public void AddAnim(int animationIndex, float delay, bool loop = false)
        {
            AnimationState.AddAnimation(0, AnimationNames[animationIndex], loop, delay);
        }

        public void AddAnim(int trackIndex, int animationIndex, float delay, bool loop = false)
        {
            AnimationState.AddAnimation(trackIndex, AnimationNames[animationIndex], loop, delay);
        }

        public void AddAnim(string animName, float delay, bool loop = false)
        {
            AnimationState.AddAnimation(0, animName, loop, delay);
        }

        public void AddAnim(int trackIndex, string animName, float delay, bool loop = false)
        {
            AnimationState.AddAnimation(trackIndex, animName, loop, delay);
        }
        #endregion

        public void ResetSkeleton()
        {
            SkeletonDataAsset.Clear();
        }



    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SpineAnimationController), true)]
    public class SpineAnimationControllerInspector : Editor
    {
        SpineAnimationController targetClass;

        float _reviewAnimDuration;

        public void OnEnable()
        {
            targetClass = (SpineAnimationController)target;
            targetClass.EnableOnInspector();
        }

        private void OnDisable()
        {
            try
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(targetClass.gameObject.scene);
            }
            catch { }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            targetClass.UpdateOnInspector();
            ReviewAnim();
        }

        void ReviewAnim()
        {
            GUILayout.Space(50);
            EditorGUI.BeginChangeCheck();
            ReviewAnimField();
            ReviewAnimSlide();

            if (EditorGUI.EndChangeCheck())
            {
                SetAnim();
            }
        }

        void ReviewAnimField()
        {
            targetClass.reviewAnim = targetClass.AnimIndexGUIPopup("Review Anim", targetClass.reviewAnim);
            _reviewAnimDuration = targetClass.GetDuration(targetClass.reviewAnim);
        }
        void ReviewAnimSlide()
        {
            targetClass.reviewAnimSlide = EditorGUILayout.Slider(targetClass.reviewAnimSlide, 0, _reviewAnimDuration);
        }

        void SetAnim()
        {
            Skeleton skeleton = targetClass.SkeletonAnimation != null? targetClass.SkeletonAnimation.Skeleton : targetClass.SkeletonGraphic.Skeleton;
            
            var state = targetClass.AnimationState;

            if (!Application.isPlaying)
            {
                if (state != null) state.ClearTrack(0);
                skeleton.SetToSetupPose();
            }

            var animationToUse = targetClass.AnimationState.Data.SkeletonData.Animations.Items[targetClass.reviewAnim];

            if (!Application.isPlaying)
            {
                if (animationToUse != null)
                {
                    float trackTime = targetClass.reviewAnimSlide / (_reviewAnimDuration + 0.01f) * _reviewAnimDuration;
                    targetClass.AnimationState.SetAnimation(0, animationToUse, false).TrackTime = trackTime;
                }
                if(targetClass.SkeletonAnimation != null)
                {
                    targetClass.SkeletonAnimation.Update(0);
                    targetClass.SkeletonAnimation.LateUpdate();
                }
                else if(targetClass.SkeletonGraphic != null)
                {
                    targetClass.SkeletonGraphic.Update(0);
                    targetClass.SkeletonGraphic.LateUpdate();
                }
            }
        }
    }
#endif
}