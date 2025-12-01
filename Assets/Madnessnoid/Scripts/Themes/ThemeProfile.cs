using System.Collections.Generic;

using UnityEngine;

namespace Madnessnoid
{
    using Abstractions;

    [CreateAssetMenu(fileName = nameof(ThemeProfile), menuName = "Scriptable Objects/" + nameof(ThemeProfile))]
    public class ThemeProfile : ScriptableObject, IThemeProfile
    {
        public ISceneBackgroundTheme MainMenuBackground => _mainMenuBackground;

        public List<ILevelTheme> LevelThemes { get; } = new();

        [field: SerializeField]
        public Sprite HitPointIcon { get; private set; }

        [field: SerializeField]
        public Sprite ScoreIcon { get; private set; }

        [field: SerializeField]
        public Sprite MoneyIcon { get; private set; }

        [SerializeField]
        private SceneBackgroundTheme _mainMenuBackground;

        [SerializeField]
        private List<LevelTheme> _levelThemes = new();

#if !UNITY_EDITOR
        private void Awake() => OnValidate();
#endif

        private void OnValidate()
        {
            LevelThemes.Clear();
            foreach (var level in _levelThemes)
            {
                LevelThemes.Add(level);
            }
        }
    }
}