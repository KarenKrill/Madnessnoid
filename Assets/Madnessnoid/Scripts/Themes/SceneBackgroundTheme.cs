using System;

using UnityEngine;

namespace Madnessnoid
{
    using Abstractions;

    [Serializable]
    public class SceneBackgroundTheme : ISceneBackgroundTheme
    {
        [field: SerializeField]
        public Sprite Image { get; private set; }

        [field: SerializeField]
        public AudioClip Music { get; private set; }
    }
}