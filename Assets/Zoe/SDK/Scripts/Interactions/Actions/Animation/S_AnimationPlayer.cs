using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public class S_AnimationPlayer : MonoBehaviour
    {
        private class S_Animation
        {
            public S_AnimationPlaylist AnimationClip = new S_AnimationPlaylist();
            public SA_Animation.ANIMATION_LOOP[] Loop = new SA_Animation.ANIMATION_LOOP[5];
            public SA_Animation.AUDIO_SEQUENCE[] Sequence = new SA_Animation.AUDIO_SEQUENCE[5];
            public SA_Animation.LOOP_MODES[] PlaylistLoop = new SA_Animation.LOOP_MODES[5];
            public bool[] LoopOnLast = new bool[5];

            public bool Looping = false;
            public int ClipIndex = -1;

            public int TrackPlaying = 0;
            public int Key = 0;

            public Animator MyAnimator;
            public bool IsPlaying = false;

            public bool Reversing = false;
        }

        private List<S_Animation> m_Animations = new List<S_Animation>();
        private bool m_Stopping;
        
        public int setParameters(Animator targetAnimator, S_AnimationPlaylist clips, SA_Animation.ANIMATION_LOOP[] loop, SA_Animation.LOOP_MODES[] playlistLoop, SA_Animation.AUDIO_SEQUENCE[] sequence, bool[] loopOnLast)
        {
            m_Animations.Add(new S_Animation());
            int key = m_Animations.Count - 1;
            m_Animations[key].Key = key;

            for (int i = 0; i < playlistLoop.Length; i++)
            {
                SA_Animation.LOOP_MODES lm = playlistLoop[i];
                if (lm == SA_Animation.LOOP_MODES.None)
                {
                    lm = SA_Animation.LOOP_MODES.Single;
                }
            }
            
            for (int i = 0; i < playlistLoop.Length; i++)
            {
                for (int k = 0; k < clips.Count(i); k++)
                {
                    if (clips.Get(i, k) != null)
                    {
                        m_Animations[key].AnimationClip.Add(i, clips.Get(i, k));
                    }
                }

                this.m_Animations[key].Sequence[i] = sequence[i];
                this.m_Animations[key].Loop[i] = loop[i];
                this.m_Animations[key].PlaylistLoop[i] = playlistLoop[i];
                this.m_Animations[key].LoopOnLast[i] = loopOnLast[i];
                this.m_Animations[key].MyAnimator = targetAnimator;
            }
            return key;
        }

        public bool ShouldUpdate = false;
        public void Update()
        {
            if (!ShouldUpdate)
                return;

            foreach (var anim in m_Animations)
            {
                if (anim.IsPlaying)
                {
                    if (m_Stopping)
                    {
                        anim.MyAnimator.enabled = false;
                        anim.Looping = false;
                        anim.IsPlaying = false;
                    }
                    else if (anim.Reversing)
                    {
                        anim.MyAnimator.playbackTime = 10;
                        Debug.Log(anim.MyAnimator.playbackTime);

                        if (anim.MyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0)
                        {
                            anim.MyAnimator.speed = 1f;
                            anim.MyAnimator.StopPlayback();
                            anim.Reversing = false;
                        }

                    }
                    else if (anim.MyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
                    {
                        if (anim.Looping)
                        {
                            if (anim.Loop[anim.TrackPlaying] == SA_Animation.ANIMATION_LOOP.PingPong)
                            {
                                anim.Reversing = true;
                                anim.MyAnimator.StartPlayback();
                                anim.MyAnimator.speed = -1f;
                                anim.MyAnimator.playbackTime = 1f;
                                Debug.Log(anim.MyAnimator.speed);
                            }
                        }

                        else if (anim.PlaylistLoop[anim.TrackPlaying] == SA_Animation.LOOP_MODES.None)
                        {
                            anim.MyAnimator.enabled = false;
                            anim.Looping = false;
                            anim.IsPlaying = false;
                        }

                        else if (anim.PlaylistLoop[anim.TrackPlaying] == SA_Animation.LOOP_MODES.Playlist)
                        {
                            nextClip(anim.Key, anim.TrackPlaying);
                            anim.MyAnimator.Play(anim.AnimationClip.Get(anim.TrackPlaying, anim.ClipIndex).name);
                        }

                        else if (anim.PlaylistLoop[anim.TrackPlaying] == SA_Animation.LOOP_MODES.PlaylistOnce)
                        {
                            if (anim.ClipIndex >= anim.AnimationClip.Count(anim.TrackPlaying))
                            {
                                if (anim.LoopOnLast[anim.TrackPlaying])
                                {
                                    anim.Looping = true;
                                    anim.MyAnimator.Play(anim.AnimationClip.Get(anim.TrackPlaying, anim.AnimationClip.Count(anim.TrackPlaying) - 1).name);
                                }
                                else
                                {
                                    anim.MyAnimator.enabled = false;
                                    anim.Looping = false;
                                    anim.IsPlaying = false;
                                }
                                anim.ClipIndex = 0;
                            }

                            else
                            {
                                anim.MyAnimator.Play(anim.AnimationClip.Get(anim.TrackPlaying, anim.ClipIndex).name);
                                anim.ClipIndex++;
                            }
                        }
                    }
                }
            }

            m_Stopping = false;
        }

        public void PlayAnim(int key, int track)
        {
            ShouldUpdate = true;
            //if (!animations[key].isPlaying)
            {
                nextClip(key, track);

                m_Animations[key].IsPlaying = true;
                m_Animations[key].MyAnimator.enabled = true;
                m_Animations[key].TrackPlaying = track;
                m_Animations[key].MyAnimator.speed = 1f;
                m_Animations[key].Reversing = false;


                if (m_Animations[key].PlaylistLoop[track] == SA_Animation.LOOP_MODES.Single)
                {
                    m_Animations[key].Looping = true;

                }
                else if (m_Animations[key].PlaylistLoop[track] == SA_Animation.LOOP_MODES.Playlist)
                {
                    m_Animations[key].Looping = false;

                }
                else if (m_Animations[key].PlaylistLoop[track] == SA_Animation.LOOP_MODES.PlaylistOnce)
                {
                    m_Animations[key].Looping = false;
                }
                else
                {
                    m_Animations[key].Looping = false;
                }
                m_Animations[key].ClipIndex %= m_Animations[key].AnimationClip.Count(track);
                // TODO @apelab crossfade on animation type CLIP
                m_Animations[key].MyAnimator.CrossFade(m_Animations[key].AnimationClip.Get(track, m_Animations[key].ClipIndex).name, 0f);
            }
        }

        public void Stop()
        {
            m_Stopping = true;

            foreach (var anim in m_Animations)
            {
                if (anim.IsPlaying)
                {
                    if (m_Stopping)
                    {
                        anim.MyAnimator.StopPlayback();
                    }
                }
            }
        }

        private void nextClip(int key, int track)
        {
            if (m_Animations[key].Sequence[track] == SA_Animation.AUDIO_SEQUENCE.Random)
            {
                m_Animations[key].ClipIndex += UnityEngine.Random.Range(0, m_Animations[key].AnimationClip.Count(track));
            }
            else
            {
                m_Animations[key].ClipIndex++;
            }
            m_Animations[key].ClipIndex %= m_Animations[key].AnimationClip.Count(track);
        }

    }

}

