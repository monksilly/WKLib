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
        float maxDistance = 10,
        float dopplerLevel = 1f,
        float spread = 0f,
        AudioMixerType mixerType = AudioMixerType.Sfx,
        string sourceType = "")
    {
        if (!clip) return null;

        if (_audioSourcePool.Count == 0)
            CreateAudioSource();

        var source = _audioSourcePool.Dequeue();
        source.clip = clip;
        if(parent)
            source.transform.parent = parent;
        source.transform.position = position;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.spatialBlend = spatial;
        source.reverbZoneMix = reverbMix;
        source.bypassEffects = bypassEffects;
        source.dopplerLevel = dopplerLevel;
        source.spread = spread;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        if (!string.IsNullOrEmpty(sourceType) && global::AudioManager.sourceTypeDict.TryGetValue(sourceType, out var audioSourceType))
        {
            AudioSource audioSourcePrefab = audioSourceType.audioSourcePrefab;
            source.dopplerLevel = audioSourcePrefab.dopplerLevel;
            source.spread = audioSourcePrefab.spread;
            source.maxDistance = audioSourcePrefab.maxDistance;
            AnimationCurve customCurve = audioSourcePrefab.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
            source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customCurve);
        }

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