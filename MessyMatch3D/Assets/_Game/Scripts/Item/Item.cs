using _Game.Scripts._helpers;
using _Game.Scripts.Tiles;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Items
{
    /// <summary>
    /// Represents an item in the game. This class encapsulates the item's properties and behaviors.
    /// </summary>
    public class Item : MonoBehaviour, ISelectable, ICollectable
    {
        [Header("Item Properties")]
        [Tooltip("Unique identifier for the item.")]
        [SerializeField, HideInInspector]
        private int _itemId;

        [Tooltip("Icon representing the item.")]
        [SerializeField]
        private Sprite _itemIcon;

        [Tooltip("Description of the item.")]
        [TextArea, SerializeField]
        private string _itemDescription;

        [Header("Tile Parameters")]
        [Tooltip("Tile associated with the item.")]
        [SerializeField]
        private Tile _itemTile;

        [Header("Item Move Settings")]
        [Tooltip("Duration for items to move to their new positions.")]
        [SerializeField]
        private float _itemMoveDuration = 0.5f;

        [Tooltip("Position offset when the item moves.")]
        [SerializeField]
        private Vector3 _itemPositionOffset = Vector3.up;

        [Header("Scale Parameters")]
        [Tooltip("Rotation of the item when it is collected.")]
        [SerializeField]
        private Vector3 _itemCollectRotation = new Vector3(0f, 90f, 90f);

        [Tooltip("Scale multiplier for the item when it is in its normal state.")]
        [SerializeField]
        private float _itemNormalScaleMultiplier = 1f;

        [Tooltip("Scale multiplier for the item when it is selected.")]
        [SerializeField]
        private float _itemSelectedMultiplier = 1.25f;

        [Tooltip("Scale multiplier for the item when it is collected.")]
        [SerializeField]
        private float _itemCollectedScaleMultiplier = 0.5f;

        [Tooltip("Duration of the scale change animation.")]
        [SerializeField]
        private float _itemScaleChangeDuration = 0.2f;

        [Header("Effects")]
        [Tooltip("Particle effect key for when the item is collected.")]
        [SerializeField]
        private string _itemCollectParticleKey = "ItemCollect";

        [Tooltip("Audio clip key for when the item is collected.")]
        [SerializeField]
        private string _itemCollectClipKey = "ItemCollect";

        private Rigidbody _rigidbody;
        private bool _isCollectable = true;

        /// <summary>
        /// Gets the unique identifier for the item.
        /// </summary>
        public int ItemId => _itemId;

        /// <summary>
        /// Gets or sets the icon representing the item.
        /// </summary>
        public Sprite ItemIcon
        {
            get => _itemIcon;
            set => _itemIcon = value;
        }

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        public string ItemDescription
        {
            get => _itemDescription;
            set => _itemDescription = value;
        }

        /// <summary>
        /// Gets or sets the tile associated with the item.
        /// </summary>
        public Tile ItemTile
        {
            get => _itemTile;
            set
            {
                _itemTile = value;
                UpdateItemPosition();
                PlayCollectionEffects();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item is collectable.
        /// </summary>
        public bool IsCollectable => _isCollectable;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            ResetItemScale();
        }

        /// <summary>
        /// Handles the selection of the item and applies a scale animation.
        /// </summary>
        public void Select()
        {
            ApplyScaleAnimation(_itemSelectedMultiplier);
        }

        /// <summary>
        /// Handles the deselection of the item and reverts the scale.
        /// </summary>
        public void DeSelect()
        {
            ApplyScaleAnimation(_itemNormalScaleMultiplier);
        }

        /// <summary>
        /// Handles the collection of the item.
        /// </summary>
        public void Collect()
        {
            _isCollectable = false;
            ApplyScaleAnimation(_itemCollectedScaleMultiplier, () =>
            {
                _rigidbody.isKinematic = true;
            });
        }

        /// <summary>
        /// Recycles the item, resetting its collectable state and scale.
        /// </summary>
        public void Recycle()
        {
            _isCollectable = true;
            ApplyScaleAnimation(_itemNormalScaleMultiplier, () =>
            {
                _rigidbody.isKinematic = false;
            });
        }

        /// <summary>
        /// Resets the scale of the item to its normal state.
        /// </summary>
        private void ResetItemScale()
        {
            transform.localScale = Vector3.one * _itemNormalScaleMultiplier;
        }

        /// <summary>
        /// Updates the item's position and rotation based on the associated tile.
        /// </summary>
        private void UpdateItemPosition()
        {
            if (_itemTile == null) return;

            Vector3 itemPosition = _itemTile.transform.position + _itemPositionOffset;
            Vector3 itemRotation = _itemCollectRotation;

            transform.DOMove(itemPosition, _itemMoveDuration);
            transform.DORotate(itemRotation, _itemMoveDuration);
        }

        /// <summary>
        /// Plays particle and sound effects when the item is collected.
        /// </summary>
        private void PlayCollectionEffects()
        {
            if (IsCollectable)
            {
                GlobalBinder.singleton.ParticleManager.PlayParticleAtPoint(_itemCollectParticleKey, transform.position);
                GlobalBinder.singleton.AudioManager.PlaySound(_itemCollectClipKey);
            }
        }

        /// <summary>
        /// Applies a scale animation to the item.
        /// </summary>
        /// <param name="scaleMultiplier">The target scale multiplier.</param>
        /// <param name="onComplete">Optional callback to be invoked when the animation is complete.</param>
        private void ApplyScaleAnimation(float scaleMultiplier, TweenCallback onComplete = null)
        {
            transform.DOScale(Vector3.one * scaleMultiplier, _itemScaleChangeDuration).OnComplete(onComplete);
        }
    }
}