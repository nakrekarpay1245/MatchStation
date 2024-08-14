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

    [Header("Button Color Effect")]
    [Tooltip("Toggle to determine if color effects should be applied.")]
    [SerializeField] private bool _useButtonColorEffect = true;
    public bool UseButtonColorEffect => _useButtonColorEffect;

    [Header("Text Color Effect")]
    [Tooltip("Toggle to determine if color effects should be applied.")]
    [SerializeField] private bool _useTextColorEffect = true;
    public bool UseTextColorEffect => _useTextColorEffect;

    [Header("Sound Effect")]
    [Tooltip("Toggle to determine whether to use sound effects.")]
    [SerializeField] private bool _useSoundEffect = true;
    public bool UseSoundEffect => _useSoundEffect;

    [Header("Button Colors")]
    [Tooltip("Color when the mouse is not interacting with the button.")]
    [SerializeField, HideInInspector] private Color _buttonNormalColor = new Color(232f, 232f, 232f);

    [Tooltip("Color when the mouse is over the button.")]
    [SerializeField, HideInInspector] private Color _buttonHoverColor = new Color(190f, 190f, 179f);

    [Tooltip("Color when the button is pressed.")]
    [SerializeField, HideInInspector] private Color _buttonPressedColor = new Color(175f, 175f, 175f);

    [Header("Text Colors")]
    [Tooltip("Text color when the mouse is not interacting with the button.")]
    [SerializeField, HideInInspector] private Color _textNormalColor = new Color(232f, 232f, 232f);

    [Tooltip("Text color when the mouse is over the button.")]
    [SerializeField, HideInInspector] private Color _textHoverColor = new Color(190f, 190f, 179f);

    [Tooltip("Text color when the button is pressed.")]
    [SerializeField, HideInInspector] private Color _textPressedColor = new Color(175f, 175f, 175f);

    [Header("Highlight")]
    [Tooltip("Image component to show when the button is selected. Active only if Image effect is enabled.")]
    [SerializeField, HideInInspector] private Image _highlightImage;
    [SerializeField, HideInInspector] private float _hightlightImageVisibleAlpha;
    [SerializeField, HideInInspector] private float _hightlightImageHiddenAlpha;
    [SerializeField, HideInInspector] private float _hightlightImageVisibleScale;
    [SerializeField, HideInInspector] private float _hightlightImageHiddenScale;
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
                _buttonImage.color = _buttonNormalColor;
        }

        if (_useTextColorEffect)
        {
            _buttonText = GetComponentInChildren<TextMeshProUGUI>();
            if (_buttonText)
                _buttonText.color = _textNormalColor;
        }

        if (_useHighlightEffect && _highlightImage)
        {
            Highlight(false);
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
        if (_useButtonColorEffect)
        {
            if (_buttonImage)
                _buttonImage.DOColor(_buttonHoverColor, _transitionDuration);
        }

        if (_useTextColorEffect)
        {
            if (_buttonText)
                _buttonText.DOColor(_textHoverColor, _transitionDuration);
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
        if (_useButtonColorEffect)
        {
            if (_buttonImage)
                _buttonImage.DOColor(_buttonNormalColor, _transitionDuration);
        }

        if (_useTextColorEffect)
        {
            if (_buttonText)
                _buttonText.DOColor(_textNormalColor, _transitionDuration);
        }

        if (_useHighlightEffect) Highlight(false);
    }

    /// <summary>
    /// Changes the button color to pressedColor when the button is pressed and invokes the OnPressed event.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    private void OnPointerDown(BaseEventData eventData)
    {
        if (_useButtonColorEffect)
        {
            if (_buttonImage)
                _buttonImage.DOColor(_buttonPressedColor, _transitionDuration);
        }

        if (_useTextColorEffect)
        {
            if (_buttonText)
                _buttonText.DOColor(_textPressedColor, _transitionDuration);
        }

        GlobalBinder.singleton.AudioManager.PlaySound(_buttonPressClipKey);

        if (_useHighlightEffect) Highlight(true);

        OnPressed?.Invoke();
    }

    /// <summary>
    /// Changes the button color back to hoverColor if the mouse is still over the button after release.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    private void OnPointerUp(BaseEventData eventData)
    {
        if (_useButtonColorEffect)
        {
            if (eventData is PointerEventData pointerEventData && pointerEventData.hovered.Contains(gameObject))
            {
                if (_useButtonColorEffect && _buttonImage)
                    _buttonImage.DOColor(_buttonPressedColor, _transitionDuration);

                if (_useTextColorEffect && _buttonText)
                    _buttonText.DOColor(_textPressedColor, _transitionDuration);
            }
            else
            {
                if (_useButtonColorEffect && _buttonImage)
                    _buttonImage.DOColor(_buttonNormalColor, _transitionDuration);

                if (_useTextColorEffect && _buttonText)
                    _buttonText.DOColor(_textNormalColor, _transitionDuration);
            }
        }

        if (_useHighlightEffect) Highlight(false);
    }

    public void DeactivateButton()
    {
        if (_useButtonColorEffect)
        {
            if (_useButtonColorEffect && _buttonImage)
                _buttonImage.DOColor(_buttonNormalColor, _transitionDuration);

            if (_useTextColorEffect && _buttonText)
                _buttonText.DOColor(_textNormalColor, _transitionDuration);
        }

        if (_useHighlightEffect) Highlight(false);
    }

    /// <summary>
    /// Highlights or hides the highlight image using DOTween, based on the toggle.
    /// </summary>
    /// <param name="highlight">True to highlight, false to hide.</param>
    private void Highlight(bool highlight)
    {
        if (_useHighlightEffect && _highlightImage)
        {
            float alpha = highlight ? _hightlightImageVisibleAlpha : _hightlightImageHiddenAlpha;
            float scale = highlight ? _hightlightImageVisibleScale : _hightlightImageHiddenScale;
            _highlightImage.GetComponent<CanvasGroup>().DOFade(alpha, _transitionDuration);
            _highlightImage.transform.DOScale(scale, _transitionDuration);
        }
    }
}