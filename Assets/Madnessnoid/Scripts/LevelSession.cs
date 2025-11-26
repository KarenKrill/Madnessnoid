using System;

namespace Madnessnoid
{
    using Abstractions;
    using UnityEngine;

    public class LevelSession : ILevelSession
    {
        public int LevelId { get; private set; } = -1;
        public int HitPointsCount { get; private set; }
        public int LevelScore { get; private set; }
        public LevelState LevelState { get; private set; }

        public event LevelChangedHandler LevelChanged;
        public event HitPointsCountChangedHandler HitPointsCountChanged;
        public event LevelScoreChangedHandler LevelScoreChanged;
        public event LevelCompletedHandler LevelCompleted;

        public LevelSession(IGameConfig gameConfig)
        {
            _gameConfig = gameConfig;
        }

        public void SetLevel(int levelId)
        {
            if (LevelId == levelId)
            {
                return;
            }
            if (levelId < _gameConfig.LevelsConfig.Count)
            {
                _levelConfig = _gameConfig.LevelsConfig[levelId];
                _remainedBlocksCount = _levelConfig.BlocksCount;
                _blockBreakCashReward = _levelConfig.BaseCashReward / _levelConfig.BlocksCount;
                LevelId = levelId;
                LevelScore = 0;
                HitPointsCount = _levelConfig.HitPointsCount;
                LevelState = LevelState.Played;
                try
                {
                    LevelScoreChanged?.Invoke(LevelScore);
                    HitPointsCountChanged?.Invoke(HitPointsCount);
                }
                finally
                {
                    LevelChanged?.Invoke(LevelId);
                }
            }
            else
            {
                throw new ArgumentException(nameof(levelId), $"LevelsConfigCount({_gameConfig.LevelsConfig.Count}) < LevelId({levelId})");
            }
        }
        public void BreakTheBlock(int blockId)
        {
            _remainedBlocksCount--;
            _levelCashReward += _blockBreakCashReward;
            LevelScore = CalcLevelScore();
            bool isLevelWon = _remainedBlocksCount <= 0;
            if (isLevelWon)
            {
                LevelState = LevelState.Won;
            }
            try
            {
                LevelScoreChanged?.Invoke(LevelScore);
            }
            finally
            {
                if (isLevelWon)
                {
                    LevelCompleted?.Invoke(new(HitPointsCount));
                }
            }
        }
        public void TakeDamage()
        {
            if (HitPointsCount > 0)
            {
                HitPointsCount--;
                bool isLevelLost = HitPointsCount == 0;
                if (isLevelLost)
                {
                    LevelState = LevelState.Lost;
                }
                try
                {
                    HitPointsCountChanged?.Invoke(HitPointsCount);
                }
                finally
                {
                    if(isLevelLost)
                    {
                        LevelCompleted?.Invoke(new(HitPointsCount));
                    }
                }
            }
        }

        private readonly IGameConfig _gameConfig;
        private ILevelConfig _levelConfig;
        private int _remainedBlocksCount = 0;
        private float _blockBreakCashReward = 0;
        private float _levelCashReward = 0;

        private int CalcLevelScore()
        {
            if (_remainedBlocksCount > 0)
            {
                return Mathf.CeilToInt(_levelCashReward);
            }
            else
            {
                return _levelConfig.BaseCashReward + HitPointsCount * _levelConfig.HitPointCashRewardBonus;
            }
        }
    }
}
