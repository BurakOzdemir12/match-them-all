using System.Collections.Generic;
using _Project.Scripts.Structs.Level;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.LevelDesign.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "ScriptableObjects/Game/Level Data")]
    public class LevelDataSo : ScriptableObject
    {
        [field: Header("Level Info")]
        [field: SerializeField]
        public int LevelIndex { get; private set; }

        [field: SerializeField] public float LevelTimeLimit { get; set; }


        [Header("Spawn Elements")] [SerializeField]
        private List<ItemLevelData> itemLevelDataList;

        public IReadOnlyList<ItemLevelData> ItemLevelDataList => itemLevelDataList;
    }
}