using System.Collections.Generic;

using UnityEngine;

namespace Madnessnoid
{
    using Abstractions;

    [CreateAssetMenu(fileName = nameof(LevelTheme), menuName = "Scriptable Objects/" + nameof(LevelTheme))]
    public class LevelTheme : ScriptableObject, ILevelTheme
    {
        public ISceneBackgroundTheme Background => _background;

        [field: SerializeField]
        public Sprite BallSprite { get; private set; }
        
        [field: SerializeField]
        public Sprite PaddleSprite { get; private set; }

        [field: SerializeField]
        public List<AudioClip> WallCollisionSounds { get; private set; } = new();

        [field: SerializeField]
        public List<AudioClip> LosingHitPointSounds { get; private set; } = new();

        public List<IBrickTheme> BrickThemes { get; } = new();

        [SerializeField]
        private SceneBackgroundTheme _background;

        [SerializeField]
        private List<BrickTheme> _brickThemes = new();

#if !UNITY_EDITOR
        private void Awake() => OnValidate();
#endif

        private void OnValidate()
        {
            BrickThemes.Clear();
            foreach (var level in _brickThemes)
            {
                BrickThemes.Add(level);
            }
        }
    }
}