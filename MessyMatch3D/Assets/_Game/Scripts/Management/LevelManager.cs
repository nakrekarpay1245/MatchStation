using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using _Game.Scripts.Items;
using System.Linq;
using _Game.Scripts.Tiles;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages the level and UI elements using DOTween for smooth animations.
    /// Manages the level progress, including item collection requirements and UI indicators.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [Header("LevelManager Parameters")]
        [Header("UI Elements")]
        [SerializeField, Tooltip("Pause menu UI element")]
        private CanvasGroup _pauseMenu;

        [SerializeField, Tooltip("Resume button UI element")]
        private LeafButton _resumeButton;

        [SerializeField, Tooltip("Restart button UI element")]
        private LeafButton _restartButton;

        [SerializeField, Tooltip("Menu button UI element")]
        private LeafButton _menuButton;

        [SerializeField, Tooltip("Pause button UI element")]
        private LeafButton _nextButton;

        [SerializeField, Tooltip("Pause button UI element")]
        private LeafButton _pauseButton;

        [SerializeField, Tooltip("Level complete text UI element")]
        private TextMeshProUGUI _levelCompleteText;

        [SerializeField, Tooltip("Level fail text UI element")]
        private TextMeshProUGUI _levelFailText;

        [SerializeField, Tooltip("Game pause text UI element")]
        private TextMeshProUGUI _gamePausedText;

        [Header("UI Elements")]
        [Tooltip("Text component to display the remaining level time.")]
        [SerializeField] private TextMeshProUGUI _levelTimeText;

        [Header("Timer Settings")]
        [Tooltip("Initial time for the level in seconds.")]
        [SerializeField] private float _initialTime = 300f; // 5 minutes

        [Tooltip("Update interval for the timer in seconds.")]
        [SerializeField] private float _updateInterval = 1f;

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
        private GameObject _indicatorPrefab;

        [Header("Item Requirements")]
        [Tooltip("List of required items and their quantities.")]
        [SerializeField]
        private List<ItemRequirement> _itemRequirements;

        private Dictionary<int, ItemIndicator> _itemIndicators = new Dictionary<int, ItemIndicator>();
        private int _totalItemsRequired;
        private int _totalItemsCollected;
        private int _currentLevelIndex = 0;

        private void Start()
        {
            // Initialize buttons with corresponding functions
            InitializeButtons();

            // Hide pause menu and end level texts at the start
            _pauseMenu.alpha = 0;
            _pauseMenu.gameObject.SetActive(false);
            _levelCompleteText.gameObject.SetActive(false);
            _levelFailText.gameObject.SetActive(false);
            _gamePausedText.gameObject.SetActive(false);

            StartTimer(_initialTime);

            if (_tutorial)
                _tutorial.Open();

            CreateItemIndicators();
        }

        /// <summary>
        /// Initializes buttons with corresponding functions using LeafButton.
        /// </summary>
        private void InitializeButtons()
        {
            // Assign corresponding functions to each button's onClick event
            AddButtonListener(_resumeButton, Resume);
            AddButtonListener(_restartButton, Restart);
            AddButtonListener(_menuButton, Menu);
            AddButtonListener(_nextButton, Next);
            AddButtonListener(_pauseButton, Pause);
        }

        /// <summ
        /// ary>
        /// Adds a listener to the button using LeafButton's onClick event.
        /// </summary>
        /// <param name="button">The LeafButton component to add the listener to.</param>
        /// <param name="action">The action to invoke when the button is clicked.</param>
        private void AddButtonListener(LeafButton button, System.Action action)
        {
            button.OnPressed.AddListener(() => action.Invoke());
        }

        private void CreateItemIndicators()
        {
            _totalItemsRequired = 0;
            _totalItemsCollected = 0;

            foreach (var itemRequirement in _itemRequirements)
            {
                _totalItemsRequired += itemRequirement.Quantity;

                // Instantiate indicator prefab
                var indicatorObject = Instantiate(_indicatorPrefab, _indicatorsParent);
                var itemIndicator = indicatorObject.GetComponent<ItemIndicator>();

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
            UpdateTimerDisplay();
            InvokeRepeating(nameof(UpdateTimer), _updateInterval, _updateInterval);
        }

        /// <summary>
        /// Updates the timer every second.
        /// </summary>
        private void UpdateTimer()
        {
            if (!_isTimerRunning) return;

            _currentLevelTime -= _updateInterval;
            if (_currentLevelTime <= 0)
            {
                _currentLevelTime = 0;
                _isTimerRunning = false;
                CancelInvoke(nameof(UpdateTimer));
            }

            UpdateTimerDisplay();
        }

        /// <summary>
        /// Updates the timer display in the format of "MM:SS".
        /// </summary>
        private void UpdateTimerDisplay()
        {
            int minutes = Mathf.FloorToInt(_currentLevelTime / 60);
            int seconds = Mathf.FloorToInt(_currentLevelTime % 60);
            _levelTimeText.text = $"{minutes:D2}:{seconds:D2}";
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
                InvokeRepeating(nameof(UpdateTimer), _updateInterval, _updateInterval);
            }

            UpdateTimerDisplay();
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
            StartTimer(_initialTime);
        }

        /// <summary>
        /// Marks the level as complete and shows the completion UI.
        /// </summary>
        public void LevelComplete()
        {
            _levelCompleteText.gameObject.SetActive(true);
            _levelFailText.gameObject.SetActive(false);
            _pauseMenu.gameObject.SetActive(true);
            _gamePausedText.gameObject.SetActive(false);

            _resumeButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
            _menuButton.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(true);

            _pauseMenu.DOFade(1, 1f).OnComplete(() =>
            {
                _levelCompleteText.DOFade(1, 1f).SetDelay(2f).OnComplete(() =>
                {
                    IncreaseLevelIndex();
                });
            });
            Debug.Log("Level Complete");
        }

        /// <summary>
        /// Marks the level as fail and shows the completion UI.
        /// </summary>
        public void LevelFail()
        {
            _levelFailText.gameObject.SetActive(true);
            _levelCompleteText.gameObject.SetActive(false);
            _pauseMenu.gameObject.SetActive(true);
            _gamePausedText.gameObject.SetActive(false);

            _resumeButton.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
            _menuButton.gameObject.SetActive(true);

            _pauseMenu.DOFade(1, 1f).OnComplete(() =>
            {
                _levelFailText.DOFade(1, 1f).SetDelay(2f);
            });
            Debug.Log("Level Fail");
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
        /// Pauses the game and shows the pause menu with a fade animation.
        /// </summary>
        public void Pause()
        {
            _pauseMenu.gameObject.SetActive(true);
            _gamePausedText.gameObject.SetActive(true);

            _pauseButton.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(false);

            Sequence pauseSequence = DOTween.Sequence();
            pauseSequence.Append(_pauseMenu.DOFade(1, 0.5f))
                          .AppendCallback(() =>
                          {
                              Time.timeScale = 0;
                          })
                          .OnComplete(() => Debug.Log("Game Paused"));
        }

        /// <summary>
        /// Resumes the game from pause state with a fade animation.
        /// </summary>
        public void Resume()
        {
            Time.timeScale = 1;
            _pauseButton.gameObject.SetActive(true);
            Sequence resumeSequence = DOTween.Sequence();
            resumeSequence.Append(_pauseMenu.DOFade(0, 0.5f))
                          .OnComplete(() =>
                          {
                              _gamePausedText.gameObject.SetActive(false);
                              _pauseMenu.gameObject.SetActive(false);
                              Debug.Log("Game Resumed");
                          });
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
                itemIndicator.DecreaseQuantity();
                _totalItemsCollected++;

                if (_totalItemsCollected >= _totalItemsRequired)
                {
                    LevelComplete();
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