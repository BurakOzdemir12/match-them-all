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

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();

            var mainParticle = _particleSystem.main;
            mainParticle.stopAction = ParticleSystemStopAction.None;
        }

        public void Initialize(EffectData data, IObjectPool<EffectEmitter> poolRef)
        {
            this._pool = poolRef;

            transform.position = data.Position;
            transform.rotation = data.Rotation;
            transform.localScale = data.Scale;

            // var main = _particleSystem.main;
            // main.startColor = data.EffectColor;
        }

        public void Play()
        {
            _particleSystem.Play(true);
            StartCoroutine(WaitForEffectEnd());
        }

        private IEnumerator WaitForEffectEnd()
        {
            yield return new WaitWhile(() => _particleSystem.IsAlive(true));
            Stop();
        }

        public void Stop()
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _pool?.Release(this);
        }
    }
}