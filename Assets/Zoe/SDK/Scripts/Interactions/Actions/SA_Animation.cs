using Gaze;
using System;
using UnityEngine;

namespace SpatialStories
{
    [AddComponentMenu("Zoe/SA_Animation")]
    public class SA_Animation : S_AbstractAction
    {
        public enum LOOP_MODES { None, Single, Playlist, PlaylistOnce }
        public enum AUDIO_SEQUENCE { InOrder, Random }
        public enum ANIMATION_OPTION { NOTHING, MECANIM }
        public enum ANIMATION_LOOP { Loop, PingPong }
        public enum ANIMATION_ACTION { LAUNCH_ANIMATION, DEACTIVATE_ANIMATOR }

        public S_InteractiveObject Target;
        public Animator SelectedAnimator;
        public ANIMATION_ACTION AnimationAction;
        public int AnimationIndexAction;
        public ANIMATION_OPTION AnimationOption = ANIMATION_OPTION.MECANIM;
        public int AnimationIndexOption;
        public bool TriggerAnimation;
        public string[] AnimatorsAvailable = new string[Enum<TriggerEventsAndStates>.Count];
        public string[] AnimatorTriggers = new string[Enum<TriggerEventsAndStates>.Count];
        public bool[] LoopOnLast = new bool[Enum<TriggerEventsAndStates>.Count];

        public S_AnimationPlaylist AnimationClip = new S_AnimationPlaylist();

        public LOOP_MODES[] LoopAnim = new LOOP_MODES[Enum<TriggerEventsAndStates>.Count];
        public ANIMATION_LOOP[] LoopAnimType = new ANIMATION_LOOP[Enum<TriggerEventsAndStates>.Count];
        public AUDIO_SEQUENCE[] AnimationSequence = new AUDIO_SEQUENCE[Enum<TriggerEventsAndStates>.Count];

        private int m_AnimationPlayListKey;
        private S_AnimationPlayer m_GazeAnimationPlayer;

        [SerializeField]
        private Animator _targetAnimator;

        public Animator TargetAnimator
        {
            get
            {
                SetUpProperties();
                return _targetAnimator;
            }
        }

        private void SetUpProperties()
        {
            if (Target == null)
            {
                Target = this.gameObject.GetComponentInParent<S_InteractiveObject>();
                if (Target == null)
                {
                    if (Application.isPlaying)
                    {
                        throw new Exception("SA_Animation::SetUpProperties::The target is NULL");
                    }
                    else
                    {
                        return;
                    }
                }
            }

            _targetAnimator = Target.GetComponent<Animator>();

            if (SelectedAnimator != null)
            {
                _targetAnimator = SelectedAnimator;
            }

            if (_targetAnimator == null)
            {
                if (Application.isPlaying)
                {
                    throw new Exception("SA_Animation::SetUpProperties::The S_InteractiveObjectVisuals does NOT have in its components Animator");
                }
                else
                {
                    return;
                }
            }

            if (_targetAnimator.GetComponent<S_AnimationPlayer>() == null)
            {
                _targetAnimator.gameObject.AddComponent<S_AnimationPlayer>();
            }
        }

        private void Awake()
        {
            SetUpProperties();
        }

        protected override void ActionLogic()
        {
            if (AnimationAction == ANIMATION_ACTION.DEACTIVATE_ANIMATOR)
            {
                SetUpProperties();

                if (_targetAnimator != null)
                {
                    if (_targetAnimator.GetComponent<S_AnimationPlayer>() != null)
                    {
                        _targetAnimator.GetComponent<S_AnimationPlayer>().Stop();
                    }

                    _targetAnimator.enabled = false;
                }
            }
            else
            {
                PlayAnim(0);
            }
        }


        private void PlayAnim(int i)
        {
            if (AnimationOption == ANIMATION_OPTION.MECANIM)
            {
                _targetAnimator.enabled = true;
                _targetAnimator.SetTrigger(AnimatorTriggers[i]);
            }
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            string ioGUI = creationData[0].ToString();
            SpatialStoriesAPI.GetInteractiveObjectWithGUID(ioGUI);

            AnimationOption = (ANIMATION_OPTION)creationData[1];

            _targetAnimator = SpatialStoriesAPI.GetInteractiveObjectWithGUID(ioGUI).GetComponentInChildren<S_InteractiveObjectVisuals>().GetComponentInChildren<Animator>();

            switch (AnimationOption)
            {
                case ANIMATION_OPTION.MECANIM:
                    ApiSetUpClipTrigger(_targetAnimator);
                    break;
                default:
                    break;
            }
        }

        private void ApiSetUpClipAnimation(Animator _targetAnimator)
        {
            AnimationClip.Add(0, (AnimationClip)creationData[2]);
        }

        private void ApiSetUpClipTrigger(Animator _targetAnimator)
        {
            this.AnimatorTriggers = new string[] { creationData[2].ToString() };
        }
    }

    /// <summary>
    /// Wrapper for the API in order to create a gaze condition
    /// </summary>
    public static partial class APIExtensions
    {
        public static SA_Animation CreateTriggerAnimationAction(this S_InteractionDefinition _def, string _ioGUID, string _triggerName)
        {
            return _def.CreateAction<SA_Animation>(_ioGUID, SA_Animation.ANIMATION_OPTION.MECANIM, _triggerName);
        }
    }

}
