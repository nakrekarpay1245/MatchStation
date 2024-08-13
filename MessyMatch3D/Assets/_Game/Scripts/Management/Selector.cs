using _Game.Scripts.Data;
using _Game.Scripts._helpers;
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

        [Header("Dependencies")]
        [Tooltip("The PlayerInput scriptable object that stores input data.")]
        [SerializeField]
        private PlayerInput _playerInput;

        private ISelectable _lastSelectable;
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
            if (_selectionCamera == null) return;

            Ray ray = _selectionCamera.ScreenPointToRay(_playerInput.MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastLength, _raycastLayerMask))
            {
                var selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    _lastSelectable = selectable;
                    Select(selectable);
                }
            }
        }

        private void HandleMouseButton()
        {
            if (_selectionCamera == null) return;

            Ray ray = _selectionCamera.ScreenPointToRay(_playerInput.MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastLength, _raycastLayerMask))
            {
                var selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    if (_lastSelectable != selectable)
                    {
                        if (_lastSelectable != null)
                        {
                            DeSelect(_lastSelectable);
                        }
                        _lastSelectable = selectable;
                    }

                    Select(selectable);
                }
                else
                {
                    if (_lastSelectable != null)
                    {
                        DeSelect(_lastSelectable);
                    }
                }
            }
        }

        private void HandleMouseButtonUp()
        {
            if (_lastSelectable != null)
            {
                DeSelect(_lastSelectable);
            }

            if (_selectionCamera == null) return;

            Ray ray = _selectionCamera.ScreenPointToRay(_playerInput.MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastLength, _raycastLayerMask))
            {
                var collectable = hit.collider.GetComponent<ICollectable>();
                if (collectable != null)
                {
                    Collect(collectable);
                }
            }
        }

        public void Select(ISelectable selectable)
        {
            selectable.Select();
        }

        public void DeSelect(ISelectable selectable)
        {
            selectable.DeSelect();
        }

        public void Collect(ICollectable collectable)
        {
            GlobalBinder.singleton.ItemManager.Collect(collectable);
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