using System.Collections.Generic;

using UnityEngine;

namespace Madnessnoid.Abstractions
{
    public interface IThemeProfile
    {
        public ISceneBackgroundTheme MainMenuBackground { get; }
        public List<ILevelTheme> LevelThemes { get; }
        public Sprite HitPointIcon { get; }
        public Sprite ScoreIcon { get; }
        public Sprite MoneyIcon { get; }
    }

    public interface ISceneBackgroundTheme
    {
        public Sprite Image { get;}
        public AudioClip Music { get; }
    }

    public interface ILevelTheme
    {
        public ISceneBackgroundTheme Background { get; }
        public Sprite BallIcon { get; }
        public List<AudioClip> WallCollisionSounds { get; }
        public List<AudioClip> LosingHitPointSounds { get; }
        public List<IBrickTheme> BrickThemes { get; }
    }

    public interface IBrickTheme
    {
        public Sprite Icon { get; }
        public List<AudioClip> DamageSounds { get; }
        public List<AudioClip> FatalDamageSounds { get; }
        public List<AnimationClip> DamageAnimations { get; }
        public List<AnimationClip> FatalDamageAnimations { get; }
    }
}
