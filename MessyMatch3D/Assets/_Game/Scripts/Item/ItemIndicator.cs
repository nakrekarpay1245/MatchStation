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
            if (_quantity > 0)
            {
                _quantity--;
                if (_quantityText != null)
                {
                    _quantityText.text = _quantity.ToString();
                }
            }
        }
    }
}