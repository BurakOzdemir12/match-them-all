using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Structs;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        private AudioSource _audioSource;
        private IObjectPool<SoundEmitter> _pool;
        private Coroutine _playingCoroutine;

        public LinkedListNode<SoundEmitter> PoolNode { get; set; }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Initialize(SoundData data, IObjectPool<SoundEmitter> poolRef)
        {
            this._pool = poolRef;
            transform.position = data.Position;

            _audioSource.clip = data.Clip;
            _audioSource.volume = data.Volume;
            _audioSource.pitch = data.Pitch;
            
            _audioSource.spatialBlend = 1f;
            _audioSource.minDistance = 1f;
            _audioSource.maxDistance = 25f;
            _audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }

        public void Play()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
            }

            _audioSource.Play();
            _playingCoroutine = StartCoroutine(WaitForSoundEnd());
        }

        private IEnumerator WaitForSoundEnd()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            Stop();
        }

        public void Stop()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }

            _audioSource.Stop();

            _pool?.Release(this);
        }
    }
}