using UnityEngine;
using _Game.Scripts.Data;

namespace _Game.Scripts
{
    /// <summary>
    /// Handles user input and forwards the input data to the PlayerInput scriptable object.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField, Tooltip("Scriptable object for handling player input.")]
        private PlayerInput _playerInput;

        private void Update()
        {
            HandleMouseInput();
        }

        /// <summary>
        /// Handles mouse input and updates the PlayerInput scriptable object.
        /// </summary>
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _playerInput.SetMouseDown(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                _playerInput.SetMouseHeld(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _playerInput.SetMouseUp(Input.mousePosition);
            }
        }
    }
}