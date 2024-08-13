using _Game.Scripts.Data;
using UnityEngine;

namespace _Game.Scripts
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField]
        private PlayerInput _playerInput;

        private void Update()
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