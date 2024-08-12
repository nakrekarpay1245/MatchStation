using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Items;
using _Game.Scripts._Data;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages the creation of items at specified intervals and ensures that items are created in multiples of three.
    /// </summary>
    public class ItemManager : MonoBehaviour
    {
        [Header("Level Config")]
        [SerializeField]
        private LevelConfig _levelConfig;

        [Header("Item Creation Settings")]
        [Tooltip("The time interval (in seconds) between creating each item.")]
        [SerializeField, Range(0.01f, 1f)]
        private float _itemCreationInterval = 0.1f;

        [Header("Spawn Area Settings")]
        [Tooltip("Minimum horizontal spawn position.")]
        [SerializeField]
        private float _minHorizontalPosition = -5f;

        [Tooltip("Maximum horizontal spawn position.")]
        [SerializeField]
        private float _maxHorizontalPosition = 5f;

        [Tooltip("Minimum vertical spawn position.")]
        [SerializeField]
        private float _minVerticalPosition = -5f;

        [Tooltip("Maximum vertical spawn position.")]
        [SerializeField]
        private float _maxVerticalPosition = 5f;

        [Header("Spawn Height Settings")]
        [Tooltip("Generation Height")]
        [SerializeField]
        private float _generationHeight = 1f;

        private int _currentItemSet = 1;

        private void Start()
        {
            // Start the item creation process
            StartCoroutine(CreateItemsRoutine());
        }

        /// <summary>
        /// Coroutine that handles the random creation of items based on the specified interval.
        /// </summary>
        private IEnumerator CreateItemsRoutine()
        {
            // Create a list to track how many items have been created from each data entry
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
                // Randomly select an item data entry that hasn't reached its creation limit
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, _levelConfig.ItemDataList.Count);
                }
                while (createdItemsCount[randomIndex] >= GetValidatedItemCount(_levelConfig.ItemDataList[randomIndex].ItemCount));

                // Create the item
                CreateItem(_levelConfig.ItemDataList[randomIndex].ItemPrefab, createdItemsCount[randomIndex] + 1);

                // Increment the count for this item data entry
                createdItemsCount[randomIndex]++;

                yield return new WaitForSeconds(_itemCreationInterval);
            }

            _currentItemSet++;
        }

        /// <summary>
        /// Ensures that the item count is a multiple of three, adjusting if necessary.
        /// </summary>
        /// <param name="requestedCount">The originally requested item count.</param>
        /// <returns>The adjusted item count, which will be a multiple of three.</returns>
        private int GetValidatedItemCount(int requestedCount)
        {
            if (requestedCount % 3 != 0)
            {
                int adjustedCount = requestedCount + (3 - (requestedCount % 3));
                //Debug.LogWarning($"Item count adjusted to {adjustedCount} to be a multiple of 3.");
                return adjustedCount;
            }
            return requestedCount;
        }

        /// <summary>
        /// Creates an item and assigns it a unique name based on the current item set and item number.
        /// </summary>
        /// <param name="itemPrefab">The item to create.</param>
        /// <param name="itemNumber">The number of the item in the current set.</param>
        private void CreateItem(Item itemPrefab, int itemNumber)
        {
            // Generate a random position within the specified bounds
            Vector3 spawnPosition = new Vector3(
                Random.Range(_minHorizontalPosition, _maxHorizontalPosition),
                _generationHeight,
                Random.Range(_minVerticalPosition, _maxVerticalPosition)
            );

            // Instantiate and animate the item
            Item newItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity, transform);
            newItem.name = $"{itemPrefab.GetType().Name}_{_currentItemSet}_{itemNumber}";

            // Additional initialization for the item if needed
        }

        /// <summary>
        /// Draws the spawn area in the Unity Editor using Gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            // Draw the rectangular spawn area based on the min and max values
            Gizmos.DrawWireCube(
                new Vector3((_minHorizontalPosition + _maxHorizontalPosition) / 2f, 0f,
                            (_minVerticalPosition + _maxVerticalPosition) / 2f),
                new Vector3(_maxHorizontalPosition - _minHorizontalPosition, 1f,
                            _maxVerticalPosition - _minVerticalPosition)
            );
        }
    }
}