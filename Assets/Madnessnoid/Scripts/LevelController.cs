using UnityEngine;

using Zenject;

namespace Madnessnoid
{
    using Abstractions;

    public class LevelController : MonoBehaviour
    {
        [Inject]
        public void Initialize(ILevelSession levelSession, IThemeProfileProvider themeProfileProvider)
        {
            _levelSession = levelSession;
            _themeProfileProvider = themeProfileProvider;
        }

        [SerializeField]
        private SpriteRenderer _levelBackground;

        private ILevelSession _levelSession;
        private IThemeProfileProvider _themeProfileProvider;

        private void OnEnable()
        {
            _themeProfileProvider.ActiveThemeChanged += OnActiveThemeChanged;
            _levelSession.LevelChanged += OnLevelChanged;
        }

        private void OnDisable()
        {
            _themeProfileProvider.ActiveThemeChanged -= OnActiveThemeChanged;
            _levelSession.LevelChanged -= OnLevelChanged;
        }

        private void Start() => UpdateBackground();

        private void UpdateBackground()
        {
            if (_levelSession.LevelId >= 0)
            {
                var background = _themeProfileProvider.ActiveTheme.LevelsBackground[_levelSession.LevelId].Image;
                _levelBackground.sprite = background;
            }
        }

        private void OnActiveThemeChanged() => UpdateBackground();

        private void OnLevelChanged(int levelId) => UpdateBackground();

    }
}
