using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Structs;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts.Managers
{
    [RequireComponent(typeof(ParticleSystem))]
    public class EffectEmitter : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private IObjectPool<EffectEmitter> _pool;
        private Coroutine _playingCoroutine;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();

            var mainParticle = _particleSystem.main;
            mainParticle.stopAction = ParticleSystemStopAction.None;
        }

        public void Initialize(EffectData data, IObjectPool<EffectEmitter> poolRef)
        {
            this._pool = poolRef;
            //! If it's UI effect it must be a child of the (Canvas) UI root
            if (data.ParentTransform != null)
            {
                transform.SetParent(data.ParentTransform);
                transform.SetAsLastSibling();
            }

            transform.position = data.Position;
            transform.rotation = data.Rotation;
            transform.localScale = data.Scale;

            // var main = _particleSystem.main;
            // main.startColor = data.EffectColor;
        }

        public void Play()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
            }

            _particleSystem.Play(true);
            _playingCoroutine = StartCoroutine(WaitForEffectEnd());
        }

        private IEnumerator WaitForEffectEnd()
        {
            yield return new WaitWhile(() => _particleSystem.IsAlive(true));
            Stop();
        }

        public void Stop()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }

            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _pool?.Release(this);
        }
    }
}