using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using UnityEngine.Events;

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

    [Header("Events")]
    [Tooltip("Event triggered when the button is pressed.")]
    [SerializeField] private UnityEvent _onPressed = new UnityEvent();

    private Image _buttonImage;
    private EventTrigger _eventTrigger;

    public UnityEvent OnPressed => _onPressed;
    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Cache references
        _buttonImage = GetComponent<Image>();
        _eventTrigger = GetComponent<EventTrigger>();

        // Setup EventTrigger entries
        AddEventTrigger(_eventTrigger, EventTriggerType.PointerClick, HandlePointerClick);
    }

    #endregion

    #region Public Methods

    public void SetActive(bool isActive)
    {
        Color targetColor = isActive ? _activeColor : _inactiveColor;
        _buttonImage.DOColor(targetColor, _transitionDuration).SetEase(Ease.InOutSine);
    }

    #endregion

    #region Private Methods

    private void HandlePointerClick(BaseEventData data)
    {
        _onPressed?.Invoke();
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType,
        Action<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((data) => action.Invoke((BaseEventData)data));
        trigger.triggers.Add(entry);
    }

    #endregion
}