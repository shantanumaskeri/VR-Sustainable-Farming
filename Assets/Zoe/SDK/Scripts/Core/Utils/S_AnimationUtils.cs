using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public class S_AnimationUtils
    {
        public static List<string> FindAnimatorTriggers(Animator _animator)
        {
            List<string> toReturn = new List<string>();

            if (Application.isPlaying)
            {
                if (_animator != null && !_animator.isInitialized)
                    _animator.Rebind();
            }
            else
            {
                if (_animator != null)
                    _animator.Rebind();
            }

            if (_animator != null && _animator.isInitialized && _animator.isActiveAndEnabled)
            {
                for (int i = 0; i < _animator.parameters.Length; i++)
                {
                    AnimatorControllerParameter p = _animator.parameters[i];
                    if (p.type == AnimatorControllerParameterType.Trigger)
                        toReturn.Add(p.name);
                }
            }
            return toReturn;
        }

        public static List<AnimationClip> FindAnimatorClips(Animator _animator)
        {
            List<AnimationClip> toReturn = new List<AnimationClip>();
            if (_animator != null && _animator.isInitialized && _animator.runtimeAnimatorController != null && _animator.runtimeAnimatorController.animationClips != null)
            {
                for (int i = 0; i < _animator.runtimeAnimatorController.animationClips.Length; i++)
                    toReturn.Add(_animator.runtimeAnimatorController.animationClips[i]);
            }
            return toReturn;
        }

        public static List<string> FindAnimatorClipsNames(Animator _animator)
        {
            List<string> toReturn = new List<string>();
            if (_animator != null && _animator.isInitialized && _animator.runtimeAnimatorController != null && _animator.runtimeAnimatorController.animationClips != null)
            {
                for (int i = 0; i < _animator.runtimeAnimatorController.animationClips.Length; i++)
                    toReturn.Add(_animator.runtimeAnimatorController.animationClips[i].name);
            }
            return toReturn;
        }

        public static List<Animator> FindAnimatorsInHierarchy(GameObject _interaction)
        {
            List<Animator> toReturn = new List<Animator>();
            Animator[] ac = _interaction.GetComponentInParent<S_InteractiveObject>().GetComponentsInChildren<Animator>();
            
            foreach (Animator a in ac)
            {
                if (a.runtimeAnimatorController != null)
                    toReturn.Add(a);
            }
            return toReturn;
        }

        public static List<string> FindAnimatorsNames(GameObject _interaction)
        {
            List<string> toReturn = new List<string>();
            Animator[] ac = _interaction.GetComponentInParent<S_InteractiveObject>().GetComponentsInChildren<Animator>();

            foreach (Animator a in ac)
            {
                if (a.runtimeAnimatorController != null)
                    toReturn.Add(a.runtimeAnimatorController.name + " (" + a.gameObject.name + ")");
            }
            return toReturn;
        }
    }
}
