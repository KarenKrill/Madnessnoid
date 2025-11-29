#nullable enable

using UnityEngine;

using KarenKrill.UniCore.StateSystem.Abstractions;

using Madnessnoid.Abstractions;

namespace Madnessnoid.GameStates
{
    public class ExitStateHandler : IStateHandler<GameState>
    {
        public GameState State => GameState.Exit;

        public ExitStateHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void Enter(GameState prevState, object? context = null)
        {
            _logger.Log(nameof(ExitStateHandler), nameof(Enter));
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#elif UNITY_WEBGL
            _logger.LogError(nameof(ExitStateHandler), "Exiting isn't supported by the platform");
#else
            Application.Quit();
#endif
        }

        public void Exit(GameState nextState) { }

        private readonly ILogger _logger;
    }
}