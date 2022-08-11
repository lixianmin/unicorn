
/********************************************************************
created:    2013-12-15
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using System.Collections.Generic;

namespace Unicorn
{
    //[Obfuscators.ObfuscatorIgnore]
    internal static class ExtendedAnimation
    { 
//        public static void SetAnimatePhysicsEx(this Animation animation, bool animatePhysics)
//        {
//            animation.animatePhysics = animatePhysics;
//        }
//
//        public static bool GetAnimatePhysicsEx(this Animation animation)
//        {
//            return animation.animatePhysics;
//        }
//
//        public static void SetClipEx(this Animation animation, AnimationClip clip)
//        {
//            animation.clip = clip;
//        }
//
//        public static AnimationClip GetClipEx(this Animation animation)
//        {
//            return animation.clip;
//        }
//
//        public static void SetCullingTypeEx(this Animation animation, AnimationCullingType cullingType)
//        {
//            animation.cullingType = cullingType;
//        }
//
//        public static AnimationCullingType GetCullingTypeEx(this Animation animation)
//        {
//            return animation.cullingType;
//        }
//
//        public static bool IsPlayingEx(this Animation animation)
//        {
//            return animation.isPlaying;
//        }
//
//        public static void SetLocalBoundsEx(this Animation animation, Bounds localBounds)
//        {
//            animation.localBounds = localBounds;
//        }
//
//        public static Bounds GetLocalBoundsEx(this Animation animation)
//        {
//            return animation.localBounds;
//        }
//
//        public static void SetPlayAutomaticallyEx(this Animation animation, bool playAutomatically)
//        {
//            animation.playAutomatically = playAutomatically;
//        }
//
//        public static bool GetPlayAutomaticallyEx(this Animation animation)
//        {
//            return animation.playAutomatically;
//        }
//
//        public static void SetWrapModeEx(this Animation animation, WrapMode wrapMode)
//        {
//            animation.wrapMode = wrapMode;
//        }
//
//        public static WrapMode GetWrapModeEx(this Animation animation)
//        {
//            return animation.wrapMode;
//        }
//
//        public static void AddClipEx(this Animation animation, AnimationClip clip, string newName)
//        {
//            animation.AddClip(clip, newName);
//        }
//
//        public static void AddClipEx(this Animation animation, AnimationClip clip, string newName, int firstFrame, int lastFrame, bool addLoopFrame= false)
//        {
//            animation.AddClip(clip, newName, firstFrame, lastFrame, addLoopFrame);
//        }
//
//        public static void BlendEx(this Animation animation, string animationName, float targetWeight= 1.0f, float fadeLength= 0.3f)
//        {
//            animation.Blend(animationName, targetWeight, fadeLength);
//        }
//
//        public static void CrossFadeEx(this Animation animation, string animationName, float fadeLength= 0.3f, PlayMode mode = PlayMode.StopSameLayer)
//        {
//            animation.CrossFade(animationName, fadeLength, mode);
//        }
//
//        public static void CrossFadeQueuedEx(this Animation animation, string animationName, float fadeLength= 0.3f, QueueMode queue= QueueMode.CompleteOthers, 
//                                            PlayMode mode= PlayMode.StopSameLayer)
//        {
//            animation.CrossFadeQueued(animationName, fadeLength, queue, mode);
//        }
//
//        public static void PlayEx(this Animation animation, PlayMode mode = PlayMode.StopSameLayer)
//        {
//            animation.Play(mode);
//        }
//
//        public static int GetClipCountEx(this Animation animation)
//        {
//            return animation.GetClipCount();
//        }
//
//        public static bool IsPlayingEx(this Animation animation, string name)
//        {
//            return animation.IsPlaying(name);
//        }
//
//        public static void PlayEx(this Animation animation, string animationName, PlayMode mode = PlayMode.StopSameLayer)
//        {
//            animation.Play(animationName, mode);
//        }
//
//        public static void PlayQueuedEx(this Animation animation, string animationName, QueueMode queue= QueueMode.CompleteOthers, PlayMode mode= PlayMode.StopSameLayer)
//        {
//            animation.PlayQueued(animationName, queue, mode);
//        }
//
//        public static void RemoveClipEx(this Animation animation, AnimationClip clip)
//        {
//            animation.RemoveClip(clip);
//        }
//
//        public static void RemoveClipEx(this Animation animation, string clipName)
//        {
//            animation.RemoveClip(clipName);
//        }
//
//        public static void RewindEx(this Animation animation, string name)
//        {
//            animation.Rewind(name);
//        }
//
//        public static void RewindEx(this Animation animation)
//        {
//            animation.Rewind();
//        }
//
//        public static void SampleEx(this Animation animation)
//        {
//            animation.Sample();
//        }
//
//        public static void StopEx(this Animation animation)
//        {
//            animation.Stop();
//        }
//
//        public static void StopEx(this Animation animation, string name)
//        {
//            animation.Stop(name);
//        }

        public static void CollectClipNamesEx (this Animation animation, List<string> names)
        {
            var e = animation.GetEnumerator();

            while (e.MoveNext())
            {
                var state = e.Current as AnimationState;
                var clip = state.clip;
                names.Add(clip.name);
            }
        }
    }
}