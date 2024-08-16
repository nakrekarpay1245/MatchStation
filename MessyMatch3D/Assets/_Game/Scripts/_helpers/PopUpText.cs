using TMPro;
using UnityEngine;
using DG.Tweening;

namespace _Game.Scripts._helpers
{
    /// <summary>
    /// Handles the pop-up text behavior, including setting the text and animating its appearance.
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class PopUpText : MonoBehaviour
    {
        private TextMeshPro _textComponent;
        private float _initialScale;

        private void Awake()
        {
            // Cache the TextMeshPro component and initial scale.
            _textComponent = GetComponent<TextMeshPro>();
            _initialScale = transform.localScale.x;
        }

        private void OnEnable()
        {
            // Reset scale and animate the pop-up appearance.
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one * _initialScale, 0.5f).SetEase(Ease.OutBack);
        }

        /// <summary>
        /// Sets the text of the pop-up.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public void SetText(string text)
        {
            _textComponent.text = text;
        }
    }
}