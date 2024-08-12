using UnityEngine;
using UnityEngine.Events;

namespace _Game.Scripts._Data
{
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "Data/PlayerInput")]
    public class PlayerInput : ScriptableObject
    {
        public UnityAction OnMouseDown;
        public UnityAction OnMouseHeld;
        public UnityAction OnMouseUp;

        public Vector3 MousePosition { get; private set; }
        public bool IsMouseHeld { get; private set; }
        public bool IsMouseUp { get; private set; }

        public void SetMouseDown(Vector3 position)
        {
            MousePosition = position;
            IsMouseHeld = true;
            OnMouseDown?.Invoke();
        }

        public void SetMouseHeld(Vector3 position)
        {
            MousePosition = position;
            OnMouseHeld?.Invoke();
        }

        public void SetMouseUp(Vector3 position)
        {
            MousePosition = position;
            IsMouseHeld = false;
            IsMouseUp = true;
            OnMouseUp?.Invoke();
        }
    }
}