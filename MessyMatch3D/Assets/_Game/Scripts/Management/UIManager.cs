using TMPro;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using _Game.Scripts._helpers;

namespace _Game.Scripts.Management
{
    public class UIManager : MonoBehaviour
    {
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

        [Header("Timer Settings")]
        [Header("UI")]
        [Tooltip("Text component to display the remaining level time.")]
        [SerializeField] private TextMeshProUGUI _levelTimeText;

        [Tooltip("Text color when the time is below the critical threshold.")]
        [SerializeField] private Color _criticalTimeColor = Color.red;

        [Header("Freeze Screen Reference")]
        [Tooltip("The CanvasGroup representing the freeze screen.")]
        [SerializeField]
        private CanvasGroup _freezeScreen;
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

            //LevelManager Events
            GlobalBinder.singleton.LevelManager.OnLevelCompleted += LevelComplete;
            GlobalBinder.singleton.LevelManager.OnLevelFailed += LevelFail;
            GlobalBinder.singleton.TimeManager.OnTimerUpdated += UpdateTimerDisplay;
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
                _levelTimeText.color = _criticalTimeColor;
                _levelTimeText.rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1, 0).
                    SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo);
            }
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
                _levelCompleteText.DOFade(1, 1f).SetDelay(2f);
            });
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
            GlobalBinder.singleton.LevelManager.Restart();
        }

        /// <summary>
        /// Starts the next level.
        /// </summary>
        public void Next()
        {
            GlobalBinder.singleton.LevelManager.Next();
        }

        /// <summary>
        /// Navigates to the main menu.
        /// </summary>
        public void Menu()
        {
            GlobalBinder.singleton.LevelManager.Menu();
        }

        /// <summary>
        /// Activates the freeze screen by fading it in, keeping it visible for a set duration, 
        /// and then fading it out.
        /// </summary>
        public void ActivateFreezeScreen(float totalDuration, float fadeInDuration, float fadeOutDuration)
        {
            // Ensure the CanvasGroup is active and starts invisible
            _freezeScreen.gameObject.SetActive(true);
            _freezeScreen.alpha = 0;

            // Sequence to manage the fade in, wait, and fade out
            Sequence freezeSequence = DOTween.Sequence();

            // Fade in
            freezeSequence.Append(_freezeScreen.DOFade(1, fadeInDuration));

            // Wait for the duration
            freezeSequence.AppendInterval(totalDuration);

            // Fade out
            freezeSequence.Append(_freezeScreen.DOFade(0, fadeOutDuration));

            // Deactivate after fade out
            freezeSequence.OnComplete(() => _freezeScreen.gameObject.SetActive(false));

            // Start the sequence
            freezeSequence.Play();
        }
    }
}