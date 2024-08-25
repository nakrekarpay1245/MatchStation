using _Game.Scripts._helpers;
using _Game.Scripts.Management;
using UnityEngine;

namespace _Game.Scripts._helpers
{
    /// <summary>
    /// Acts as a central hub for accessing various managers in the game.
    /// Inherits from MonoSingleton to ensure a single instance across the game.
    /// </summary>
    public class GlobalBinder : MonoSingleton<GlobalBinder>
    {
        [Header("Managers")]
        [Tooltip("Handles audio functionalities like playing sounds and managing music.")]
        [SerializeField] private AudioManager _audioManager;

        [Tooltip("Manages item-related operations such as item spawning and collection.")]
        [SerializeField] private ItemManager _itemManager;

        [Tooltip("Handles level-specific operations including level initialization and completion checks.")]
        [SerializeField] private LevelManager _levelManager;

        [Tooltip("Manages particle effects used throughout the game.")]
        [SerializeField] private ParticleManager _particleManager;

        [Tooltip("Manages tile-related operations like tile state and interactions.")]
        [SerializeField] private TileManager _tileManager;

        [Tooltip("Handles time management including countdowns, timers, and related functions.")]
        [SerializeField] private TimeManager _timeManager;

        [Tooltip("Handles UI operations such as menu transitions, HUD updates, and UI effects.")]
        [SerializeField] private UIManager _uiManager;

        /// <summary>
        /// Provides public access to the AudioManager instance.
        /// </summary>
        public AudioManager AudioManager => _audioManager;

        /// <summary>
        /// Provides public access to the ItemManager instance.
        /// </summary>
        public ItemManager ItemManager => _itemManager;

        /// <summary>
        /// Provides public access to the LevelManager instance.
        /// </summary>
        public LevelManager LevelManager => _levelManager;

        /// <summary>
        /// Provides public access to the ParticleManager instance.
        /// </summary>
        public ParticleManager ParticleManager => _particleManager;

        /// <summary>
        /// Provides public access to the TileManager instance.
        /// </summary>
        public TileManager TileManager => _tileManager;

        /// <summary>
        /// Provides public access to the TimeManager instance.
        /// </summary>
        public TimeManager TimeManager => _timeManager;

        /// <summary>
        /// Provides public access to the UIManager instance.
        /// </summary>
        public UIManager UIManager => _uiManager;
    }
}