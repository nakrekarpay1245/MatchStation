using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Items
{
    /// <summary>
    /// Represents an item in the game. This class encapsulates the item's ID, icon, and description.
    /// </summary>
    public class Item : MonoBehaviour, ISelectable, ICollectable
    {
        [Header("Item Properties")]
        [Tooltip("Unique identifier for the item.")]
        [SerializeField]
        private int _itemId;

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

        private bool _collectable = true;
        public bool Collectable { get => _collectable; private set => _collectable = value; }

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Handles the selection of the item and applies a pop-up scale animation.
        /// </summary>
        public void Select()
        {
            //Debug.Log(name + "Selected!");
            // Apply a scale effect using DOTween
            transform.DOScale(Vector3.one * 1.2f, 0.2f); // Scale up 
        }

        public void DeSelect()
        {
            //Debug.Log(name + "DeSelected!");
            // Apply a scale effect using DOTween
            transform.DOScale(Vector3.one, 0.2f); // Scale back to original
        }

        public void Collect()
        {
            _collectable = false;

            //Debug.Log(name + "Collected!");
            // Apply a scale effect using DOTween
            // Scale back to original
            transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
            {
                _rigidbody.isKinematic = true;
            });
        }
    }
}