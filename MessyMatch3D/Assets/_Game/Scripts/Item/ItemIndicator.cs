using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.Items
{
    /// <summary>
    /// Manages the display of an item indicator including the icon and quantity text.
    /// </summary>
    public class ItemIndicator : MonoBehaviour
    {
        [Header("UI Components")]
        [Tooltip("The icon image for the item.")]
        [SerializeField]
        private Image _itemIcon;

        [Tooltip("The text component displaying the quantity.")]
        [SerializeField]
        private TextMeshProUGUI _quantityText;

        [Header("Scale Effects Parameters")]
        [Tooltip("")]
        [SerializeField]
        private float _scaleEffectDuration = 0.5f;
        [Tooltip("")]
        [SerializeField]
        private float _scalePopAmount = 1f;

        private int _quantity;

        /// <summary>
        /// Sets the icon of the indicator.
        /// </summary>
        /// <param name="icon">The icon to display.</param>
        public void SetIcon(Sprite icon)
        {
            if (_itemIcon != null)
            {
                _itemIcon.sprite = icon;
            }
        }

        /// <summary>
        /// Sets the text displaying the quantity.
        /// </summary>
        /// <param name="quantity">The quantity to display.</param>
        public void SetText(string quantity)
        {
            if (_quantityText != null)
            {
                _quantityText.text = quantity;
            }

            _quantity = int.Parse(quantity);
        }

        /// <summary>
        /// Decreases the displayed quantity by one.
        /// </summary>
        public void DecreaseQuantity()
        {
            _quantity--;

            if (_quantity > 0)
            {
                if (_quantityText != null)
                {
                    _quantityText.text = _quantity.ToString();
                }

                transform.DOPunchScale(Vector3.one * _scalePopAmount, _scaleEffectDuration, 5, 0.1f).OnComplete(() =>
                {
                    transform.DOScale(1, 0f);
                });
            }
            else
            {
                transform.DOScale(0, _scaleEffectDuration).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            }
        }
    }
}