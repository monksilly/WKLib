using UnityEngine;
using WKLib.Core.Classes;

namespace WKLib.API.Audio;

public static class AudioUtility
{
    public static AudioSource PlaySound(
        AudioClip clip,
        Vector3 position,
        Transform parent = null,
        float volume = 1f,
        float pitch = 1f,
        bool loop = false, 
        float spatial = 1f,
        float reverbMix = 1f,
        bool bypassEffects = false,
        float minDistance = 0,
        float? maxDistance = null,
        float? dopplerLevel = null,
        float? spread = null,
        AudioRolloffMode? rolloffMode = null,
        AnimationCurve customRolloffCurve = null,
        AudioMixerType mixerType = AudioMixerType.Sfx,
        string sourceType = "")
    {
        if(!Core.Classes.AudioManager.Instance)
            Core.Classes.AudioManager.CreateAudioManager();

        return Core.Classes.AudioManager.Instance.PlaySound(
            clip, 
            position, 
            parent, 
            volume, 
            pitch, 
            loop, 
            spatial, 
            reverbMix, 
            bypassEffects, 
            minDistance, 
            maxDistance,
            dopplerLevel, 
            spread, 
            rolloffMode,
            customRolloffCurve,
            mixerType, 
            sourceType);
    }
}