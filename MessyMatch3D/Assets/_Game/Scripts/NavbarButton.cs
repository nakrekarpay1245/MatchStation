using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using UnityEngine.Events;
using TMPro; // Ensure you have this namespace for TextMeshProUGUI

/// <summary>
/// Handles the behavior of a navigation button, including color transitions, scaling, positioning, and alpha transitions for the button icon and text.
/// </summary>
[RequireComponent(typeof(EventTrigger))]
public class NavbarButton : MonoBehaviour
{
    #region Fields

    [Header("Button Colors")]
    [Tooltip("Color of the button when it is active.")]
    [SerializeField] private Color _activeColor = Color.green;

    [Tooltip("Color of the button when it is inactive.")]
    [SerializeField] private Color _inactiveColor = Color.gray;

    [Header("Transition Settings")]
    [Tooltip("Duration of the color transition.")]
    [Range(0.1f, 2f)][SerializeField] private float _transitionDuration = 0.5f;

    [Tooltip("Duration of the scale and position transition.")]
    [Range(0.1f, 2f)][SerializeField] private float _scalePositionDuration = 0.5f;

    [Header("Button Icon Settings")]
    [SerializeField] private Image _buttonIcon;

    [Tooltip("Scale value when the button icon is active.")]
    [SerializeField] private Vector3 _activeScale = new Vector3(1.2f, 1.2f, 1.2f);

    [Tooltip("Scale value when the button icon is inactive.")]
    [SerializeField] private Vector3 _inactiveScale = Vector3.one;

    [Tooltip("Position offset when the button icon is active.")]
    [SerializeField] private Vector3 _activePositionOffset = new Vector3(0, 10, 0);

    [Tooltip("Position offset when the button icon is inactive.")]
    [SerializeField] private Vector3 _inactivePositionOffset = Vector3.zero;

    [Tooltip("Alpha value when the button icon is active.")]
    [Range(0f, 1f)][SerializeField] private float _activeAlpha = 1f;

    [Tooltip("Alpha value when the button icon is inactive.")]
    [Range(0f, 1f)][SerializeField] private float _inactiveAlpha = 0.5f;

    [Header("Button Text Settings")]
    [SerializeField] private TextMeshProUGUI _buttonText;

    [Tooltip("Color of the text when the button is active.")]
    [SerializeField] private Color _activeTextColor = Color.white;

    [Tooltip("Color of the text when the button is inactive.")]
    [SerializeField] private Color _inactiveTextColor = Color.gray;

    [Header("Button Size Settings")]
    [Tooltip("Size of the button when it is active.")]
    [SerializeField] private Vector2 _activeSize = new Vector2(300, 200);

    [Tooltip("Size of the button when it is inactive.")]
    [SerializeField] private Vector2 _inactiveSize = new Vector2(175, 200);

    [Header("Events")]
    [Tooltip("Event triggered when the button is pressed.")]
    [SerializeField] private UnityEvent _onPressed = new UnityEvent();

    private Image _buttonImage;
    private EventTrigger _eventTrigger;
    private RectTransform _rectTransform;

    public UnityEvent OnPressed => _onPressed;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Cache references to Image, EventTrigger, and TextMeshProUGUI components
        _buttonImage = GetComponent<Image>();
        _eventTrigger = GetComponent<EventTrigger>();
        _rectTransform = GetComponent<RectTransform>();

        // Set up EventTrigger for pointer clicks
        AddEventTrigger(EventTriggerType.PointerClick, HandlePointerClick);

        // Ensure the ButtonIcon and ButtonText are assigned
        if (_buttonIcon == null)
        {
            Debug.LogError("ButtonIcon is not assigned. Please assign it in the Inspector.");
        }

        if (_buttonText == null)
        {
            Debug.LogError("ButtonText is not assigned. Please assign it in the Inspector.");
        }

        SetActive(false);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Updates the button's appearance based on its active state.
    /// </summary>
    /// <param name="isActive">Determines if the button is active or inactive.</param>
    public void SetActive(bool isActive)
    {
        Color targetColor = isActive ? _activeColor : _inactiveColor;
        Vector3 targetScale = isActive ? _activeScale : _inactiveScale;
        Vector3 targetPosition = isActive ? _activePositionOffset : _inactivePositionOffset;
        float targetAlpha = isActive ? _activeAlpha : _inactiveAlpha;
        Color targetTextColor = isActive ? _activeTextColor : _inactiveTextColor;
        Vector2 targetSize = isActive ? _activeSize : _inactiveSize;

        // Transition the button's color
        _buttonImage.DOColor(targetColor, _transitionDuration).SetEase(Ease.InOutSine);

        if (_buttonIcon != null)
        {
            // Transition the button icon's scale and position
            _buttonIcon.transform.DOScale(targetScale, _scalePositionDuration).SetEase(Ease.InOutSine);
            _buttonIcon.transform.DOLocalMove(targetPosition, _scalePositionDuration).SetEase(Ease.InOutSine);

            // Transition the button icon's alpha
            CanvasGroup buttonIconCanvasGroup = _buttonIcon.GetComponent<CanvasGroup>();
            if (buttonIconCanvasGroup != null)
            {
                buttonIconCanvasGroup.DOFade(targetAlpha, _transitionDuration).SetEase(Ease.InOutSine);
            }
            else
            {
                Debug.LogWarning("CanvasGroup component is missing from ButtonIcon. Alpha transition will not be applied.");
            }
        }

        if (_buttonText != null)
        {
            // Transition the button text's color
            _buttonText.DOColor(targetTextColor, _transitionDuration).SetEase(Ease.InOutSine);
        }

        if (_rectTransform != null)
        {
            // Transition the button's size
            _rectTransform.DOSizeDelta(targetSize, _transitionDuration).SetEase(Ease.InOutSine);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Invokes the OnPressed event when the button is clicked.
    /// </summary>
    private void HandlePointerClick(BaseEventData data)
    {
        _onPressed?.Invoke();
    }

    /// <summary>
    /// Adds an event trigger for the specified event type.
    /// </summary>
    /// <param name="eventType">The type of event to listen for.</param>
    /// <param name="action">The action to execute when the event occurs.</param>
    private void AddEventTrigger(EventTriggerType eventType, Action<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(data => action.Invoke((BaseEventData)data));
        _eventTrigger.triggers.Add(entry);
    }

    #endregion
}