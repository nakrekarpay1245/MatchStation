using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts._helpers
{
    /// <summary>
    /// Manages a pool of PopUpText instances and controls their animation and display.
    /// </summary>
    public class PopUpTextManager : MonoBehaviour
    {
        [Header("Pop Up Text Manager")]
        [Tooltip("Prefab for the PopUpText.")]
        [SerializeField] private PopUpText _popUpTextPrefab;

        [Tooltip("Size of the object pool.")]
        [SerializeField] private int _poolSize = 10;

        [Tooltip("Randomizes the position of the pop-up text.")]
        [SerializeField] private float _positionRandomizer = 0.1f;

        [Tooltip("Override for the vertical position adjustment of the pop-up text.")]
        [SerializeField] private float _verticalPositionOverride = 0.25f;

        [Tooltip("Scale applied to the pop-up text during animation.")]
        [SerializeField] private Vector3 _popUpTextScale = Vector3.one;

        private ObjectPool<PopUpText> _popUpTextPool;

        // Animation settings
        private readonly float _popUpTextAnimationDuration = 0.25f;
        private readonly float _popUpTextAnimationDelay = 0.125f;

        private void Awake()
        {
            // Initialize the object pool with the specified size.
            _popUpTextPool = new ObjectPool<PopUpText>(InstantiatePopUpText, _poolSize);
        }

        /// <summary>
        /// Instantiates a new PopUpText instance and adds it to the pool.
        /// </summary>
        /// <returns>A new PopUpText instance.</returns>
        private PopUpText InstantiatePopUpText()
        {
            PopUpText popUpTextInstance = Instantiate(_popUpTextPrefab, transform);
            popUpTextInstance.gameObject.SetActive(false);
            return popUpTextInstance;
        }

        /// <summary>
        /// Displays a pop-up text at the specified position with optional custom duration.
        /// </summary>
        /// <param name="position">The position where the pop-up text will appear.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="duration">The duration before the pop-up text is hidden.</param>
        public void ShowPopUpText(Vector3 position, string text, float duration = 0.25f)
        {
            PopUpText popUpText = _popUpTextPool.GetObject();
            SetPopUpTextPosition(popUpText, position);
            popUpText.SetText(text);

            AnimatePopUpText(popUpText, duration);
        }

        /// <summary>
        /// Sets the position of the pop-up text with a randomized offset.
        /// </summary>
        /// <param name="popUpText">The PopUpText instance.</param>
        /// <param name="position">The target position.</param>
        private void SetPopUpTextPosition(PopUpText popUpText, Vector3 position)
        {
            popUpText.transform.position = position +
                Vector3.right * Random.Range(-_positionRandomizer, _positionRandomizer) +
                Vector3.up * Random.Range(-_positionRandomizer, _positionRandomizer);
        }

        /// <summary>
        /// Animates the pop-up text and hides it after the specified duration.
        /// </summary>
        /// <param name="popUpText">The PopUpText instance.</param>
        /// <param name="duration">The duration before hiding the pop-up text.</param>
        private void AnimatePopUpText(PopUpText popUpText, float duration)
        {
            Vector3 startPosition = popUpText.transform.position;
            Vector3 endPosition = startPosition + Vector3.up * _verticalPositionOverride;

            Sequence popUpSequence = DOTween.Sequence();

            popUpSequence.Append(popUpText.transform.DOScale(_popUpTextScale, _popUpTextAnimationDuration).SetEase(Ease.OutBack))
                         .Join(popUpText.transform.DOMove(endPosition, _popUpTextAnimationDuration).SetEase(Ease.OutQuad))
                         .AppendInterval(_popUpTextAnimationDelay)
                         .Append(popUpText.transform.DOScale(Vector3.zero, _popUpTextAnimationDuration).SetEase(Ease.InQuad))
                         .Join(popUpText.transform.DOMove(endPosition + Vector3.up * _verticalPositionOverride, _popUpTextAnimationDuration).SetEase(Ease.InQuad))
                         .OnComplete(() => ReturnPopUpTextToPool(popUpText, duration));
        }

        /// <summary>
        /// Returns the pop-up text to the pool after the animation is complete.
        /// </summary>
        /// <param name="popUpText">The PopUpText instance.</param>
        /// <param name="duration">The duration before hiding the pop-up text.</param>
        private void ReturnPopUpTextToPool(PopUpText popUpText, float duration)
        {
            popUpText.gameObject.SetActive(false);
            _popUpTextPool.ReturnObject(popUpText);
        }
    }
}