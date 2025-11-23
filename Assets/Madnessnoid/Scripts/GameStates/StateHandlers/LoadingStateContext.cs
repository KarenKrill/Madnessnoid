namespace Madnessnoid.GameStates
{
    public class LoadingStateContext
    {
        /// <remarks>-1 to load main menu</remarks>
        public int LevelIndex { get; }

        /// <param name="levelIndex">Specify -1 to load main menu (default value)</param>
        public LoadingStateContext(int levelIndex = -1)
        {
            LevelIndex = levelIndex;
        }
    }
}