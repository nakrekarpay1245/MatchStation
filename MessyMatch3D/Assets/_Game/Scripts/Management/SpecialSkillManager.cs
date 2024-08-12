using _Game.Scripts._helpers;
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
            // Implement the logic to shake and shuffle items on the board
            Debug.Log("ItemShaker skill activated.");
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