using System.IO;
using UnityEngine;

namespace Animator_Enum_Codegen.Editor
{
    public class AnimatorStatesConfig : ScriptableObject
    {
        private static AnimatorStatesConfig _instance;

        public static AnimatorStatesConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<AnimatorStatesConfig>("AnimatorStatesConfig");

                    if (_instance == null)
                    {
                        _instance = CreateInstance<AnimatorStatesConfig>();
#if UNITY_EDITOR
                        if (!Directory.Exists("Assets/Resources")) Directory.CreateDirectory("Assets/Resources");
                        UnityEditor.AssetDatabase.CreateAsset(_instance, "Assets/Resources/AnimatorStatesConfig.asset");
                        UnityEditor.AssetDatabase.SaveAssets();
#endif
                    }
                }

                return _instance;
            }
        }
        
        public string folderPath = "Assets/_Source/_Code/_Generated";
        public string @namespace  = "GeneratedData";
        public string className  = "AnimatorParams";
    }
}