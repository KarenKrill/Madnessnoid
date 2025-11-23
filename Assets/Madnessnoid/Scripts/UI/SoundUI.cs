using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Madnessnoid
{
    using Abstractions;

    public class SoundUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerEntered = true;
            if (!_isSelected && _IsInteractable)
            {
                if (_hoverSound != null)
                {
                    _audioController?.PlaySfx(_hoverSound);
                }
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerEntered = false;
        }
        public void OnSelect(BaseEventData eventData)
        {
            _isSelected = true;
            if (!_isPointerEntered && _IsInteractable)
            {
                if (_selectSound != null)
                {
                    _audioController?.PlaySfx(_selectSound);
                }
            }
        }
        public void OnDeselect(BaseEventData eventData)
        {
            _isSelected = false;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_IsInteractable && _clickSound != null)
            {
                _audioController?.PlaySfx(_clickSound);
            }
        }

        private bool _IsInteractable => !_isAttachedToSelectable || _selectable.interactable;
        [SerializeField]
        private AudioClip _hoverSound = null;
        [SerializeField]
        private AudioClip _selectSound = null;
        [SerializeField]
        private AudioClip _clickSound = null;
        private static IAudioController _audioController;
        private bool _isSelected = false;
        private bool _isPointerEntered = false;
        private bool _isAttachedToSelectable = false;
        private Selectable _selectable;

        private void Awake()
        {
            _isAttachedToSelectable = TryGetComponent(out _selectable);
        }
        private void Start()
        {
            _audioController ??= FindFirstObjectByType<AudioController>();
        }
    }
}
