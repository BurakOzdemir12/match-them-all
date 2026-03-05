using _Project.Scripts.Enums;
using _Project.Scripts.Managers;

namespace _Project.Scripts.Structs
{
    [System.Serializable]
    public struct EffectPoolSetup
    {
        public EffectType EffectType;
        public EffectEmitter EmitterPrefab;

        public EffectPoolSetup(EffectType effectType, EffectEmitter emitterPrefab)
        {
            EffectType = effectType;
            EmitterPrefab = emitterPrefab;
        }
    }
}