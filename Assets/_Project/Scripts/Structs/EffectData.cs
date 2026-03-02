using UnityEngine;

namespace _Project.Scripts.Structs
{
    public struct EffectData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public EffectData(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
}