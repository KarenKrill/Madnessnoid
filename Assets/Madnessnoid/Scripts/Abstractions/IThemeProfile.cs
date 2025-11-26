using System.Collections.Generic;

using UnityEngine;

namespace Madnessnoid.Abstractions
{
    public interface IThemeProfile
    {
        public ISceneBackgroundTheme MainMenuBackground { get; }
        public List<ISceneBackgroundTheme> LevelsBackground { get; }
        public Sprite HitPointIcon { get; }
        public Sprite ScoreIcon { get; }
        public Sprite MoneyIcon { get; }
    }

    public interface ISceneBackgroundTheme
    {
        public Sprite Image { get;}
        public AudioClip Music { get; }
    }
}
