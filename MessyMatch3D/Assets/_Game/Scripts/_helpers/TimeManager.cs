using _Game.Scripts.Data;
using UnityEngine;
using UnityEngine.Events;

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

        public void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
        }
    }
}