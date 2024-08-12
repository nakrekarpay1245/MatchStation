using _Game.Scripts._helpers;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Management
{
    public class SpecialSkillManager : MonoBehaviour
    {
        [Header("Leaf Buttons")]
        [Tooltip("Button to trigger the Destroy Triple Item skill.")]
        [SerializeField]
        private LeafButton _destroyTripleItemButton;

        [Tooltip("Button to trigger the Item Shaker skill.")]
        [SerializeField]
        private LeafButton _itemShakerButton;
        [SerializeField]
        private Transform _itemShaker;

        [SerializeField]
        private float _shakerMovementDuration = 0.5f;
        [SerializeField]
        private float _shakerRotationDuration = 0.5f;
        [SerializeField]
        private float _shakerHeightChangeAmount = 2f;

        [Tooltip("Button to trigger the Recycle Item skill.")]
        [SerializeField]
        private LeafButton _recycleItemButton;

        [Tooltip("Button to trigger the Freeze Time skill.")]
        [SerializeField]
        private LeafButton _freezeTimeButton;

        [Tooltip("")]
        [SerializeField]
        private float _timeFreezeDuration = 10f;

        private void Awake()
        {
            // Initialize buttons with corresponding functions
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            // Assigning the corresponding skill functions to the button's OnPressed event
            _destroyTripleItemButton.OnPressed.AddListener(DestroyTripleItem);
            _itemShakerButton.OnPressed.AddListener(ItemShaker);
            _recycleItemButton.OnPressed.AddListener(RecycleItem);
            _freezeTimeButton.OnPressed.AddListener(FreezeTime);
        }

        /// <summary>
        /// Destroys three items of the same type on the board.
        /// </summary>
        private void DestroyTripleItem()
        {
            // Implement the logic to destroy three items of the same type
            GlobalBinder.singleton.ItemManager.DeactivateRandomRequiredItems();
            Debug.Log("DestroyTripleItem skill activated.");
        }

        /// <summary>
        /// Shakes the items on the board to shuffle their positions.
        /// </summary>
        private void ItemShaker()
        {
            // Log the activation of the skill
            Debug.Log("ItemShaker skill activated.");

            // Set up the rotation parameters
            Vector3 rotation = new Vector3(0, 360, 0); // Rotate around Y-axis

            // Create a sequence for the shaking animation
            Sequence shakerSequence = DOTween.Sequence();

            // Add a vertical movement up
            shakerSequence.Append(_itemShaker.transform.DOMoveY(_itemShaker.transform.position.y +
                _shakerHeightChangeAmount, _shakerMovementDuration).SetEase(Ease.Linear));

            // Add a rotation
            shakerSequence.Join(_itemShaker.transform.DORotate(rotation, _shakerRotationDuration,
                RotateMode.LocalAxisAdd).SetEase(Ease.Linear));

            // Add a rotation
            shakerSequence.Append(_itemShaker.transform.DORotate(rotation, _shakerRotationDuration,
                RotateMode.LocalAxisAdd).SetEase(Ease.Linear));

            // Add a vertical movement down
            shakerSequence.Append(_itemShaker.transform.DOMoveY(_itemShaker.transform.position.y,
                _shakerMovementDuration).SetEase(Ease.Linear));

            // Add a rotation
            shakerSequence.Join(_itemShaker.transform.DORotate(rotation, _shakerRotationDuration,
                RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        }

        /// <summary>
        /// Recycles an item by replacing it with a new one.
        /// </summary>
        private void RecycleItem()
        {
            // Implement the logic to recycle an item
            Debug.Log("RecycleItem skill activated.");
            GlobalBinder.singleton.ItemManager.RecycleLastCollectedItem();
        }

        /// <summary>
        /// Freezes time for a certain duration, stopping all item movements.
        /// </summary>
        private void FreezeTime()
        {
            // Implement the logic to freeze time
            Debug.Log("FreezeTime skill activated.");
            GlobalBinder.singleton.TimeManager.FreezeTimer(_timeFreezeDuration);
        }
    }
}