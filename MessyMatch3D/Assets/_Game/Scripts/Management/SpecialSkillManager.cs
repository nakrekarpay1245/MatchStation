using _Game.Scripts._helpers;
using _Game.Scripts.Items;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages special skills in the game, including destroying items, shaking items, recycling items, and freezing time.
    /// Each skill is triggered by corresponding buttons.
    /// </summary>
    public class SpecialSkillManager : MonoBehaviour
    {
        [Header("Leaf Buttons")]
        [Header("Destroy Triple Item Settings")]
        [Tooltip("Button to trigger the Destroy Triple Item skill.")]
        [SerializeField] private LeafButton _destroyTripleItemButton;

        [Header("Item Shaker Settings")]
        [Tooltip("Button to trigger the Item Shaker skill.")]
        [SerializeField] private LeafButton _itemShakerButton;

        [SerializeField, Tooltip("The minimum force applied in the upward direction (y-axis).")]
        private float _minUpwardForce = 5f;

        [SerializeField, Tooltip("The maximum force applied in the upward direction (y-axis).")]
        private float _maxUpwardForce = 10f;

        [SerializeField, Tooltip("The minimum force applied in the positive X and Z directions.")]
        private float _minHorizontalForce = 2f;

        [SerializeField, Tooltip("The maximum force applied in the positive X and Z directions.")]
        private float _maxHorizontalForce = 5f;

        [SerializeField, Tooltip("The minimum force applied in the positive X and Z directions.")]
        private float _minVerticalForce = 2f;

        [SerializeField, Tooltip("The maximum force applied in the positive X and Z directions.")]
        private float _maxVerticalForce = 5f;

        [Header("Recycle Item Settings")]
        [Tooltip("Button to trigger the Recycle Item skill.")]
        [SerializeField] private LeafButton _recycleItemButton;

        [Header("Freeze Time Settings")]
        [Tooltip("Button to trigger the Freeze Time skill.")]
        [SerializeField] private LeafButton _freezeTimeButton;
        [Tooltip("Duration for which the time is frozen.")]
        [SerializeField, Range(1f, 20f)] private float _timeFreezeDuration = 10f;

        [Header("Effect Settings")]
        [Header("Audio Settings")]
        [SerializeField, Tooltip("")]
        private string _freezeEffectParticleKey = "Freeze";
        [SerializeField, Tooltip("")]
        private string _itemShakerParticleKey = "ItemShaker";

        [Header("Particle Settings")]
        [SerializeField, Tooltip("")]
        private string _freezeEffectClipKey = "Freeze";
        [SerializeField, Tooltip("")]
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
            _destroyTripleItemButton.OnPressed.AddListener(DestroyTripleItem);
            _itemShakerButton.OnPressed.AddListener(ItemShaker);
            _recycleItemButton.OnPressed.AddListener(RecycleItem);
            _freezeTimeButton.OnPressed.AddListener(FreezeTime);
        }

        /// <summary>
        /// Destroys up to three items of the same type on the board.
        /// </summary>
        private void DestroyTripleItem()
        {
            GlobalBinder.singleton.ItemManager.DeactivateRandomRequiredItems();
            Debug.Log("DestroyTripleItem skill activated.");
        }

        /// <summary>
        /// Shakes the items on the board by moving and rotating the specified transform.
        /// </summary>
        private void ItemShaker()
        {
            Debug.Log("ItemShaker skill activated.");

            List<Item> itemList = GlobalBinder.singleton.ItemManager.ActiveItems;

            foreach (Item item in itemList)
            {
                Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
                if (itemRigidbody != null)
                {
                    Vector3 randomForce = new Vector3(
                        Random.Range(_minHorizontalForce, _maxHorizontalForce),
                        Random.Range(_minUpwardForce, _maxUpwardForce),
                        Random.Range(_minVerticalForce, _maxVerticalForce)
                    );

                    itemRigidbody.AddForce(randomForce, ForceMode.Impulse);
                }
            }
        }

        /// <summary>
        /// Recycles the last collected item, placing it back on the board.
        /// </summary>
        private void RecycleItem()
        {
            Debug.Log("RecycleItem skill activated.");
            GlobalBinder.singleton.ItemManager.RecycleLastCollectedItem();
        }

        /// <summary>
        /// Freezes the game timer for a specified duration, stopping all item movements.
        /// </summary>
        private void FreezeTime()
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