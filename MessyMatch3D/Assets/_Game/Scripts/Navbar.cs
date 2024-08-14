using UnityEngine;
using DG.Tweening;

namespace _Game.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class Navbar : MonoBehaviour
    {
        [Header("Menu Settings")]
        [Tooltip("Parent object containing all the menus.")]
        [SerializeField] private RectTransform _menuParent;

        [Tooltip("Duration of the menu transition.")]
        [SerializeField, Range(0.1f, 2f)] private float _transitionDuration = 0.5f;

        [Tooltip("Ease type for the menu transition.")]
        [SerializeField] private Ease _transitionEase = Ease.InOutQuad;

        [Header("Button Size Settings")]
        [Tooltip("Size of the selected button.")]
        [SerializeField] private Vector2 _selectedButtonSize = new Vector2(150f, 150f);

        [Tooltip("Size of the non-selected buttons.")]
        [SerializeField] private Vector2 _nonSelectedButtonSize = new Vector2(100f, 100f);
        private void Awake()
        {
            // Register each LeafButton's OnPressed event to the MoveToMenu function
            foreach (LeafButton button in GetComponentsInChildren<LeafButton>())
            {
                button.OnPressed.AddListener(() => MoveToMenu(button));
            }
        }

        /// <summary>
        /// Moves the MenuParent to center the menu corresponding to the pressed LeafButton
        /// and resizes the buttons accordingly.
        /// </summary>
        /// <param name="button">The button that was pressed, indicating which menu to center.</param>
        private void MoveToMenu(LeafButton button)
        {
            // Get the target menu's RectTransform from the LeafButton
            RectTransform targetMenu = button.GetComponent<RectTransform>();

            if (targetMenu == null)
            {
                Debug.LogError("The LeafButton is not associated with a menu!");
                return;
            }

            // Calculate the width of the MenuParent and the correct offset for centering
            float parentWidth = _menuParent.rect.width;
            int menuCount = _menuParent.childCount;

            // Calculate the new anchored position to center the target menu
            float targetPositionX = (-parentWidth / menuCount * button.transform.GetSiblingIndex()) + ((parentWidth / menuCount) * 2);
            Vector2 newAnchoredPosition = new Vector2(targetPositionX, _menuParent.anchoredPosition.y);

            // Move the MenuParent to center the target menu using DOTween for smooth transition
            _menuParent.DOAnchorPos(newAnchoredPosition, _transitionDuration).SetEase(_transitionEase);

            // Resize the selected button and reset the size of the non-selected buttons
            foreach (LeafButton btn in GetComponentsInChildren<LeafButton>())
            {
                RectTransform btnRectTransform = btn.GetComponent<RectTransform>();

                if (btn == button)
                {
                    // Resize the selected button
                    btnRectTransform.DOSizeDelta(_selectedButtonSize, _transitionDuration).SetEase(_transitionEase);
                }
                else
                {
                    // Reset the size of the non-selected buttons
                    btnRectTransform.DOSizeDelta(_nonSelectedButtonSize, _transitionDuration).SetEase(_transitionEase);
                }
            }
        }
    }
}