#nullable enable

using UnityEngine;

using KarenKrill.UniCore.StateSystem.Abstractions;
using KarenKrill.UniCore.UI.Presenters.Abstractions;

using Madnessnoid.Abstractions;
using Madnessnoid.UI.Views.Abstractions;

namespace Madnessnoid.GameStates
{
    public class InitialStateHandler : IStateHandler<GameState>
    {
        public GameState State => GameState.Initial;

        public InitialStateHandler(ILogger logger,
            IGameFlow gameFlow,
            GameSettings gameSettings,
            IAudioController audioController,
            IPresenter<IDiagnosticsView> diagnosticsPresenter)
        {
            _logger = logger;
            _gameFlow = gameFlow;
            _gameSettings = gameSettings;
            _audioController = audioController;
            _diagnosticsPresenter = diagnosticsPresenter;
        }

        public void Enter(GameState prevState, object? context = null)
        {
            _logger.Log(nameof(InitialStateHandler), nameof(Enter));
            _gameSettings.ShowFpsChanged += OnShowFpsChanged;
            _gameSettings.MusicVolumeChanged += OnMusicVolumeChanged;
            _gameSettings.QualityLevelChanged += OnQualityLevelChanged;
            if (_gameSettings.ShowFps)
            {
                _diagnosticsPresenter.Enable();
            }
            _audioController.MasterVolume = _gameSettings.MusicVolume;
            _gameFlow.LoadMainMenu();
        }

        public void Exit(GameState nextState)
        {
            _logger.Log(nameof(InitialStateHandler), nameof(Exit));
        }

        private readonly ILogger _logger;
        private readonly IGameFlow _gameFlow;
        private readonly GameSettings _gameSettings;
        private readonly IAudioController _audioController;
        private readonly IPresenter<IDiagnosticsView> _diagnosticsPresenter;

        private void OnShowFpsChanged(bool state)
        {
            if (state)
            {
                _diagnosticsPresenter.Enable();
            }
            else
            {
                _diagnosticsPresenter.Disable();
            }
        }

        private void OnMusicVolumeChanged(float value)
        {
            _audioController.MasterVolume = value;
        }

        private void OnQualityLevelChanged(Abstractions.QualityLevel qualityLevel)
        {
        }
    }
}