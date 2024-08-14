using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Events;
using _Game.Scripts.Data;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Game Data")]
    [Tooltip("")]
    [SerializeField] private GameData _gameData;

    [Header("UI Elements")]
    [Tooltip("The Canvas Group that controls the level start menu's visibility.")]
    [SerializeField] private CanvasGroup _levelStartMenu;

    [Tooltip("The RectTransform of the level start menu.")]
    [SerializeField] private RectTransform _menuRectTransform;

    [Header("Animation Settings")]
    [Tooltip("The duration for the fade and move animations.")]
    [Range(0.1f, 1f)]
    [SerializeField] private float _animationDuration = 0.25f;

    [Tooltip("The target Y position for the menu when it appears on screen.")]
    [SerializeField] private float _targetPositionY = 0f;

    [Tooltip("The initial Y position of the menu when it is off screen.")]
    [SerializeField] private float _initialPositionY = -2160f;

    [Header("Scene Management")]
    [Tooltip("The index of the scene to load when starting the game.")]
    [SerializeField] private int _gameSceneIndex = 1;

    [Header("Level UI")]
    [Tooltip("")]
    [SerializeField] private TextMeshProUGUI _levelStartMenuLevelText;
    [Tooltip("")]
    [SerializeField] private TextMeshProUGUI _menuLevelText;

    private void Awake()
    {
        // Fade out the menu and move it to the initial position
        _levelStartMenu.DOFade(0f, 0f);
        _menuRectTransform.DOAnchorPosY(_initialPositionY, 0f);

        _levelStartMenuLevelText.text = "LEVEL " + (_gameData.CurrentLevelIndex + 1).ToString();
        _menuLevelText.text = "LEVEL " + (_gameData.CurrentLevelIndex + 1).ToString();
    }

    /// <summary>
    /// Shows the level start menu with a fade-in effect and moves it from the bottom to its target position.
    /// </summary>
    public void ShowLevelStartMenu()
    {
        // Reset menu position to off-screen
        _menuRectTransform.anchoredPosition = new Vector2(_menuRectTransform.anchoredPosition.x,
            _initialPositionY);

        // Fade in the menu and move it to the target position
        _levelStartMenu.DOFade(1f, _animationDuration).SetEase(Ease.OutQuad);
        _menuRectTransform.DOAnchorPosY(_targetPositionY, _animationDuration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// Hides the level start menu with a fade-out effect and moves it back to its initial off-screen position.
    /// </summary>
    public void HideLevelStartMenu()
    {
        // Fade out the menu and move it off-screen
        _levelStartMenu.DOFade(0f, _animationDuration).SetEase(Ease.InOutQuad);
        _menuRectTransform.DOAnchorPosY(_initialPositionY, _animationDuration).SetEase(Ease.InQuad);
    }

    /// <summary>
    /// Loads the game scene with the specified index.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(_gameSceneIndex);
    }

    /// <summary>
    /// Opens the specified URL in the default web browser.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}