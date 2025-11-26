namespace Madnessnoid.Abstractions
{
    public interface IGameFlow
    {
        GameState State { get; }

        void LoadMainMenu();
        void StartLevel(int levelId);
        void PauseLevel();
        void ResumeLevel();
        void FinishLevel();
        void Exit();
    }
}
