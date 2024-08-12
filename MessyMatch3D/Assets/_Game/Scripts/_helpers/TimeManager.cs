using _Game.Scripts.Data;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace _Game.Scripts._helpers
{
    public class TimeManager : MonoSingleton<TimeManager>
    {
        [Header("TimeManager Parameters")]
        [Header("Level Configuration")]
        [Tooltip("Reference to the level configuration.")]
        [SerializeField] private LevelConfig _levelConfig;

        private float _currentLevelTime;
        private bool _isTimerRunning;

        public UnityAction<float, float> OnTimerUpdated;
        public UnityAction OnTimeFinished;

        private void Start()
        {
            StartTimer(_levelConfig.InitialTime);
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
                OnTimeFinished?.Invoke();
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
        /// Sets the time scale, controlling the flow of time in the game.
        /// </summary>
        /// <param name="scale">The scale at which time passes. 1 is normal speed.</param>
        public void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
        }

        /// <summary>
        /// Freezes the timer for a specified duration. After the duration, the timer resumes.
        /// </summary>
        /// <param name="duration">The duration for which to freeze the timer, in seconds.</param>
        public void FreezeTimer(float duration)
        {
            if (_isTimerRunning)
            {
                StopTimer(); // Pause the timer

                // Use DOTween to resume the timer after the specified duration
                DOVirtual.DelayedCall(duration, () =>
                {
                    if (_currentLevelTime > 0)
                    {
                        _isTimerRunning = true;
                        InvokeRepeating(nameof(UpdateTimer), _levelConfig.UpdateInterval, _levelConfig.UpdateInterval);
                    }
                });
            }
        }
    }
}