using System;
using System.Collections.Generic;

using UnityEngine;

namespace Madnessnoid
{
    using Abstractions;

    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "Scriptable Objects/" + nameof(GameConfig))]
    public class GameConfig : ScriptableObject, IGameConfig
    {
        public IReadOnlyList<ILevelConfig> LevelsConfig => _levelsConfigList;

        [SerializeField]
        private List<LevelConfig> _levelsConfig;
        private readonly List<ILevelConfig> _levelsConfigList = new();

        private void OnValidate()
        {
            _levelsConfigList.Clear();
            foreach (var level in _levelsConfig)
            {
                _levelsConfigList.Add(level);
            }
        }
    }

    [Serializable]
    public class LevelConfig : ILevelConfig
    {
        [field: SerializeField]
        public int BlocksCount { get; private set; }

        [field: SerializeField]
        public int HitPointsCount { get; private set; }

        [field: SerializeField]
        public int BaseCashReward { get; private set; }

        [field: SerializeField]
        public int HitPointCashRewardBonus { get; private set; }
    }
}