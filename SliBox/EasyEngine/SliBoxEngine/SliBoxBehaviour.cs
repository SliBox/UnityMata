using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SliBoxEngine
{
    public class SliBoxBehaviour : MonoBehaviour
    {
        static SliBoxBehaviour s_sliBoxBehaviour;
        public static SliBoxBehaviour Behaviour
        {
            get
            {
                if (s_sliBoxBehaviour == null || s_sliBoxBehaviour.gameObject == null)
                {
                    s_sliBoxBehaviour = Instantiate(new GameObject()).AddComponent<SliBoxBehaviour>();
                    s_sliBoxBehaviour.gameObject.name = "SliBox";
                }
                return s_sliBoxBehaviour;
            }
        }

        public static int Today
        {
            get { return ((int)DateTimeOffset.Now.ToUnixTimeSeconds() + (int)DateTimeOffset.Now.Offset.TotalSeconds) / (60 * 60 * 24); }
        }

        public static float FixDeltaTime
        {
            get
            {
                if (Time.deltaTime > 0.2)
                {
                    return 0.2f;
                }
                else
                {
                    return Time.deltaTime;
                }
            }
        }

        public static bool InternetIsConnecting
        {
            get
            {
                return !(Application.internetReachability == NetworkReachability.NotReachable);
            }
        }

        public static IEnumerator ActionNeedNetWork(Action action, Action disconenctAction = null)
        {
            while (!InternetIsConnecting)
            {
                if (disconenctAction != null) disconenctAction();
                yield return null;
            }

            action();
        }


        public static Vector2 AngleToVector2(float a, bool bUseRadians = false)
        {
            Vector2 v = Vector2.up; // vector offset
            if (!bUseRadians) a *= Mathf.Deg2Rad;
            var ca = Mathf.Cos(a);
            var sa = Mathf.Sin(a);
            var rx = v.x * ca - v.y * sa;

            return new Vector2((float)rx * -1, (float)(v.x * sa + v.y * ca));
        }


        public static float Vector2ToAngle(Vector2 dirVector)
        {
            return dirVector.x > 0 ? -Vector2.Angle(Vector2.up, dirVector) : Vector2.Angle(Vector2.up, dirVector);
        }


        public class SpriteColor
        {
            public static void ChangeSpriteRendererColor(SpriteRenderer spr, Color colorTarget, float duration)
            {
                Behaviour.StartCoroutine(IEChangeSpriteRendererColor(spr, colorTarget, duration));
            }
            static IEnumerator IEChangeSpriteRendererColor(SpriteRenderer spr, Color colorTarget, float duration)
            {
                float timer = 0;
                Color colorStart = spr.color;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    float t = timer / duration;
                    spr.color = Color.Lerp(colorStart, colorTarget, t);
                    yield return null;
                }
                spr.color = colorTarget;
            }
            public static void ChangeImageColor(Image img, Color colorTarget, float duration)
            {
                Behaviour.StartCoroutine(IEChangeImageColor(img, colorTarget, duration));
            }
            static IEnumerator IEChangeImageColor(Image img, Color colorTarget, float duration)
            {
                float timer = 0;
                Color colorStart = img.color;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    float t = timer / duration;
                    img.color = Color.Lerp(colorStart, colorTarget, t);
                    yield return null;
                }
                img.color = colorTarget;
            }
        }
        public class CouroutineAnim
        {
            public static void ShakeAnimLocal(Transform objTransform, float shakeDelay, int loop = 0, float shakeAngle = 15)
            {
                Behaviour.StartCoroutine(IEShake(objTransform, shakeDelay, loop, shakeAngle, true));
            }
            public static void ShakeAnimWorldSpace(Transform objTransform, float shakeDelay, int loop = 0, float shakeAngle = 15)
            {
                Behaviour.StartCoroutine(IEShake(objTransform, shakeDelay, loop, shakeAngle, false));
            }

            static IEnumerator IEShake(Transform objTransform, float waveDuration, int loop = 0, float shakeAngle = 15, bool originLocal = false)
            {
                if (loop < 1) loop = 1;

                Quaternion originalRotation = objTransform.rotation;
                float eulerAngleZ = objTransform.eulerAngles.z;
                float localEulerAngleZ = objTransform.localEulerAngles.z;

                float t = 0;
                for (int i = 0; i < loop; i++)
                {
                    while (t < waveDuration )
                    {
                        int dir = i % 2 == 0 ? 1 : -1;
                        float deltaT = (waveDuration * 0.5f) - Mathf.Abs(t - (waveDuration * 0.5f));
                        float angle = deltaT / waveDuration * shakeAngle * dir;
                        if (originLocal)
                        {
                            objTransform.localRotation = Quaternion.Euler(0, 0, angle + localEulerAngleZ);
                        }
                        else
                        {
                            objTransform.rotation = originalRotation * Quaternion.Euler(0, 0, angle + eulerAngleZ);
                        }
                        t += Time.deltaTime;
                        yield return null;
                    }
                    t %= waveDuration;

                    //yield return new WaitForSeconds(shakeDelay);
                }

                // Reset rotation after shaking
                if (originLocal)
                {
                    objTransform.localRotation = Quaternion.identity;
                }
                else
                {
                    objTransform.rotation = originalRotation;
                }
            }
        }


        public class Shuffle
        {
            public static void ShuffleArrayElements<T>(T[] array)
            {
                for (int i = 0; i < array.Length - 1; i++)
                {
                    int j = UnityEngine.Random.Range(i + 1, array.Length);
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }

            public static void ShuffleListElements<T>(List<T> li)
            {
                for (int i = 0; i < li.Count - 1; i++)
                {
                    int j = UnityEngine.Random.Range(i + 1, li.Count);
                    var temp = li[i];
                    li[i] = li[j];
                    li[j] = temp;
                }
            }
        }

        public class RectTransformPluss
        {
            public static void SetRectTransformMaxSize(RectTransform rect, Vector2 nativeSize, float maxSize)
            {
                float currentDeltaSize = nativeSize.x > nativeSize.y ? nativeSize.x : nativeSize.y;

                rect.sizeDelta = nativeSize * maxSize / (float)currentDeltaSize;
            }
        }

        public class PlayerPrefPluss
        {
            public static bool GetBool(string key)
            {
                return PlayerPrefs.GetInt(key) == 1;
            }

            public static void SetBool(string key, bool value)
            {
                PlayerPrefs.SetInt(key, value ? 1 : 0);
            }
        }

        public class LoadResources
        {
            public static UnityEngine.Object Load(string path)
            {
                return Resources.Load(path);
            }
            public static string GetObjectPathInResourcesFolder(UnityEngine.Object obj)
            {
#if UNITY_EDITOR
                string path = AssetDatabase.GetAssetPath(obj);

                int resourcesIndex = path.IndexOf("Resources/");
                if (resourcesIndex >= 0)
                {
                    path = path.Substring(resourcesIndex + "Resources/".Length);

                    int dotIndex = path.LastIndexOf('.');
                    if (dotIndex >= 0)
                    {
                        path = path.Substring(0, dotIndex);
                    }
#if LOG_INFO
                    Debug.Log("Resources/" + path);
#endif

                }
                else
                {
#if LOG_INFO
                    Debug.LogWarning("Object Is Not Contain In Resources Folder.");
#endif
                }
                return path;
#else
                return obj.name;
#endif
            }
        }

    }
    
}
