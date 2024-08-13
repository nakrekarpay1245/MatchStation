using _Game.Scripts._helpers;
using _Game.Scripts.Tiles;
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

        [Header("Tile Parameters")]
        [Tooltip("")]
        [SerializeField]
        private Tile _itemTile;
        public Tile ItemTile
        {
            get
            {
                return _itemTile;
            }
            set
            {
                _itemTile = value;
                Vector3 itemPosition = _itemTile ? _itemTile.transform.position : Vector3.zero;

                if (IsCollectable)
                {
                    GlobalBinder.singleton.ParticleManager.PlayParticleAtPoint(_itemCollectParticleKey,
                            transform.position);

                    GlobalBinder.singleton.AudioManager.PlaySound(_itemCollectClipKey);
                }

                transform.DOMove(itemPosition + _itemPositionOffset, _itemMoveDuration);

                transform.DORotate(Vector3.zero, _itemMoveDuration);
            }
        }

        [Header("Item Move Settings")]
        [Tooltip("Duration for items to move to their new positions.")]
        [SerializeField]
        private float _itemMoveDuration = 0.5f;

        [Tooltip("")]
        [SerializeField]
        private Vector3 _itemPositionOffset = Vector3.up;

        private bool _isCollectable = true;
        public bool IsCollectable { get => _isCollectable; private set => _isCollectable = value; }

        [Header("Effects")]
        [Header("Particle Effects")]
        [SerializeField, Tooltip("")]
        private string _itemCollectParticleKey = "ItemCollect";
        [Header("Audio Effects")]
        [SerializeField, Tooltip("")]
        private string _itemCollectClipKey = "ItemCollect";

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

            GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 1f);
        }

        public void DeSelect()
        {
            //Debug.Log(name + "DeSelected!");
            // Apply a scale effect using DOTween
            transform.DOScale(Vector3.one, 0.2f); // Scale back to original

            GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0f);
        }

        public void Collect()
        {
            _isCollectable = false;

            //Debug.Log(name + "Collected!");
            // Apply a scale effect using DOTween
            // Scale back to original
            transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
            {
                _rigidbody.isKinematic = true;
            });

            GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0f);
        }

        public void Recycle()
        {
            _isCollectable = true;

            //Debug.Log(name + " Recycled!");
            // Apply a scale effect using DOTween
            // Scale back to original
            transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
            {
                _rigidbody.isKinematic = false;
            });
        }
    }
}