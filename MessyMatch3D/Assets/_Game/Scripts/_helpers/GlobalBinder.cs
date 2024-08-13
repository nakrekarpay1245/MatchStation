using _Game.Scripts.Management;
using UnityEngine;

namespace _Game.Scripts._helpers
{
    public class GlobalBinder : MonoSingleton<GlobalBinder>
    {
        [Header("Audio")]
        public AudioManager AudioManager;

        [Header("Item")]
        public ItemManager ItemManager;

        [Header("Level")]
        public LevelManager LevelManager;

        [Header("Particle")]
        public ParticleManager ParticleManager;

        [Header("PopUp")]
        public PopUpTextManager PopUpTextManager;

        [Header("Tile")]
        public TileManager TileManager;

        [Header("Time")]
        public TimeManager TimeManager;

        [Header("Time")]
        public UIManager UIManager;
    }
}