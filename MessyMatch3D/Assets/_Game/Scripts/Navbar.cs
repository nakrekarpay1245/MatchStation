using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// Manages the navigation bar, including moving and centering the menu based on the selected button.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class Navbar : MonoBehaviour
{
    #region Fields

    [Header("Menu Settings")]
    [Tooltip("Parent object containing all the menus.")]
    [SerializeField] private RectTransform _menuParent;

    [Tooltip("Duration of the menu transition.")]
    [SerializeField, Range(0.1f, 2f)] private float _transitionDuration = 0.5f;

    [Tooltip("Ease type for the menu transition.")]
    [SerializeField] private Ease _transitionEase = Ease.InOutQuad;

    [Header("Button List")]
    [Tooltip("List of navigation buttons.")]
    [SerializeField] private List<NavbarButton> _navbarButtonList = new List<NavbarButton>();

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Initialize and register button events
        InitializeButtons();
    }

    private void Start()
    {
        // Optionally set the initial active button
        SetInitialActiveButton();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initializes buttons and assigns their OnPressed events to the MoveToMenu method.
    /// </summary>
    private void InitializeButtons()
    {
        foreach (NavbarButton button in GetComponentsInChildren<NavbarButton>())
        {
            _navbarButtonList.Add(button);
            button.OnPressed.AddListener(() => MoveToMenu(button));
        }
    }

    /// <summary>
    /// Sets the initial active button, if needed.
    /// </summary>
    private void SetInitialActiveButton()
    {
        if (_navbarButtonList.Count > 0)
        {
            _navbarButtonList[2].SetActive(true); // Example for initial setup
        }
    }

    /// <summary>
    /// Moves the MenuParent to center the menu corresponding to the pressed button.
    /// </summary>
    /// <param name="button">The button that was pressed, indicating which menu to center.</param>
    private void MoveToMenu(NavbarButton button)
    {
        // Calculate the width of the MenuParent and the correct offset for centering
        float parentWidth = _menuParent.rect.width;
        int menuCount = _menuParent.childCount;

        // Calculate the new anchored position to center the target menu
        float targetPositionX = -parentWidth / menuCount / 2 * button.transform.GetSiblingIndex() +
            (parentWidth * 2 / menuCount);

        Vector2 newAnchoredPosition = new Vector2(targetPositionX, _menuParent.anchoredPosition.y);

        // Move the MenuParent to center the target menu using DOTween for smooth transition
        _menuParent.DOAnchorPos(newAnchoredPosition, _transitionDuration).SetEase(_transitionEase);

        // Update button states
        UpdateButtonStates(button);
    }

    /// <summary>
    /// Updates the active state of buttons based on the currently selected button.
    /// </summary>
    /// <param name="activeButton">The button to be set active.</param>
    private void UpdateButtonStates(NavbarButton activeButton)
    {
        foreach (var button in _navbarButtonList)
        {
            button.SetActive(button == activeButton);
        }
    }

    #endregion
}