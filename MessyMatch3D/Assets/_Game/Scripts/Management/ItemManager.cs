using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using _Game.Scripts.Items;
using _Game.Scripts.Data;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages the creation and management of items in the game.
    /// Ensures items are created at specified intervals and organized into required and normal categories.
    /// </summary>
    public class ItemManager : MonoBehaviour
    {
        [Header("Level Configuration")]
        [SerializeField]
        private LevelConfig _levelConfig;

        [Header("Item Lists")]
        [SerializeField]
        private List<Item> _generatedItems = new List<Item>();
        [SerializeField]
        private List<Item> _activeItems = new List<Item>();
        [SerializeField]
        private List<Item> _activeRequireItems = new List<Item>();
        [SerializeField]
        private List<Item> _activeNormalItems = new List<Item>();
        [SerializeField]
        private List<Item> _collectedItems = new List<Item>();

        [Header("Item Creation Settings")]
        [Tooltip("Time interval between creating each item.")]
        [SerializeField, Range(0.001f, 0.1f)]
        private float _itemCreationInterval = 0.1f;

        [Header("Spawn Area Settings")]
        [Tooltip("Minimum and maximum spawn positions on the horizontal axis.")]
        [SerializeField]
        private float _minHorizontalPosition = -5f;
        [SerializeField]
        private float _maxHorizontalPosition = 5f;
        [Tooltip("Minimum and maximum spawn positions on the vertical axis.")]
        [SerializeField]
        private float _minVerticalPosition = -5f;
        [SerializeField]
        private float _maxVerticalPosition = 5f;

        [Header("Spawn Height")]
        [Tooltip("Height at which items are spawned.")]
        [SerializeField]
        private float _spawnHeight = 1f;

        private void Start()
        {
            StartCoroutine(CreateItemsRoutine());
        }

        /// <summary>
        /// Coroutine to handle the creation of items at intervals and their categorization.
        /// </summary>
        private IEnumerator CreateItemsRoutine()
        {
            List<int> createdItemsCount = new List<int>();
            foreach (var itemData in _levelConfig.ItemDataList)
            {
                createdItemsCount.Add(0);
            }

            int totalItemsToCreate = 0;
            foreach (var itemData in _levelConfig.ItemDataList)
            {
                totalItemsToCreate += GetValidatedItemCount(itemData.ItemCount);
            }

            for (int i = 1; i <= totalItemsToCreate; i++)
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, _levelConfig.ItemDataList.Count);
                }
                while (createdItemsCount[randomIndex] >= GetValidatedItemCount(_levelConfig.ItemDataList[randomIndex].ItemCount));

                CreateItem(_levelConfig.ItemDataList[randomIndex], createdItemsCount[randomIndex] + 1);
                createdItemsCount[randomIndex]++;

                yield return new WaitForSeconds(_itemCreationInterval);
            }

            CategorizeItems();

            // Log the categorized items to the console.
            LogItemsToConsole();
        }

        /// <summary>
        /// Validates item count to ensure it's a multiple of three.
        /// </summary>
        /// <param name="requestedCount">The initially requested item count.</param>
        /// <returns>Adjusted item count, which is a multiple of three.</returns>
        private int GetValidatedItemCount(int requestedCount)
        {
            return (requestedCount + 2) / 3 * 3; // Simplified adjustment
        }

        /// <summary>
        /// Creates an item and assigns it a unique name based on the current item set and item number.
        /// Adds the item to the active list.
        /// </summary>
        /// <param name="itemData">The data of the item to create.</param>
        /// <param name="itemNumber">The item's number in the current set.</param>
        private void CreateItem(LevelConfig.ItemData itemData, int itemNumber)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(_minHorizontalPosition, _maxHorizontalPosition),
                _spawnHeight,
                Random.Range(_minVerticalPosition, _maxVerticalPosition)
            );

            Item newItem = Instantiate(itemData.ItemPrefab, spawnPosition, Quaternion.identity, transform);
            newItem.name = $"{itemData.ItemPrefab.GetType().Name}_{itemNumber}";

            _generatedItems.Add(newItem);
            _activeItems.Add(newItem);

            // Optional: Animate item creation with DOTween
            newItem.transform.DOScale(Vector3.one, _itemCreationInterval);
        }

        /// <summary>
        /// Categorizes active items into required and normal lists based on their configuration.
        /// </summary>
        private void CategorizeItems()
        {
            _activeRequireItems.Clear();
            _activeNormalItems.Clear();

            foreach (var item in _activeItems)
            {
                var itemData = _levelConfig.ItemDataList.Find(data => data.ItemPrefab.ItemId == item.ItemId);
                if (itemData != null && itemData.IsRequired)
                {
                    _activeRequireItems.Add(item);
                }
                else
                {
                    _activeNormalItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Logs the categorized items to the console.
        /// </summary>
        private void LogItemsToConsole()
        {
            Debug.Log("Require Items:");
            foreach (var item in _activeRequireItems)
            {
                Debug.Log(item.name);
            }

            Debug.Log("Normal Items:");
            foreach (var item in _activeNormalItems)
            {
                Debug.Log(item.name);
            }
        }

        /// <summary>
        /// Collects an item, moving it from the active list to the collected list.
        /// </summary>
        /// <param name="collectedItem">The item to be collected.</param>
        public void CollectItem(Item collectedItem)
        {
            if (_activeItems.Remove(collectedItem))
            {
                _collectedItems.Add(collectedItem);
            }
        }

        /// <summary>
        /// Recycles an item, moving it from the collected list back to the active list.
        /// </summary>
        /// <param name="recycledItem">The item to be recycled.</param>
        public void RecycleItem(Item recycledItem)
        {
            if (_collectedItems.Remove(recycledItem))
            {
                _activeItems.Add(recycledItem);
            }
        }

        /// <summary>
        /// Draws the spawn area in the Unity Editor using Gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireCube(
                new Vector3(
                    (_minHorizontalPosition + _maxHorizontalPosition) / 2f,
                    0f,
                    (_minVerticalPosition + _maxVerticalPosition) / 2f
                ),
                new Vector3(
                    _maxHorizontalPosition - _minHorizontalPosition,
                    1f,
                    _maxVerticalPosition - _minVerticalPosition
                )
            );
        }
    }
}