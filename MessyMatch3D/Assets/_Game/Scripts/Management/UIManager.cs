using TMPro;
using UnityEngine;
using DG.Tweening;
using _Game.Scripts._helpers;
using _Game.Scripts.Data;

namespace _Game.Scripts.Management
{
    public class UIManager : MonoBehaviour
    {
        [Header("Game Data")]
        [SerializeField, Tooltip("")]
        private GameData _gameData;

        [Header("UI Elements")]
        [Header("Displayers")]
        [SerializeField, Tooltip("")]
        private TextMeshProUGUI _levelText;

        [Header("Buttons")]
        [SerializeField, Tooltip("Pause menu UI element")]
        private CanvasGroup _pauseMenu;
        [SerializeField, Tooltip("Resume button UI element")]
        private LeafButton _resumeButton;
        [SerializeField, Tooltip("Restart button UI element")]
        private LeafButton _restartButton;
        [SerializeField, Tooltip("Menu button UI element")]
        private LeafButton _menuButton;
        [SerializeField, Tooltip("Next level button UI element")]
        private LeafButton _nextButton;
        [SerializeField, Tooltip("Pause button UI element")]
        private LeafButton _pauseButton;

        [Header("Results")]
        [SerializeField, Tooltip("Level complete text UI element")]
        private TextMeshProUGUI _levelCompleteText;
        [SerializeField, Tooltip("Level fail text UI element")]
        private TextMeshProUGUI _levelFailText;

        [Header("PauseMenu")]
        [SerializeField, Tooltip("Game pause text UI element")]
        private TextMeshProUGUI _gamePausedText;

        [Header("Timer Settings")]
        [SerializeField, Tooltip("Text component to display the remaining level time.")]
        private TextMeshProUGUI _levelTimeText;
        [SerializeField, Tooltip("Text color when the time is below the critical threshold.")]
        private Color _criticalTimeColor = Color.red;

        [Header("Freeze Screen Reference")]
        [SerializeField, Tooltip("The CanvasGroup representing the freeze screen.")]
        private CanvasGroup _freezeScreen;

        private void Start()
        {
            InitializeUI();
            RegisterEventListeners();
        }

        /// <summary>
        /// Initializes the UI elements by setting their initial states.
        /// </summary>
        private void InitializeUI()
        {
            // Initialize buttons with corresponding functions
            InitializeButtons();

            // Initialize displayers
            InitializeDisplayers();

            // Hide pause menu and end level texts at the start
            SetCanvasGroupVisibility(_pauseMenu, false);
            SetUIElementVisibility(_levelCompleteText, false);
            SetUIElementVisibility(_levelFailText, false);
            SetUIElementVisibility(_gamePausedText, false);
        }

        /// <summary>
        /// Registers event listeners for the LevelManager and TimeManager.
        /// </summary>
        private void RegisterEventListeners()
        {
            var levelManager = GlobalBinder.singleton.LevelManager;
            var timeManager = GlobalBinder.singleton.TimeManager;

            levelManager.OnLevelCompleted += HandleLevelComplete;
            levelManager.OnLevelFailed += HandleLevelFail;
            timeManager.OnTimerUpdated += UpdateTimerDisplay;
        }

        /// <summary>
        /// Initializes buttons with corresponding functions using LeafButton.
        /// </summary>
        private void InitializeButtons()
        {
            AddButtonListener(_resumeButton, Resume);
            AddButtonListener(_restartButton, Restart);
            AddButtonListener(_menuButton, NavigateToMenu);
            AddButtonListener(_nextButton, StartNextLevel);
            AddButtonListener(_pauseButton, Pause);
        }

        private void InitializeDisplayers()
        {
            _levelText.text = "LEVEL " + (_gameData.CurrentLevelIndex + 1);
        }

        /// <summary>
        /// Adds a listener to the button using LeafButton's onClick event.
        /// </summary>
        /// <param name="button">The LeafButton component to add the listener to.</param>
        /// <param name="action">The action to invoke when the button is clicked.</param>
        private void AddButtonListener(LeafButton button, System.Action action)
        {
            button.OnPressed.AddListener(() => action.Invoke());
        }

        /// <summary>
        /// Updates the timer display in the format of "MM:SS".
        /// </summary>
        private void UpdateTimerDisplay(float currentLevelTime, float criticalTimeThreshold)
        {
            int minutes = Mathf.FloorToInt(currentLevelTime / 60);
            int seconds = Mathf.FloorToInt(currentLevelTime % 60);
            _levelTimeText.text = $"{minutes:D2}:{seconds:D2}";

            if (currentLevelTime <= criticalTimeThreshold)
            {
                ApplyCriticalTimeEffects();
            }
        }

