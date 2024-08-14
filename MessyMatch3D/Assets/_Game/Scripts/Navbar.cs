using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

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

        [Header("")]
        [Tooltip("")]
        [SerializeField] private List<NavbarButton> _navbarButtonList;

        private void Awake()
        {
            // Register each LeafButton's OnPressed event to the MoveToMenu function
            foreach (NavbarButton button in GetComponentsInChildren<NavbarButton>())
            {
                _navbarButtonList.Add(button);
                button.OnPressed.AddListener(() => MoveToMenu(button));
            }
        }

        private void Start()
        {
            NavbarButton button = transform.GetChild(2).GetComponent<NavbarButton>();
            button.SetActive(true);
        }

        /// <summary>
        /// Moves the MenuParent to center the menu corresponding to the pressed LeafButton
        /// and resizes the buttons accordingly.
        /// </summary>
        /// <param name="button">The button that was pressed, indicating which menu to center.</param>
        private void MoveToMenu(NavbarButton button)
        {
            // Calculate the width of the MenuParent and the correct offset for centering
            float parentWidth = _menuParent.rect.width;
            int menuCount = _menuParent.childCount;

            // Calculate the new anchored position to center the target menu
            float targetPositionX = (-parentWidth / menuCount * button.transform.GetSiblingIndex()) +
                ((parentWidth / menuCount) * 2);
            Vector2 newAnchoredPosition = new Vector2(targetPositionX, _menuParent.anchoredPosition.y);

            // Move the MenuParent to center the target menu using DOTween for smooth transition
            _menuParent.DOAnchorPos(newAnchoredPosition, _transitionDuration).SetEase(_transitionEase);

            foreach (NavbarButton navbarButton in _navbarButtonList)
            {
                if (navbarButton == button)
                {
                    navbarButton.SetActive(true);
                }
                else
                {
                    navbarButton.SetActive(false);
                }
            }
        }
    }
}