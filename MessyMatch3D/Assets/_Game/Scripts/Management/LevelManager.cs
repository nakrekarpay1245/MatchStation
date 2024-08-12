using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using _Game.Scripts.Items;
using UnityEngine.Events;
using _Game.Scripts._Data;

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

        [Header("Tutroial Params")]
        [SerializeField]
        private MenuView _tutorial;

        [Header("Indicator Settings")]
        [Tooltip("Parent object for the item indicators.")]
        [SerializeField]
        private Transform _indicatorsParent;

        [Tooltip("Prefab for the item indicator.")]
        [SerializeField]
        private ItemIndicator _indicatorPrefab;

        [Header("Item Requirements")]
        [Tooltip("List of required items and their quantities.")]
        [SerializeField]
        private List<ItemRequirement> _itemRequirements;

        [Tooltip("List of all required items")]
        [SerializeField]
        private List<Item> _requireItems;

        private Dictionary<int, ItemIndicator> _itemIndicators = new Dictionary<int, ItemIndicator>();

        private int _currentLevelIndex = 0;

        public UnityAction<float, float> OnTimerUpdated;
        public UnityAction OnLevelFailed;
        public UnityAction OnLevelCompleted;

        private void Start()
        {
            StartTimer(_levelConfig.InitialTime);

            if (_tutorial)
                _tutorial.Open();

            SetRequireItems();

            CreateItemIndicators();
        }

        /// <summary>
        /// Populates the _requireItems list based on _itemRequirements.
        /// Items are added to _requireItems according to their quantity in _itemRequirements.
        /// </summary>
        private void SetRequireItems()
        {
            // Clear the list to avoid duplication or stale data
            _requireItems.Clear();

            // Iterate through the item requirements
            foreach (var requirement in _itemRequirements)
            {
                // Add the item to the list as many times as specified by the quantity
                for (int i = 0; i < requirement.Quantity; i++)
                {
                    _requireItems.Add(requirement.Item);
                }
            }

            // Optionally, you can use DOTween for animations or other effects here
            // For example, if you want to animate the addition of items
            // DOTween.Sequence()...
        }

        private void CreateItemIndicators()
        {
            foreach (var itemRequirement in _itemRequirements)
            {
                // Instantiate indicator prefab
                var currentIndicator = Instantiate(_indicatorPrefab, _indicatorsParent);
                var itemIndicator = currentIndicator;

                // Set up the indicator
                itemIndicator.SetIcon(itemRequirement.Item.ItemIcon);
                itemIndicator.SetText(itemRequirement.Quantity.ToString());

                _itemIndicators[itemRequirement.Item.ItemId] = itemIndicator;
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
                InvokeRepeating(nameof(UpdateTimer), _levelConfig.UpdateInterval,
                    _levelConfig.UpdateInterval);
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
            Debug.Log("Level CompleteD!");
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
            // Add logic to reload the current level here
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Game Restarted");
        }

        /// <summary>
        /// Starts the next level.
        /// </summary>
        public void Next()
        {
            Time.timeScale = 1;
            // Add logic to reload the current level here
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) %
                SceneManager.sceneCountInBuildSettings);
            Debug.Log("Game Restarted");
        }

        /// <summary>
        /// Navigates to the main menu.
        /// </summary>
        public void Menu()
        {
            Time.timeScale = 1;
            // Add logic to load the main menu scene here
            SceneManager.LoadScene(0);
            Debug.Log("Navigated to Menu");
        }

        public void CloseTutorial()
        {
            if (_tutorial)
                _tutorial.Close();
        }

        /// <summary>
        /// Updates the item indicator when an item is collected.
        /// </summary>
        /// <param name="item">The collected item.</param>
        public void UpdateItemCollection(Item item)
        {
            if (_itemIndicators.TryGetValue(item.ItemId, out var itemIndicator))
            {
                //Debug.Log(item.name + " is required!");
                itemIndicator.DecreaseQuantity();

                foreach (Item requireItem in _requireItems)
                {
                    if (item.ItemId == requireItem.ItemId)
                    {
                        _requireItems.Remove(requireItem);

                        if (_requireItems.Count <= 0)
                        {
                            LevelComplete();
                        }
                        break;
                    }
                }
            }
        }
    }
}

/// <summary>
/// Represents the item requirement for the level.
/// </summary>
[System.Serializable]
public class ItemRequirement
{
    [Tooltip("The item to be collected.")]
    public Item Item;

    [Tooltip("The quantity of this item required.")]
    public int Quantity;
}