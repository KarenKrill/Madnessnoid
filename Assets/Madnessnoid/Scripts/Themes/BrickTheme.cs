using System.Collections.Generic;

using UnityEngine;

namespace Madnessnoid
{
    using Abstractions;

    [CreateAssetMenu(fileName = nameof(BrickTheme), menuName = "Scriptable Objects/" + nameof(BrickTheme))]
    public class BrickTheme : ScriptableObject, IBrickTheme
    {
        [field: SerializeField]
        public Sprite Sprite { get; private set; }

        [field: SerializeField]
        public List<AudioClip> DamageSounds { get; private set; } = new();

        [field: SerializeField]
        public List<AudioClip> FatalDamageSounds { get; private set; } = new();

        [field: SerializeField]
        public List<AnimationClip> DamageAnimations { get; private set; } = new();

        [field: SerializeField]
        public List<AnimationClip> FatalDamageAnimations { get; private set; } = new();
    }
}