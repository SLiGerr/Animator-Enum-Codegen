using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace Animator_Enum_Codegen.Editor
{
    public static class AnimatorExt
    {
        //Thanks: https://www.reddit.com/r/Unity3D/comments/gigup7/is_there_no_way_to_get_information_from_an/
        public static List<AnimatorState> GetAnimatorStateInfo(this Animator animator)
        {
            var controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller != null)
            {
                var acLayers  = controller.layers;
                var allStates = new List<AnimatorState>();
                foreach (var i in acLayers)
                {
                    var animStates = i.stateMachine.states;
                    allStates.AddRange(animStates.Select(j => j.state));
                }

                return allStates;
            } else throw new NullReferenceException("animator is null or broken");
        }
        
        public static List<AnimatorState> GetAnimatorStateInfo(this AnimatorController controller)
        {
            if (controller != null)
            {
                var acLayers  = controller.layers;
                var allStates = new List<AnimatorState>();
                foreach (var i in acLayers)
                {
                    var animStates = i.stateMachine.states;
                    allStates.AddRange(animStates.Select(j => j.state));
                }

                return allStates;
            } else throw new NullReferenceException("animator is null or broken");
        }
    }
}