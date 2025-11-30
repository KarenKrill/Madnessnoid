using UnityEngine;

namespace Madnessnoid
{
    using Abstractions;

    public class BrickBehaviour : DamagableBehaviour
    {
        public IBrickTheme BrickTheme
        {
            set
            {
                if (_brickTheme != value)
                {
                    _brickTheme = value;
                    OnBrickThemeChanged();
                }
            }
        }

        public void Initialize(IAudioController audioController)
        {
            _audioController = audioController;
        }

        protected override void OnDamaged()
        {
            if (_brickTheme?.DamageSounds.Count > 0)
            {
                var soundIndex = Random.Range(0, _brickTheme.DamageSounds.Count);
                _audioController.PlaySfx(_brickTheme.DamageSounds[soundIndex]);
            }
        }

        protected override void OnDied()
        {
            gameObject.SetActive(false);
            if (_brickTheme?.FatalDamageSounds.Count > 0)
            {
                var soundIndex = Random.Range(0, _brickTheme.FatalDamageSounds.Count);
                _audioController.PlaySfx(_brickTheme.FatalDamageSounds[soundIndex]);
            }
        }

        [SerializeField]
        private SpriteRenderer _brickImage;

        private IBrickTheme _brickTheme;
        private IAudioController _audioController;

        private void OnBrickThemeChanged()
        {
            _brickImage.sprite = _brickTheme.Icon;
        }
    }
}
