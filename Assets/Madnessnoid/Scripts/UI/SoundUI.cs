using UnityEngine;
using UnityEngine.EventSystems;

namespace Madnessnoid
{
    using Abstractions;

    public class SoundUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerEntered = true;
            if (!_isSelected)
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
            if (!_isPointerEntered)
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
            if (_clickSound != null)
            {
                _audioController?.PlaySfx(_clickSound);
            }
        }

        [SerializeField]
        private AudioClip _hoverSound = null;
        [SerializeField]
        private AudioClip _selectSound = null;
        [SerializeField]
        private AudioClip _clickSound = null;
        private static IAudioController _audioController;
        private bool _isSelected = false;
        private bool _isPointerEntered = false;

        private void Start()
        {
            _audioController ??= FindFirstObjectByType<AudioController>();
        }
    }
}
