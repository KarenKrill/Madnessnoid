#nullable enable

namespace Madnessnoid.Abstractions
{
    public delegate void LevelCompletedHandler(LevelCompletionResult result);
    public delegate void HitPointsCountChangedHandler(int hitPointsCount);
    public delegate void LevelChangedHandler(int levelId);
    public delegate void LevelScoreChangedHandler(int score);

    public enum LevelState
    {
        None = 0,
        Played,
        Won,
        Lost
    }

    public interface ILevelSession
    {
        public int LevelId { get; }
        public int HitPointsCount { get; }
        public int LevelScore { get; }
        public LevelState LevelState { get; }

        public event LevelChangedHandler? LevelChanged;
        public event HitPointsCountChangedHandler? HitPointsCountChanged;
        public event LevelScoreChangedHandler? LevelScoreChanged;
        public event LevelCompletedHandler? LevelCompleted;

        public void SetLevel(int levelId);
        public void BreakTheBlock(int blockId);
        public void TakeDamage();
    }
}
