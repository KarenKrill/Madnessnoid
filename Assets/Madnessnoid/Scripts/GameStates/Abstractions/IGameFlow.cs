namespace Madnessnoid.Abstractions
{
    public interface IGameFlow
    {
        GameState State { get; }

        void LoadMainMenu();
        void StartLevel(int index);
        void PauseLevel();
        void ResumeLevel();
        void Exit();
    }
}
