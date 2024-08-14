using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;
using _Game.Scripts._helpers;

/// <summary>
/// The LeafButton class handles the color change of an Image or Text component,
/// scaling, and highlight effects when the mouse interacts with it, using EventTrigger.
/// </summary>
[RequireComponent(typeof(EventTrigger))]
public class LeafButton : MonoBehaviour
{
    [Header("Effect Toggles")]
    [Header("Highlight Effect")]
    [Tooltip("Toggle to determine if effects should be applied to the Image component.")]
    [SerializeField] private bool _useHighlightEffect = true;
    public bool UseHighlightEffect => _useHighlightEffect;

    [Header("Text Effect")]
    [Tooltip("Toggle to determine if effects should be applied to the Text component.")]
    [SerializeField] private bool _useTextEffect = false;
    public bool UseTextEffect => _useTextEffect;

    [Header("Color Effect")]
    [Tooltip("Toggle to determine if color effects should be applied.")]
    [SerializeField] private bool _useColorEffect = true;
    public bool UseColorEffect => _useColorEffect;

    [Header("Sound Effect")]
    [Tooltip("Toggle to determine whether to use sound effects.")]
    [SerializeField] private bool _useSoundEffect = true;
    public bool UseSoundEffect => _useSoundEffect;

    [Header("Colors")]
    [Tooltip("Color when the mouse is not interacting with the button.")]
    [SerializeField, HideInInspector] private Color _normalColor = new Color(232f, 232f, 232f);

    [Tooltip("Color when the mouse is over the button.")]
    [SerializeField, HideInInspector] private Color _hoverColor = new Color(190f, 190f, 179f);

    [Tooltip("Color when the button is pressed.")]
    [SerializeField, HideInInspector] private Color _pressedColor = new Color(175f, 175f, 175f);

    [Header("Highlight")]
    [Tooltip("Image component to show when the button is selected. Active only if Image effect is enabled.")]
    [SerializeField, HideInInspector] private Image _highlightImage;

    [Header("Sound Params")]
    [Tooltip("Key for the sound to play when the button is pressed.")]
    [SerializeField, HideInInspector] private string _buttonPressClipKey = "ButtonPressClip";

    [Tooltip("Key for the sound to play when the button is hovered over.")]
    [SerializeField, HideInInspector] private string _buttonHoverClipKey = "ButtonHoverClip";

    private Image _buttonImage;
    [Header("Button Text")]
    [Tooltip("")]
    [SerializeField, HideInInspector] private TextMeshProUGUI _buttonText;

    [Header("Transition Speed")]
    [Tooltip("Duration of the transition effects.")]
    [SerializeField] private float _transitionDuration = 0.25f;

    [Header("Events")]
    [Space]
    [Tooltip("Event to invoke when the button is pressed.")]
    public UnityEvent OnPressed;

    /// <summary>
    /// Initializes the Image and Text components and sets up EventTrigger events.
    /// </summary>
    private void Awake()
    {
        if (_useHighlightEffect)
        {
            _buttonImage = GetComponent<Image>();
            if (_buttonImage)
                _buttonImage.color = _normalColor;
        }

        if (_useTextEffect)
        {
            _buttonText = GetComponent<TextMeshProUGUI>();
            if (_buttonText)
                _buttonText.color = _normalColor;
        }

        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }

        AddEventTrigger(eventTrigger, EventTriggerType.PointerEnter, OnPointerEnter);
        AddEventTrigger(eventTrigger, EventTriggerType.PointerExit, OnPointerExit);
        AddEventTrigger(eventTrigger, EventTriggerType.PointerDown, OnPointerDown);
        AddEventTrigger(eventTrigger, EventTriggerType.PointerUp, OnPointerUp);

        if (_useHighlightEffect && _highlightImage)
        {
            _highlightImage.DOFade(0, 0f);
            _highlightImage.transform.DOScale(0, 0f);
        }
    }

    /// <summary>
    /// Adds an event trigger to the EventTrigger component.
    /// </summary>
    /// <param name="trigger">The EventTrigger component.</param>
    /// <param name="eventType">The type of event to trigger.</param>
    /// <param name="action">The callback function to invoke.</param>
    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType,
        System.Action<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((data) => action.Invoke((BaseEventData)data));
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// Changes the button color to hoverColor when the mouse enters the button.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    private void OnPointerEnter(BaseEventData eventData)
    {
        if (_useColorEffect)
        {
            if (_useHighlightEffect && _buttonImage)
                _buttonImage.DOColor(_hoverColor, _transitionDuration);
            else if (_useTextEffect && _buttonText)
                _buttonText.DOColor(_hoverColor, _transitionDuration);
        }

        GlobalBinder.singleton.AudioManager.PlaySound(_buttonHoverClipKey);

        if (_useHighlightEffect) Highlight(true);
    }

    /// <summary>
    /// Changes the button color to normalColor when the mouse exits the button.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    private void OnPointerExit(BaseEventData eventData)
    {
        if (_useColorEffect)
        {
            if (_useHighlightEffect && _buttonImage)
                _buttonImage.DOColor(_normalColor, _transitionDuration);
            else if (_useTextEffect && _buttonText)
                _buttonText.DOColor(_normalColor, _transitionDuration);
        }

        if (_useHighlightEffect) Highlight(false);
    }

    /// <summary>
    /// Changes the button color to pressedColor when the button is pressed and invokes the OnPressed event.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    private void OnPointerDown(BaseEventData eventData)
    {
        if (_useColorEffect)
        {
            if (_useHighlightEffect && _buttonImage)
                _buttonImage.DOColor(_pressedColor, _transitionDuration);
            else if (_useTextEffect && _buttonText)
                _buttonText.DOColor(_pressedColor, _transitionDuration);
        }

        GlobalBinder.singleton.AudioManager.PlaySound(_buttonPressClipKey);

        OnPressed?.Invoke();
    }

    /// <summary>
    /// Changes the button color back to hoverColor if the mouse is still over the button after release.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    private void OnPointerUp(BaseEventData eventData)
    {
        if (_useColorEffect)
        {
            if (eventData is PointerEventData pointerEventData && pointerEventData.hovered.Contains(gameObject))
            {
                if (_useHighlightEffect && _buttonImage)
                    _buttonImage.DOColor(_hoverColor, _transitionDuration);
                else if (_useTextEffect && _buttonText)
                    _buttonText.DOColor(_hoverColor, _transitionDuration);
            }
            else
            {
                if (_useHighlightEffect && _buttonImage)
                    _buttonImage.DOColor(_normalColor, _transitionDuration);
                else if (_useTextEffect && _buttonText)
                    _buttonText.DOColor(_normalColor, _transitionDuration);
            }
        }
    }

    /// <summary>
    /// Highlights or hides the highlight image using DOTween, based on the toggle.
    /// </summary>
    /// <param name="highlight">True to highlight, false to hide.</param>
    private void Highlight(bool highlight)
    {
        if (_useHighlightEffect && _highlightImage)
        {
            float alpha = highlight ? 1f : 0f;
            _highlightImage.DOFade(alpha, _transitionDuration);
            _highlightImage.transform.DOScale(highlight ? 1 : 0, _transitionDuration);
        }
    }
}