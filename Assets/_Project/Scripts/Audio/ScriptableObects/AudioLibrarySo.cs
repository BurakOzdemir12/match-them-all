using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using UnityEngine;

namespace _Project.Scripts.Audio.ScriptableObects
{
    [System.Serializable]
    public struct SoundEntry
    {
        public string name;
        public SoundType soundType;
        public AudioClip clip;
        public bool isFrequent;
        [Range(0f, 1f)] public float volume;
        [Range(0f, 1.2f)] public float pitch;
    }

    [CreateAssetMenu(fileName = "newAudioLibrary", menuName = "ScriptableObjects/Audio/Audio Library")]
    public class AudioLibrarySo : ScriptableObject
    {
        public List<SoundEntry> soundEntries = new List<SoundEntry>();

        public SoundEntry GetSound(SoundType soundType)
        {
            return soundEntries.Find(entry => entry.soundType == soundType);
        }
    }
}