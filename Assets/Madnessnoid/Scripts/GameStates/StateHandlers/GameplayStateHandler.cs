#nullable enable

using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;
using KarenKrill.UniCore.StateSystem.Abstractions;

using Madnessnoid.Abstractions;
using Madnessnoid.Input.Abstractions;
using Madnessnoid.UI.Presenters.Abstractions;

namespace Madnessnoid.GameStates
{
    public class GameplayStateHandler : PresentableStateHandlerBase<GameState>, IStateHandler<GameState>
    {
        public override GameState State => GameState.Gameplay;

        public GameplayStateHandler(ILogger logger,
            IGameFlow gameFlow,
            IBasicActionsProvider actionsProvider,
            IPlayerActionsProvider playerActionsProvider,
            IInGameMenuPresenter inGameMenuPresenter,
            IThemeProfileProvider themeProfileProvider,
            IAudioController audioController)
            : base(inGameMenuPresenter)
        {
            _logger = logger;
            _gameFlow = gameFlow;
            _actionsProvider = actionsProvider;
            _playerActionsProvider = playerActionsProvider;
            _inGameMenuPresenter = inGameMenuPresenter;
            _themeProfileProvider = themeProfileProvider;
            _audioController = audioController;
        }
        public override void Enter(GameState prevState, object? context = null)
        {
            base.Enter(prevState);

            if (context is GameplayStateContext gameplayStateContext)
            {
                _context = gameplayStateContext;
            }
            _playerActionsProvider.Pause += OnPause;
            _inGameMenuPresenter.Pause += OnPause;
            _themeProfileProvider.ActiveThemeChanged += OnActiveThemeChanged;
            if (prevState == GameState.Pause && _currThemeProfile == _themeProfileProvider.ActiveTheme)
            {
                _audioController.ResumeMusic();
            }
            else
            {
                OnActiveThemeChanged();
            }
            _actionsProvider.SetActionMap(ActionMap.Player);
            _logger.Log(nameof(GameplayStateHandler), nameof(Enter));
        }
        public override void Exit(GameState nextState)
        {
            base.Exit(nextState);

            _context = null;
            _playerActionsProvider.Pause -= OnPause;
            _inGameMenuPresenter.Pause -= OnPause;
            _themeProfileProvider.ActiveThemeChanged -= OnActiveThemeChanged;
            if (nextState == GameState.Pause)
            {
                _audioController.PauseMusic();
            }
            _actionsProvider.SetActionMap(ActionMap.UI);
            _logger.Log(nameof(GameplayStateHandler), nameof(Exit));
        }

        private readonly ILogger _logger;
        private readonly IGameFlow _gameFlow;
        private GameplayStateContext? _context;

        private readonly IBasicActionsProvider _actionsProvider;
        private readonly IPlayerActionsProvider _playerActionsProvider;
        private readonly IInGameMenuPresenter _inGameMenuPresenter;
        private readonly IThemeProfileProvider _themeProfileProvider;
        private readonly IAudioController _audioController;
        private IThemeProfile? _currThemeProfile = null;

        private void OnPause()
        {
            _gameFlow.PauseLevel();
        }
        private void OnActiveThemeChanged()
        {
            _currThemeProfile = _themeProfileProvider.ActiveTheme;
            int levelBackgroundIndex = _currThemeProfile.LevelsBackground.Count - 1;
            if (_context?.LevelIndex < levelBackgroundIndex)
            {
                levelBackgroundIndex = _context.LevelIndex;
            }
            if (levelBackgroundIndex >= 0)
            {
                var levelBackground = _currThemeProfile.LevelsBackground[levelBackgroundIndex];
                _audioController.PlayMusic(levelBackground.Music);
            }
        }
    }
}