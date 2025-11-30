using KarenKrill.UniCore.StateSystem.Abstractions;

using Madnessnoid.Abstractions;

namespace Madnessnoid.GameStates
{
    public class GameFlow : IGameFlow
    {
        public GameState State => _stateSwitcher.State;

        public GameFlow(IStateSwitcher<GameState> stateSwitcher)
        {
            _stateSwitcher = stateSwitcher;
        }

        public void LoadMainMenu()
        {
            _stateSwitcher.TransitTo(GameState.Loading, new LoadingStateContext());
        }

        public void StartLevel(int levelId)
        {
            _stateSwitcher.TransitTo(GameState.Loading, new LoadingStateContext(levelId));
        }

        public void PauseLevel()
        {
            _stateSwitcher.TransitTo(GameState.Pause);
        }

        public void ResumeLevel()
        {
            _stateSwitcher.TransitTo(GameState.Gameplay);
        }

        public void FinishLevel()
        {
            _stateSwitcher.TransitTo(GameState.LevelEnd);
        }

        public void Exit()
        {
            _stateSwitcher.TransitTo(GameState.Exit);
        }

        private readonly IStateSwitcher<GameState> _stateSwitcher;
    }
}
