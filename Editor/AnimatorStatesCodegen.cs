﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Animator_Enum_Codegen.Editor
{
    public static class AnimatorStatesCodegen
    {
        [MenuItem("Tools/Animator-Enum-Codegen/Create Config")]
        public static void GenerateConfig()
        {
            var config = AnimatorStatesConfig.Instance;
        }
        
        [MenuItem("Tools/Animator-Enum-Codegen/Generate Animator States")]
        public static void ScanAnimationControllers()
        {
            string[] guids = AssetDatabase.FindAssets("t:animatorcontroller");

            var config = AnimatorStatesConfig.Instance;
            
            var folderPath = config.folderPath;
            var className  = config.className;
            var @namespace = config.@namespace;
            
            var enumDefinitions  = new List<string>();
            var stateNamesLists  = new List<string>();
            var stateHashesLists = new List<string>();
            var infoDictionary   = new List<string>();
            
            var nameCountMap = new Dictionary<string, int>();

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

                var layer = controller.layers[0];
                var states = layer.stateMachine.states;

                var cleanName = CleanText(controller.name);
                
                if (nameCountMap.ContainsKey(cleanName)) //Duplicate names check
                {
                    var count = nameCountMap[cleanName];
                    count++;
                    nameCountMap[cleanName] = count;
                    cleanName               = cleanName + "_" + count;
                }
                else nameCountMap.Add(cleanName, 0);

                // Generate enum
                var enumName       = cleanName + "_States";
                var enumNames      = states.Select(s => CleanText(s.state.name)).ToList();
                var enumDefinition = $"public enum {enumName} {{ {string.Join(", ", enumNames)} }}";
                enumDefinitions.Add(enumDefinition);

                // Generate string list
                var listName = cleanName + "_Names";
                var stateNames = states.Select(s => s.state.name).ToList();
                var stateNamesList = $"public static readonly List<string> {listName} = new List<string>{{ {string.Join(", ", stateNames.Select(n => $"\"{n}\""))} }};";
                stateNamesLists.Add(stateNamesList);

                // Generate int list
                var hashesName = cleanName + "_Hash";
                var stateHashes = stateNames.Select(Animator.StringToHash).ToList();
                var stateHashesList = $"public static readonly List<int> {hashesName} = new List<int>{{ {string.Join(", ", stateHashes)} }};";
                stateHashesLists.Add(stateHashesList);
                
                // Generate info list
                string info = $"{{ typeof({enumName}), new StatesData({listName}, {hashesName}) }}";
                infoDictionary.Add(info);
            }

            string code = CodegenPasta.GetPasta(@namespace, className, 
                enumDefinitions, stateNamesLists, stateHashesLists, infoDictionary);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            File.WriteAllText($"{folderPath}/{className}.cs", code);
            
            AssetDatabase.Refresh();
        }

        private static string CleanText(string input)
        {
            var cleanedString = new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
            //var titleCaseString        = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleanedString.ToLower());
            var cleanedTitleCaseString = Regex.Replace(cleanedString, @"[^a-zA-Z0-9]", "");
            return cleanedTitleCaseString;
        }
    }

    public static class CodegenPasta
    {
        public static string GetPasta(
            string       @namespace,       string       className,
            List<string> enumDefinitions,  List<string> stateNamesLists,
            List<string> stateHashesLists, List<string> infoDictionary)
        { 
            return 
$@"// ------------------------------------------------------------------------------------
// This class is autogenerated by AnimatorStatesCodegen.cs, do not modify it
// Author - Bolttalk 2023 : https://github.com/SLiGerr
// ------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Animator_Enum_Codegen.Runtime;
using UnityEngine;

namespace {@namespace}
{{
    public static class {className}
    {{
        // Formatted enums
        {string.Join("\n\t\t", enumDefinitions)}

        // Raw names
        {string.Join("\n\t\t", stateNamesLists)}

        // int hashes
        {string.Join("\n\t\t", stateHashesLists)}

        // Typed Dictionary
        public static readonly Dictionary<Type, StatesData> Infos = new () 
        {{
            {string.Join(",\n\t\t\t", infoDictionary)}
        }};

        /// <summary>
        /// Shortcut to get state hash from enum value
        /// </summary>
        /// <param name=""state"">Generated animator state enum value</param>
        /// <typeparam name=""T"">Generated animator controller type</typeparam>
        /// <returns>State hash</returns>
        public static int GetHash<T>(T state) where T : Enum =>
            Infos[typeof(T)].Hashes[Convert.ToInt32(state)];

        /// <summary>
        /// Shortcut to get actual state name from enum value
        /// </summary>
        /// <param name=""state"">Generated animator state enum value</param>
        /// <typeparam name=""T"">Generated animator controller type</typeparam>
        /// <returns>State name</returns>
        public static string GetName<T>(T state) where T : Enum =>
            Infos[typeof(T)].Names[Convert.ToInt32(state)];

        /// <summary>
        /// Get state-durations dictionary table from animator by controller type enum 
        /// </summary>
        /// <param name=""animator"">Animator to collect info from</param>
        /// <param name=""durations"" >Output enum-float Dictionary </param >
        /// <typeparam name=""T"">Type of animator's generated ENUM</typeparam>
        public static void GatherStatesDurations<T>(this Animator animator, out Dictionary<T, float> durations) where T : Enum
        {{
            var animatorStates = animator.GetAnimatorStateInfo();
            var enumStates = Enum.GetValues(typeof(T));
            durations = new Dictionary<T, float>();
            
            foreach (T enumState in enumStates)
            foreach (var animatorState in animatorStates)
                if (GetName(enumState).Equals(animatorState.name))
                    durations.Add(enumState, animatorState.motion.averageDuration / animatorState.speed);
        }}
    }}
}}
";
        }
    }
}

