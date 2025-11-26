namespace Madnessnoid.Abstractions
{
    public readonly struct LevelCompletionResult
    {
        public int RemainingHitPointsCount { get; }
        public bool IsWon => RemainingHitPointsCount > 0;
        public bool IsLost => RemainingHitPointsCount <= 0;

        public LevelCompletionResult(int remainingHitPointsCount = 0)
        {
            RemainingHitPointsCount = remainingHitPointsCount;
        }
    }
}
