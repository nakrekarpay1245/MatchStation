using UnityEngine;

namespace _Game.Scripts.Items
{
    /// <summary>
    /// Represents an item in the game. This class encapsulates the item's ID, icon, and description.
    /// </summary>
    public class Item : MonoBehaviour
    {
        // Private fields
        [Header("Item Properties")]
        [Tooltip("Unique identifier for the item.")]
        [SerializeField]
        private int _itemId;

        // Encapsulated properties
        /// <summary>
        /// Gets the unique identifier for the item.
        /// </summary>
        public int ItemId
        {
            get => _itemId;
            private set => _itemId = value; // Can be set only internally.
        }

        [Tooltip("Icon representing the item.")]
        [SerializeField]
        private Sprite _itemIcon;

        /// <summary>
        /// Gets the icon representing the item.
        /// </summary>
        public Sprite ItemIcon
        {
            get => _itemIcon;
            set => _itemIcon = value;
        }

        [Tooltip("Description of the item.")]
        [TextArea]
        [SerializeField]
        private string _itemDescription;

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        public string ItemDescription
        {
            get => _itemDescription;
            set => _itemDescription = value;
        }
    }
}