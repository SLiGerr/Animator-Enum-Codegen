using System.Collections.Generic;
using System.Globalization;
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
        private const string FolderPath = "Assets/_Source/_Code/_Generated";
        private const string Namespace = "GeneratedData";
        private const string ClassName = "AnimatorParams";

        [MenuItem("Tools/Generate Animator States")]
        public static void ScanAnimationControllers()
        {
            string[] guids = AssetDatabase.FindAssets("t:animatorcontroller");
            
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
            
            string code = 
$@"using System;
using System.Collections.Generic;
using Animator_Enum_Codegen;

namespace {Namespace}
{{
    public static class {ClassName}
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
    }}
}}
";
            if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
            File.WriteAllText($"{FolderPath}/{ClassName}.cs", code);
            
            AssetDatabase.Refresh();
        }

        private static string CleanText(string input)
        {
            var cleanedString          = new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
            var titleCaseString        = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleanedString.ToLower());
            var cleanedTitleCaseString = Regex.Replace(titleCaseString, @"[^a-zA-Z0-9]", "");
            return cleanedTitleCaseString;
        }
    }
}

