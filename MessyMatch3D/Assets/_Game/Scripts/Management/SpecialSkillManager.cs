using _Game.Scripts._helpers;
using _Game.Scripts.Items;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages special skills in the game, including destroying items, shaking items, recycling items,
    /// and freezing time.
    /// Each skill is triggered by corresponding buttons.
    /// </summary>
    public class SpecialSkillManager : MonoBehaviour
    {
        [Header("Skill Buttons")]
        [SerializeField, Tooltip("Button to trigger the Destroy Triple Item skill.")]
        private LeafButton _destroyTripleItemButton;

        [SerializeField, Tooltip("Button to trigger the Item Shaker skill.")]
        private LeafButton _itemShakerButton;

        [SerializeField, Tooltip("Button to trigger the Recycle Item skill.")]
        private LeafButton _recycleItemButton;

        [SerializeField, Tooltip("Button to trigger the Freeze Time skill.")]
        private LeafButton _freezeTimeButton;

        [Header("Item Shaker Settings")]
        [SerializeField, Tooltip("The minimum force applied in the upward direction (y-axis).")]
        private float _minUpwardForce = 5f;
        [SerializeField, Tooltip("The maximum force applied in the upward direction (y-axis).")]
        private float _maxUpwardForce = 10f;
        [SerializeField, Tooltip("The minimum force applied in the positive X and Z directions.")]
        private float _minHorizontalForce = 2f;
        [SerializeField, Tooltip("The maximum force applied in the positive X and Z directions.")]
        private float _maxHorizontalForce = 5f;
        [SerializeField, Tooltip("The minimum force applied in the positive Y direction.")]
        private float _minVerticalForce = 2f;
        [SerializeField, Tooltip("The maximum force applied in the positive Y direction.")]
        private float _maxVerticalForce = 5f;

        [Header("Freeze Time Settings")]
        [SerializeField, Range(1f, 20f), Tooltip("Duration for which the time is frozen.")]
        private float _timeFreezeDuration = 10f;

        [Header("Particle and Audio Settings")]
        [SerializeField, Tooltip("Key for the freeze effect particle.")]
        private string _freezeEffectParticleKey = "Freeze";
        [SerializeField, Tooltip("Key for the item shaker particle.")]
        private string _itemShakerParticleKey = "ItemShaker";
        [SerializeField, Tooltip("Audio clip key for freeze effect.")]
        private string _freezeEffectClipKey = "Freeze";
        [SerializeField, Tooltip("Audio clip key for item shaker effect.")]
        private string _itemShakerClipKey = "ItemShaker";

        private void Awake()
        {
            InitializeButtons();
        }

        /// <summary>
        /// Initializes the buttons by assigning the appropriate skill functions to their OnPressed events.
        /// </summary>
        private void InitializeButtons()
        {
            _destroyTripleItemButton.OnPressed.AddListener(OnDestroyTripleItem);
            _itemShakerButton.OnPressed.AddListener(OnItemShaker);
            _recycleItemButton.OnPressed.AddListener(OnRecycleItem);
            _freezeTimeButton.OnPressed.AddListener(OnFreezeTime);
        }

        /// <summary>
        /// Destroys up to three items of the same type on the board.
        /// </summary>
        private void OnDestroyTripleItem()
        {
            GlobalBinder.singleton.ItemManager.DeactivateRandomRequiredItems();
            Debug.Log("DestroyTripleItem skill activated.");
        }

        /// <summary>
        /// Shakes the items on the board by applying random forces to their rigidbodies.
        /// </summary>
        private void OnItemShaker()
        {
            Debug.Log("ItemShaker skill activated.");
            List<Item> items = GlobalBinder.singleton.ItemManager.ActiveItems;

            foreach (Item item in items)
            {
                Rigidbody rb = item.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 force = new Vector3(
                        Random.Range(_minHorizontalForce, _maxHorizontalForce),
                        Random.Range(_minUpwardForce, _maxUpwardForce),
                        Random.Range(_minVerticalForce, _maxVerticalForce)
                    );
                    rb.AddForce(force, ForceMode.Impulse);
                }
            }
        }

        /// <summary>
        /// Recycles the last collected item, placing it back on the board.
        /// </summary>
        private void OnRecycleItem()
        {
            Debug.Log("RecycleItem skill activated.");
            GlobalBinder.singleton.ItemManager.RecycleLastCollectedItem();
        }

        /// <summary>
        /// Freezes the game timer for a specified duration, stopping all item movements and playing
        /// associated effects.
        /// </summary>
        private void OnFreezeTime()
        {
            Debug.Log("FreezeTime skill activated.");
            GlobalBinder.singleton.TimeManager.FreezeTimer(_timeFreezeDuration);
            GlobalBinder.singleton.UIManager.ActivateFreezeScreen(_timeFreezeDuration, 1f, 1f);

            GlobalBinder.singleton.ParticleManager.PlayParticleAtPoint(_freezeEffectParticleKey, 
                Vector2.up * 2);
            GlobalBinder.singleton.AudioManager.PlaySound(_freezeEffectClipKey);
        }
    }
}