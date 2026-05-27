using System;
using System.Collections.Generic;
using UnityEngine;

namespace WKLib.Core.Classes;

public enum AudioMixerType
{
    Music,
    Sfx,
    UI,
}

internal class AudioManager : MonoSingleton<AudioManager>
{
    private Queue<AudioSource> _audioSourcePool = new();
    private List<AudioSource> _playingSources = new();

    public static void CreateAudioManager()
    {
        if (Instance) return;
        Instance = new GameObject("AudioManager").AddComponent<AudioManager>();
    }

    public AudioSource PlaySound(
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
        if (!clip) return null;

        if (_audioSourcePool.Count == 0)
            CreateAudioSource();

        var source = _audioSourcePool.Dequeue();
        source.clip = clip;
        source.transform.position = position;
        if(parent)
            source.transform.SetParent(parent, true);
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.spatialBlend = spatial;
        source.reverbZoneMix = reverbMix;
        source.bypassEffects = bypassEffects;
        source.minDistance = minDistance;

        if (!string.IsNullOrEmpty(sourceType) && global::AudioManager.sourceTypeDict.TryGetValue(sourceType, out var audioSourceType))
        {
            AudioSource audioSourcePrefab = audioSourceType.audioSourcePrefab;
            source.dopplerLevel = dopplerLevel ?? audioSourcePrefab.dopplerLevel;
            source.spread = spread ?? audioSourcePrefab.spread;
            source.maxDistance = maxDistance ?? audioSourcePrefab.maxDistance;
            if (rolloffMode == null)
            {
                AnimationCurve customCurve = audioSourcePrefab.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
                source.rolloffMode = AudioRolloffMode.Custom;
                source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customRolloffCurve ?? customCurve);
            }
        }
        else
        {
            AudioSource defaultAudio = global::AudioManager.defaultAudio;
            source.dopplerLevel = dopplerLevel ?? defaultAudio.dopplerLevel;
            source.spread = spread ?? defaultAudio.spread;
            source.maxDistance = maxDistance ?? defaultAudio.maxDistance;
            if (rolloffMode == null)
            {
                AnimationCurve customCurve = defaultAudio.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
                source.rolloffMode = AudioRolloffMode.Custom;
                source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customRolloffCurve ?? customCurve);
            }
        }
        
        source.rolloffMode = rolloffMode ?? source.rolloffMode;

        source.outputAudioMixerGroup = mixerType switch
        {
            AudioMixerType.Music => global::AudioManager.instance.soundtrackMixer,
            AudioMixerType.Sfx => global::AudioManager.instance.gameMixer,
            AudioMixerType.UI => global::AudioManager.instance.UIMixer,
            _ => source.outputAudioMixerGroup
        };

        source.gameObject.SetActive(true);
        _playingSources.Add(source);
        source.Play();

        return source;
    }

    private void CreateAudioSource()
    {
        var source = new GameObject("AudioSource").AddComponent<AudioSource>();
        source.transform.SetParent(transform);
        source.playOnAwake = false;
        _audioSourcePool.Enqueue(source);
    }

    private void SendBackToPool(AudioSource source)
    {
        _audioSourcePool.Enqueue(source);
        source.transform.SetParent(transform); 
        source.gameObject.SetActive(false);
    }

    private void Update()
    {
        for (int sourceIndex = _playingSources.Count - 1; sourceIndex >= 0; sourceIndex--)
        {
            var source = _playingSources[sourceIndex];
            if (!source)
            {
                _playingSources.RemoveAt(sourceIndex);
                continue;
            }
            if (!source.isPlaying)
            {
                _playingSources.RemoveAt(sourceIndex);
                SendBackToPool(source);
            }
        }
    }
}