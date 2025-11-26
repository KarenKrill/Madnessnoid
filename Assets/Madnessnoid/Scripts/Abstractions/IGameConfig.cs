using System.Collections.Generic;

namespace Madnessnoid.Abstractions
{
    public interface IGameConfig
    {
        public IReadOnlyList<ILevelConfig> LevelsConfig { get; }
    }

    public interface ILevelConfig
    {
        public int BlocksCount { get; }
        public int HitPointsCount { get; }
        public int BallVelocity { get; }
        public int BallAngularVelocity { get; }
        public int BaseCashReward { get; }
        public int HitPointCashRewardBonus { get; }
    }
}
