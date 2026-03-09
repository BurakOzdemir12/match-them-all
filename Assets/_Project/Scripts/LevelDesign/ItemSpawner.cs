using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.ItemScripts.ScriptableObjects;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Static;
using _Project.Scripts.Structs.Level;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.LevelDesign
{
    public class ItemSpawner : MonoBehaviour
    {
        [Header("Item Spawn & Place Settings ")] [SerializeField]
        private BoxCollider spawnZone;

        // [SerializeField] private float spawnDelay = 0.05f;

        [Header("Item Database")] [Tooltip("All Items in Database")] [SerializeField]
        private ItemDataSo itemDatabase;

        [Header("Items Parent")] [SerializeField]
        private Transform itemsParent;

        // private Coroutine spawnCoroutine;
        [Header("Scatter Settings")] [Tooltip("Scatter force")] [Range(0, 10)] [SerializeField]
        private float scatterForce = 2f;

        [SerializeField] private float scatterRadius = 5f;

        private void Awake()
        {
            if (spawnZone == null) spawnZone = GetComponent<BoxCollider>();
            spawnZone.isTrigger = true;
        }

        private void OnEnable()
        {
            GameEvents.OnLevelStarted += HandleLevelStarted;
        }

        private void HandleLevelStarted(LevelDataSo data)
        {
            ClearBoard();

            SpawnAllItemsInstantly(data);
            // if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
            // spawnCoroutine = StartCoroutine(SpawnItemRoutine(data));
        }

        private void ClearBoard()
        {
            foreach (Transform child in itemsParent)
            {
                Destroy(child.gameObject);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SpawnAllItemsInstantly(LevelDataSo data)
        {
            Bounds bounds = spawnZone.bounds;
            Vector3 spawnCenter = bounds.center;

            List<Rigidbody> spawnedRigidbodies = new List<Rigidbody>();
            foreach (var itemData in data.ItemLevelDataList)
            {
                Item prefabToSpawn = GetItemPrefabByType(itemData.ItemType);
                if (prefabToSpawn == null) continue;

                for (int i = 0; i < itemData.Amount; i++)
                {
                    Vector3 randomPos = new Vector3(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y),
                        Random.Range(bounds.min.z, bounds.max.z)
                    );
                    Item spawnedItem = Instantiate(prefabToSpawn, randomPos, Random.rotation, itemsParent);

                    if (spawnedItem.TryGetComponent(out Rigidbody rb))
                    {
                        spawnedRigidbodies.Add(rb);
                    }
                    // yield return new WaitForSeconds(spawnDelay);
                }
            }

            foreach (var rb in spawnedRigidbodies)
            {
                rb.AddExplosionForce(scatterForce, spawnCenter, scatterRadius, 0.5f, ForceMode.Impulse);
            }
        }

        private Item GetItemPrefabByType(ItemType itemType)
        {
            var dataEntry = itemDatabase.GetItemData(itemType);
            if (dataEntry.itemPrefab == null)
            {
                Debug.LogError($"ItemSpawner: didn't find prefab for {itemType}  ");
                return null;
            }

            return dataEntry.itemPrefab;
        }

        private void OnDisable()
        {
            GameEvents.OnLevelStarted -= HandleLevelStarted;
        }
    }
}