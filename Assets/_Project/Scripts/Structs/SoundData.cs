using UnityEngine;

namespace _Project.Scripts.Structs
{
    public struct SoundData
    {
        public AudioClip Clip;
        public Vector3 Position;
        public float Volume;
        public float Pitch;
        public bool IsFrequent; 

        public SoundData(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, bool isFrequent = true)
        {
            Clip = clip;
            Position = position;
            Volume = volume;
            Pitch = pitch;
            IsFrequent = isFrequent;
        }
    }
}