        /// <summary>
        /// Applies visual effects when the timer is below the critical threshold.
        /// </summary>
        private void ApplyCriticalTimeEffects()
        {
            _levelTimeText.color = _criticalTimeColor;
            _levelTimeText.rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1, 0)
                .SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo);
        }

        /// <summary>
        /// Handles the logic when the level is completed.
        /// </summary>
        private void HandleLevelComplete()
        {
            SetUIElementVisibility(_levelCompleteText, true);
            SetUIElementVisibility(_levelFailText, false);
            SetCanvasGroupVisibility(_pauseMenu, true);

            ConfigureButtonsForLevelEnd();
            AnimateUIOnLevelComplete();
        }

        /// <summary>
        /// Configures buttons to display after the level ends.
        /// </summary>
        private void ConfigureButtonsForLevelEnd()
        {
            _resumeButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
            _menuButton.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// Animates the UI when the level is completed.
        /// </summary>
        private void AnimateUIOnLevelComplete()
        {
            _pauseMenu.DOFade(1, 1f).OnComplete(() =>
            {
                _levelCompleteText.DOFade(1, 1f).SetDelay(2f);
            });
        }

        /// <summary>
        /// Handles the logic when the level fails.
        /// </summary>
        private void HandleLevelFail()
        {
            SetUIElementVisibility(_levelFailText, true);
            SetUIElementVisibility(_levelCompleteText, false);
            SetCanvasGroupVisibility(_pauseMenu, true);

            ConfigureButtonsForLevelFail();
            AnimateUIOnLevelFail();
        }

        /// <summary>
        /// Configures buttons to display after the level fails.
        /// </summary>
        private void ConfigureButtonsForLevelFail()
        {
            _resumeButton.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(true);
            _menuButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// Animates the UI when the level fails.
        /// </summary>
        private void AnimateUIOnLevelFail()
        {
            _pauseMenu.DOFade(1, 1f).OnComplete(() =>
            {
                _levelFailText.DOFade(1, 1f).SetDelay(2f);
            });
        }

        /// <summary>
        /// Pauses the game and shows the pause menu with a fade animation.
        /// </summary>
        public void Pause()
        {
            SetCanvasGroupVisibility(_pauseMenu, true);
            SetUIElementVisibility(_gamePausedText, true);
            ToggleGameElementsForPause(true);

            AnimatePauseMenuIn();
        }

        /// <summary>
        /// Resumes the game from pause state with a fade animation.
        /// </summary>
        public void Resume()
        {
            Time.timeScale = 1;
            ToggleGameElementsForPause(false);

            AnimatePauseMenuOut();
        }

        /// <summary>
        /// Toggles the visibility of game elements when the game is paused or resumed.
        /// </summary>
        private void ToggleGameElementsForPause(bool isPaused)
        {
            _pauseButton.gameObject.SetActive(!isPaused);
            _nextButton.gameObject.SetActive(!isPaused);
        }

        /// <summary>
        /// Animates the pause menu fading in.
        /// </summary>
        private void AnimatePauseMenuIn()
        {
            DOTween.Sequence()
                   .Append(_pauseMenu.DOFade(1, 0.5f))
                   .AppendCallback(() => Time.timeScale = 0)
                   .OnComplete(() => Debug.Log("Game Paused"));
        }

        /// <summary>
        /// Animates the pause menu fading out.
        /// </summary>
        private void AnimatePauseMenuOut()
        {
            DOTween.Sequence()
                   .Append(_pauseMenu.DOFade(0, 0.5f))
                   .OnComplete(() =>
                   {
                       SetUIElementVisibility(_gamePausedText, false);
                       SetCanvasGroupVisibility(_pauseMenu, false);
                       Debug.Log("Game Resumed");
                   });
        }

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void Restart()
        {
            GlobalBinder.singleton.LevelManager.Restart();
        }

        /// <summary>
        /// Starts the next level.
        /// </summary>
        public void StartNextLevel()
        {
            GlobalBinder.singleton.LevelManager.Next();
        }

        /// <summary>
        /// Navigates to the main menu.
        /// </summary>
        public void NavigateToMenu()
        {
            GlobalBinder.singleton.LevelManager.Menu();
        }

        /// <summary>
        /// Activates the freeze screen by fading it in, keeping it visible for a set duration, 
        /// and then fading it out.
        /// </summary>
        public void ActivateFreezeScreen(float totalDuration, float fadeInDuration, float fadeOutDuration)
        {
            _freezeScreen.gameObject.SetActive(true);
            _freezeScreen.alpha = 0;

            Sequence freezeSequence = DOTween.Sequence();
            freezeSequence.Append(_freezeScreen.DOFade(1, fadeInDuration));
            freezeSequence.AppendInterval(totalDuration);
            freezeSequence.Append(_freezeScreen.DOFade(0, fadeOutDuration));
            freezeSequence.OnComplete(() => _freezeScreen.gameObject.SetActive(false));
        }

        /// <summary>
        /// Sets the visibility of a CanvasGroup element by adjusting its alpha.
        /// </summary>
        private void SetCanvasGroupVisibility(CanvasGroup canvasGroup, bool isVisible)
        {
            canvasGroup.alpha = isVisible ? 1 : 0;
            canvasGroup.gameObject.SetActive(isVisible);
        }

        /// <summary>
        /// Sets the visibility of a UI element (like Text) by enabling or disabling the GameObject.
        /// </summary>
        private void SetUIElementVisibility(MonoBehaviour uiElement, bool isVisible)
        {
            uiElement.gameObject.SetActive(isVisible);
        }
    }
}