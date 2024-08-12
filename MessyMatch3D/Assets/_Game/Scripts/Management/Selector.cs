using _Game.Scripts._Data;
using _Game.Scripts._helpers;
using _Game.Scripts.Items;
using _Game.Scripts.Tiles;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Handles the selection of items and their interaction.
    /// Utilizes PlayerInput to manage user interactions with items.
    /// </summary>
    public class Selector : MonoBehaviour, ISelector, ICollector
    {
        [Header("Camera Parameters")]
        [SerializeField]
        private Camera _selectionCamera;

        [Header("Raycast Parameters")]
        [Tooltip("The layer mask used for raycasting.")]
        [SerializeField]
        private LayerMask _raycastLayerMask = ~0; // Default to all layers

        [Tooltip("The length of the raycast.")]
        [SerializeField]
        private float _raycastLength = 100f;

        [Tooltip("The color of the raycast for mouse down.")]
        [SerializeField]
        private Color _mouseDownGizmoColor = Color.green;

        [Tooltip("The color of the raycast for mouse held.")]
        [SerializeField]
        private Color _mouseHoldGizmoColor = Color.yellow;

        [Tooltip("The color of the raycast for mouse up.")]
        [SerializeField]
        private Color _mouseUpGizmoColor = Color.red;

        [Header("Item Move Settings")]
        [Tooltip("Duration for items to move to their new positions.")]
        [SerializeField]
        private float _itemMoveDuration = 0.5f;

        [Header("Dependencies")]
        [Tooltip("The PlayerInput scriptable object that stores input data.")]
        [SerializeField]
        private PlayerInput _playerInput;

        private Item _lastHoveredItem;
        private Item _currentlySelectedItem;

        private void OnEnable()
        {
            _playerInput.OnMouseDown += HandleMouseButtonDown;
            _playerInput.OnMouseHeld += HandleMouseButton;
            _playerInput.OnMouseUp += HandleMouseButtonUp;
        }

        private void OnDisable()
        {
            _playerInput.OnMouseDown -= HandleMouseButtonDown;
            _playerInput.OnMouseHeld -= HandleMouseButton;
            _playerInput.OnMouseUp -= HandleMouseButtonUp;
        }

        private void HandleMouseButtonDown()
        {
            CheckForItemAtMouse();
        }

        private void HandleMouseButton()
        {
            CheckForItemAtMouse();
        }

        private void HandleMouseButtonUp()
        {
            CheckForItemAtMouse();

            if (_currentlySelectedItem != null)
            {
                Collect(_currentlySelectedItem);
            }
        }

        private void CheckForItemAtMouse()
        {
            if (_selectionCamera == null) return;

            Ray ray = _selectionCamera.ScreenPointToRay(_playerInput.MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastLength, _raycastLayerMask))
            {
                var selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    _lastHoveredItem = selectable as Item;
                    Select(_lastHoveredItem);
                }
                else
                {
                    DeSelectCurrentItem();
                }
            }
            else
            {
                DeSelectCurrentItem();
            }
        }

        private void DeSelectCurrentItem()
        {
            if (_currentlySelectedItem != null)
            {
                DeSelect(_currentlySelectedItem);
                _currentlySelectedItem = null;
            }
        }

        public void Select(ISelectable selectable)
        {
            Item item = selectable as Item;
            if (item == null) return;

            _currentlySelectedItem = item;
            _currentlySelectedItem.Select();
        }

        public void DeSelect(ISelectable selectable)
        {
            Item item = selectable as Item;

            if (item == null) return;

            item.DeSelect();
        }

        public void Collect(ICollectable collectable)
        {
            Item item = collectable as Item;
            if (item == null || !item.Collectable) return;

            Tile emptyTile = GlobalBinder.singleton.TileManager.FindEmptyTile();
            if (emptyTile != null)
            {
                emptyTile.Item = item;
                item.transform.DORotate(Vector3.zero, _itemMoveDuration);
                item.Collect();
                GlobalBinder.singleton.LevelManager.UpdateItemCollection(item);
                GlobalBinder.singleton.ItemManager.CollectItem(item);
            }
            else
            {
                Debug.Log("No empty tile available to place the item.");
                GlobalBinder.singleton.LevelManager.LevelFail();
            }

            GlobalBinder.singleton.TileManager.AlignMatchingItems();
        }

        private void OnDrawGizmos()
        {
            if (_selectionCamera == null) return;

            Vector3 rayOrigin = _selectionCamera.transform.position;
            Vector3 rayDirection = _selectionCamera.ScreenPointToRay(_playerInput.MousePosition).direction;

            Gizmos.color = _playerInput.IsMouseHeld ? _mouseHoldGizmoColor :
                           _playerInput.IsMouseUp ? _mouseUpGizmoColor : _mouseDownGizmoColor;
            Gizmos.DrawRay(rayOrigin, rayDirection * _raycastLength);
        }
    }
}