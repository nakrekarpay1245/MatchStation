using _Game.Scripts.Data;
using _Game.Scripts._helpers;
using UnityEngine;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Handles the selection and interaction with items in the game.
    /// Utilizes PlayerInput to manage user interactions with selectable and collectable objects.
    /// </summary>
    public class Selector : MonoBehaviour, ISelector, ICollector
    {
        [Header("Camera Parameters")]
        [SerializeField, Tooltip("The camera used for item selection.")]
        private Camera _selectionCamera;

        [Header("Raycast Parameters")]
        [SerializeField, Tooltip("The layer mask used for raycasting.")]
        private LayerMask _raycastLayerMask = ~0; // Default to all layers

        [SerializeField, Tooltip("The length of the raycast.")]
        private float _raycastLength = 100f;

        [Header("Raycast Gizmo Colors")]
        [SerializeField, Tooltip("The color of the raycast for mouse down.")]
        private Color _mouseDownGizmoColor = Color.green;

        [SerializeField, Tooltip("The color of the raycast for mouse held.")]
        private Color _mouseHoldGizmoColor = Color.yellow;

        [SerializeField, Tooltip("The color of the raycast for mouse up.")]
        private Color _mouseUpGizmoColor = Color.red;

        [Header("Dependencies")]
        [SerializeField, Tooltip("The PlayerInput scriptable object that stores input data.")]
        private PlayerInput _playerInput;

        private ISelectable _currentSelectable;

        private void OnEnable()
        {
            SubscribeToInputEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromInputEvents();
        }

        /// <summary>
        /// Subscribes to input events from the PlayerInput scriptable object.
        /// </summary>
        private void SubscribeToInputEvents()
        {
            _playerInput.OnMouseDown += HandleMouseButtonDown;
            _playerInput.OnMouseHeld += HandleMouseHeld;
            _playerInput.OnMouseUp += HandleMouseButtonUp;
        }

        /// <summary>
        /// Unsubscribes from input events from the PlayerInput scriptable object.
        /// </summary>
        private void UnsubscribeFromInputEvents()
        {
            _playerInput.OnMouseDown -= HandleMouseButtonDown;
            _playerInput.OnMouseHeld -= HandleMouseHeld;
            _playerInput.OnMouseUp -= HandleMouseButtonUp;
        }

        /// <summary>
        /// Handles the logic when the mouse button is pressed down.
        /// Selects the item under the mouse cursor.
        /// </summary>
        private void HandleMouseButtonDown()
        {
            PerformRaycastAction(hit =>
            {
                var selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    _currentSelectable = selectable;
                    Select(_currentSelectable);
                }
            });
        }

        /// <summary>
        /// Handles the logic when the mouse button is held.
        /// Continuously selects the item under the mouse cursor and deselects the previous one.
        /// </summary>
        private void HandleMouseHeld()
        {
            PerformRaycastAction(hit =>
            {
                var selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null && _currentSelectable != selectable)
                {
                    DeSelect(_currentSelectable);
                    _currentSelectable = selectable;
                    Select(_currentSelectable);
                }
            });
        }

        /// <summary>
        /// Handles the logic when the mouse button is released.
        /// Deselects the current item and attempts to collect an item under the mouse cursor.
        /// </summary>
        private void HandleMouseButtonUp()
        {
            if (_currentSelectable != null)
            {
                DeSelect(_currentSelectable);
                _currentSelectable = null;
            }

            PerformRaycastAction(hit =>
            {
                var collectable = hit.collider.GetComponent<ICollectable>();
                if (collectable != null)
                {

                    Collect(collectable);
                }
            });
        }

        /// <summary>
        /// Performs a raycast and executes the given action if an object is hit.
        /// </summary>
        /// <param name="onHit">Action to perform on a raycast hit.</param>
        private void PerformRaycastAction(System.Action<RaycastHit> onHit)
        {
            if (_selectionCamera == null) return;

            Ray ray = _selectionCamera.ScreenPointToRay(_playerInput.MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastLength, _raycastLayerMask))
            {
                onHit?.Invoke(hit);
            }
        }

        /// <summary>
        /// Selects the given selectable object.
        /// </summary>
        /// <param name="selectable">The object to select.</param>
        public void Select(ISelectable selectable)
        {
            selectable?.Select();
        }

        /// <summary>
        /// Deselects the given selectable object.
        /// </summary>
        /// <param name="selectable">The object to deselect.</param>
        public void DeSelect(ISelectable selectable)
        {
            selectable?.DeSelect();
        }

        /// <summary>
        /// Collects the given collectable object.
        /// </summary>
        /// <param name="collectable">The object to collect.</param>
        public void Collect(ICollectable collectable)
        {
            GlobalBinder.singleton.ItemManager.Collect(collectable);
        }

        /// <summary>
        /// Draws raycast gizmos in the editor for visualization.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_selectionCamera == null || _playerInput == null) return;

            Vector3 rayOrigin = _selectionCamera.transform.position;
            Vector3 rayDirection = _selectionCamera.ScreenPointToRay(_playerInput.MousePosition).direction;

            Gizmos.color = _playerInput.IsMouseHeld ? _mouseHoldGizmoColor :
                           _playerInput.IsMouseUp ? _mouseUpGizmoColor : _mouseDownGizmoColor;
            Gizmos.DrawRay(rayOrigin, rayDirection * _raycastLength);
        }
    }
}