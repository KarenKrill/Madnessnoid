namespace Madnessnoid.GameStates
{
    public class GameplayStateContext
    {
        /// <summary>
        /// Whether game is resuming or starts at first time
        /// </summary>
        public bool IsResuming;
        public int LevelIndex { get; }

        public GameplayStateContext(bool isResuming, int levelIndex)
        {
            IsResuming = isResuming;
            LevelIndex = levelIndex;
        }
    }
}