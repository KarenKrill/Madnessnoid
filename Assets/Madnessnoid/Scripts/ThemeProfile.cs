using System;
using System.Collections.Generic;

using UnityEngine;

namespace Madnessnoid
{
    using Abstractions;

    [CreateAssetMenu(fileName = nameof(ThemeProfile), menuName = "Scriptable Objects/" + nameof(ThemeProfile))]
    public class ThemeProfile : ScriptableObject, IThemeProfile
    {
        public ISceneBackgroundTheme MainMenuBackground => _mainMenuBackground;

        public List<ISceneBackgroundTheme> LevelsBackground { get; } = new();

        [field: SerializeField]
        public Sprite HitPointIcon { get; private set; }

        [field: SerializeField]
        public Sprite ScoreIcon { get; private set; }

        [field: SerializeField]
        public Sprite MoneyIcon { get; private set; }

        [SerializeField]
        private SceneBackgroundTheme _mainMenuBackground;
        [SerializeField]
        private List<SceneBackgroundTheme> _levelsBackground;

#if !UNITY_EDITOR
        private void Awake() => OnValidate();
#endif

        private void OnValidate()
        {
            LevelsBackground.Clear();
            foreach (var level in _levelsBackground)
            {
                LevelsBackground.Add(level);
            }
        }
    }
    [Serializable]
    public class SceneBackgroundTheme : ISceneBackgroundTheme
    {
        [field: SerializeField]
        public Sprite Image { get; private set; }

        [field: SerializeField]
        public AudioClip Music { get; private set; }
    }
}