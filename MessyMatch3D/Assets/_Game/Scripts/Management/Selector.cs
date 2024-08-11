using _Game.Scripts._helpers;
using _Game.Scripts.Items;
using _Game.Scripts.Management;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    /// <summary>
    /// Handles the selection of items and their interaction.
    /// </summary>
    public class Selector : MonoBehaviour, ISelector, ICollector
    {
        [Header("Selector Params")]
        [SerializeField]
        private string _selectParticleKey = "Select";
        [SerializeField]
        private string _selectClipKey = "Select";

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

        private bool _isMouseHeld = false;
        private Item _lastHoveredItem;
        private Item _currentlySelectedItem;

        private void Update()
        {
            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseButtonDown();
            }
            else if (Input.GetMouseButton(0))
            {
                HandleMouseButton();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleMouseButtonUp();
            }
        }

        private void HandleMouseButtonDown()
        {
            _isMouseHeld = true;
            CheckForItemAtMouse();
        }

        private void HandleMouseButton()
        {
            if (_isMouseHeld)
            {
                CheckForItemAtMouse();
            }
        }

        private void HandleMouseButtonUp()
        {
            _isMouseHeld = false;
            CheckForItemAtMouse();

            if (_currentlySelectedItem != null)
            {
                Collect(_currentlySelectedItem);
            }

            Debug.Log("Mouse button released. Last hovered item: " +
                (_lastHoveredItem ? _lastHoveredItem.name : "None"));
        }

        private void CheckForItemAtMouse()
        {
            if (_selectionCamera == null) return;

            Ray ray = _selectionCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _raycastLength, _raycastLayerMask))
            {
                var selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    _lastHoveredItem = selectable as Item;
                    if (_isMouseHeld)
                    {
                        Select(_lastHoveredItem);
                    }
                }
                else
                {
                    if (_isMouseHeld && _currentlySelectedItem != null)
                    {
                        DeSelect(_currentlySelectedItem);
                    }
                    Debug.Log("No item detected.");
                }
            }
            else
            {
                if (_isMouseHeld && _currentlySelectedItem != null)
                {
                    DeSelect(_currentlySelectedItem);
                }
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

            if (item == null) return;

            // Find an empty tile and assign the item to it
            Tile emptyTile = GlobalBinder.singleton.TileManager.FindEmptyTile();
            if (emptyTile != null)
            {
                // Update tile and item references
                emptyTile.Item = item;
                item.transform.position = emptyTile.transform.position;
                item.transform.rotation = Quaternion.identity; // Reset rotation to (0,0,0)

                // Optionally, animate item to move to the tile's position using DOTween
                item.transform.DOMove(emptyTile.transform.position + Vector3.up, 0.5f);
                item.transform.DORotate(Vector3.zero, 0.5f);

                // Collect the item (optional)
                item.Collect();
            }
            else
            {
                Debug.Log("No empty tile available to place the item.");
            }
        }

        private void OnDrawGizmos()
        {
            if (_selectionCamera == null) return;

            Vector3 rayOrigin = _selectionCamera.transform.position;
            Vector3 rayDirection = _selectionCamera.ScreenPointToRay(Input.mousePosition).direction;

            // Draw raycast for different mouse states
            if (Input.GetMouseButton(0))
            {
                Gizmos.color = _mouseHoldGizmoColor;
                Gizmos.DrawRay(rayOrigin, rayDirection * _raycastLength);
            }
            else
            {
                Gizmos.color = _mouseUpGizmoColor;
                Gizmos.DrawRay(rayOrigin, rayDirection * _raycastLength);
            }
        }
    }
}