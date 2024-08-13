using _Game.Scripts._helpers;
using DG.Tweening;
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
        [Tooltip("Transform to shake during the Item Shaker skill.")]
        [SerializeField] private Transform _itemShaker;
        [Tooltip("Duration of the vertical movement during the shake.")]
        [SerializeField, Range(0.1f, 1f)] private float _shakerMovementDuration = 0.5f;
        [Tooltip("Duration of the rotation during the shake.")]
        [SerializeField, Range(0.1f, 1f)] private float _shakerRotationDuration = 0.5f;
        [Tooltip("Height change amount during the shake.")]
        [SerializeField, Range(0.1f, 5f)] private float _shakerHeightChangeAmount = 2f;

        [Header("Recycle Item Settings")]
        [Tooltip("Button to trigger the Recycle Item skill.")]
        [SerializeField] private LeafButton _recycleItemButton;

        [Header("Freeze Time Settings")]
        [Tooltip("Button to trigger the Freeze Time skill.")]
        [SerializeField] private LeafButton _freezeTimeButton;
        [Tooltip("Duration for which the time is frozen.")]
        [SerializeField, Range(1f, 20f)] private float _timeFreezeDuration = 10f;

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

            // Create a sequence for the shaking animation
            Sequence shakerSequence = DOTween.Sequence();

            shakerSequence.Append(_itemShaker.DOMoveY(_itemShaker.position.y + _shakerHeightChangeAmount,
                _shakerMovementDuration).SetEase(Ease.Linear));
            shakerSequence.Join(_itemShaker.DORotate(new Vector3(0, 360, 0),
                _shakerRotationDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
            shakerSequence.Append(_itemShaker.DOMoveY(_itemShaker.position.y,
                _shakerMovementDuration).SetEase(Ease.Linear));
            shakerSequence.Join(_itemShaker.DORotate(new Vector3(0, 360, 0),
                _shakerRotationDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
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
        }
    }
}