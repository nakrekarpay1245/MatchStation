using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using _Game.Scripts.Items;
using UnityEngine.Events;
using _Game.Scripts._Data;
using DG.Tweening; // DOTween k�t�phanesi i�in

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages the level and UI elements using DOTween for smooth animations.
    /// Manages the level progress, including item collection requirements and UI indicators.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [Header("LevelManager Parameters")]
        [Header("Level Configuration")]
        [Tooltip("Reference to the level configuration.")]
        [SerializeField] private LevelConfig _levelConfig;

        private float _currentLevelTime;
        private bool _isTimerRunning;

        [Header("Indicator Settings")]
        [Tooltip("Parent object for the item indicators.")]
        [SerializeField]
        private Transform _indicatorsParent;

        [Tooltip("Prefab for the item indicator.")]
        [SerializeField]
        private ItemIndicator _indicatorPrefab;

        private Dictionary<int, ItemIndicator> _itemIndicators = new Dictionary<int, ItemIndicator>();
        private Dictionary<int, int> _requiredItemCounts = new Dictionary<int, int>();

        private int _currentLevelIndex = 0;

        public UnityAction<float, float> OnTimerUpdated;
        public UnityAction OnLevelFailed;
        public UnityAction OnLevelCompleted;

        private void Start()
        {
            StartTimer(_levelConfig.InitialTime);

            CreateItemIndicators();
        }

        private void CreateItemIndicators()
        {
            // Clear existing item indicators and required item counts
            _itemIndicators.Clear();
            _requiredItemCounts.Clear();

            foreach (var itemData in _levelConfig.ItemDataList)
            {
                if (itemData.IsRequired)
                {
                    // Instantiate indicator prefab
                    var currentIndicator = Instantiate(_indicatorPrefab, _indicatorsParent);
                    var itemIndicator = currentIndicator;

                    // Set up the indicator
                    itemIndicator.SetIcon(itemData.ItemPrefab.ItemIcon);
                    itemIndicator.SetText(itemData.ItemCount.ToString());

                    // Store the indicator by item ID
                    _itemIndicators[itemData.ItemPrefab.ItemId] = itemIndicator;

                    // Store the required item count
                    _requiredItemCounts[itemData.ItemPrefab.ItemId] = itemData.ItemCount;
                }
            }
        }

        /// <summary>
        /// Starts the timer with a specified time.
        /// </summary>
        /// <param name="timeInSeconds">The time to start the timer with, in seconds.</param>
        public void StartTimer(float timeInSeconds)
        {
            _currentLevelTime = timeInSeconds;
            _isTimerRunning = true;

            OnTimerUpdated?.Invoke(_currentLevelTime, _levelConfig.CriticalTimeThreshold);

            InvokeRepeating(nameof(UpdateTimer), _levelConfig.UpdateInterval, _levelConfig.UpdateInterval);
        }

        /// <summary>
        /// Updates the timer every second.
        /// </summary>
        private void UpdateTimer()
        {
            if (!_isTimerRunning) return;

            _currentLevelTime -= _levelConfig.UpdateInterval;
            if (_currentLevelTime <= 0)
            {
                _currentLevelTime = 0;
                _isTimerRunning = false;
                CancelInvoke(nameof(UpdateTimer));
                LevelFail();
            }

            OnTimerUpdated?.Invoke(_currentLevelTime, _levelConfig.CriticalTimeThreshold);
        }

        /// <summary>
        /// Adds extra time to the current timer.
        /// </summary>
        /// <param name="extraTimeInSeconds">The additional time to add, in seconds.</param>
        public void AddExtraTime(float extraTimeInSeconds)
        {
            _currentLevelTime += extraTimeInSeconds;
            if (!_isTimerRunning)
            {
                _isTimerRunning = true;
                InvokeRepeating(nameof(UpdateTimer), _levelConfig.UpdateInterval, _levelConfig.UpdateInterval);
            }

            OnTimerUpdated?.Invoke(_currentLevelTime, _levelConfig.CriticalTimeThreshold);
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void StopTimer()
        {
            _isTimerRunning = false;
            CancelInvoke(nameof(UpdateTimer));
        }

        /// <summary>
        /// Resets the timer to the initial time.
        /// </summary>
        public void ResetTimer()
        {
            StopTimer();
            StartTimer(_levelConfig.InitialTime);
        }

        /// <summary>
        /// Marks the level as complete and shows the completion UI.
        /// </summary>
        public void LevelComplete()
        {
            OnLevelCompleted?.Invoke();
            Debug.Log("Level Completed!");
        }

        /// <summary>
        /// Marks the level as fail and shows the completion UI.
        /// </summary>
        public void LevelFail()
        {
            OnLevelFailed?.Invoke();
            Debug.Log("Level Failed!");
        }

        /// <summary>
        /// Increases the current level index and proceeds to the next level.
        /// </summary>
        public void IncreaseLevelIndex()
        {
            _currentLevelIndex++;
            Debug.Log($"Increased Level Index to {_currentLevelIndex}");
        }

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void Restart()
        {
            Time.timeScale = 1;
            // Reload the current level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Game Restarted");
        }

        /// <summary>
        /// Starts the next level.
        /// </summary>
        public void Next()
        {
            Time.timeScale = 1;
            // Load the next level
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
            Debug.Log("Next Level");
        }

        /// <summary>
        /// Navigates to the main menu.
        /// </summary>
        public void Menu()
        {
            Time.timeScale = 1;
            // Load the main menu scene
            SceneManager.LoadScene(0);
            Debug.Log("Navigated to Menu");
        }

        /// <summary>
        /// Updates the item indicator when an item is collected.
        /// </summary>
        /// <param name="item">The collected item.</param>
        public void UpdateItemCollection(Item item)
        {
            if (_itemIndicators.TryGetValue(item.ItemId, out var itemIndicator))
            {
                itemIndicator.DecreaseQuantity();

                // Reduce the count of the required item
                if (_requiredItemCounts.ContainsKey(item.ItemId))
                {
                    _requiredItemCounts[item.ItemId]--;

                    // If the count reaches zero, remove the item from required items
                    if (_requiredItemCounts[item.ItemId] <= 0)
                    {
                        _requiredItemCounts.Remove(item.ItemId);

                        // Check if all required items are collected
                        if (_requiredItemCounts.Count <= 0)
                        {
                            LevelComplete();
                        }
                    }
                }
            }
        }
    }
}