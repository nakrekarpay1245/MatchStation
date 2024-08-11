using UnityEngine;

namespace _Game.Scripts._helpers
{
    public class GlobalBinder : MonoSingleton<GlobalBinder>
    {
        [Header("Audio")]
        public AudioManager AudioManager;

        [Header("Coin")]
        public CoinManager CoinManager;

        [Header("Particle")]
        public ParticleManager ParticleManager;

        [Header("PopUp")]
        public PopUpTextManager PopUpTextManager;

        [Header("Time")]
        public TimeManager TimeManager;
    }
}