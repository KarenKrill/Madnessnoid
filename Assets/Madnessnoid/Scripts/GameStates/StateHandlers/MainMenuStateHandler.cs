#nullable enable

using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;
using KarenKrill.UniCore.StateSystem.Abstractions;

namespace Madnessnoid.GameStates
{
    using Abstractions;
    using UI.Presenters.Abstractions;

    public class MainMenuStateHandler : PresentableStateHandlerBase<GameState>, IStateHandler<GameState>
    {
        public override GameState State => GameState.MainMenu;

        public MainMenuStateHandler(ILogger logger,
            IGameFlow gameFlow,
            IActionsProvider<ActionMap> actionsProvider,
            IMainMenuPresenter mainMenuPresenter,
            IThemeProfileProvider themeProfileProvider,
            IAudioController audioController) : base(mainMenuPresenter)
        {
            _logger = logger;
            _gameFlow = gameFlow;
            _mainMenuPresenter = mainMenuPresenter;
            _actionsProvider = actionsProvider;
            _themeProfileProvider = themeProfileProvider;
            _audioController = audioController;
        }

        public override void Enter(GameState prevState, object? context = null)
        {
            _mainMenuPresenter.NewGame += OnNewGame;
            _mainMenuPresenter.Exit += OnExit;
            base.Enter(prevState);
            _actionsProvider.SetActionMap(ActionMap.UI);
            _themeProfileProvider.ActiveThemeChanged += OnActiveThemeChanged;
            OnActiveThemeChanged();
            _logger.Log($"{nameof(MainMenuStateHandler)}.{nameof(Enter)}()");
        }

        public override void Exit(GameState nextState)
        {
            base.Exit(nextState);
            _mainMenuPresenter.NewGame -= OnNewGame;
            _mainMenuPresenter.Exit -= OnExit;
            _themeProfileProvider.ActiveThemeChanged -= OnActiveThemeChanged;
            _audioController.StopMusic();
            _logger.Log($"{nameof(MainMenuStateHandler)}.{nameof(Exit)}()");
        }
        
        private readonly ILogger _logger;
        private readonly IGameFlow _gameFlow;
        private readonly IActionsProvider<ActionMap> _actionsProvider;
        private readonly IMainMenuPresenter _mainMenuPresenter;
        private readonly IThemeProfileProvider _themeProfileProvider;
        private readonly IAudioController _audioController;

        private void OnExit()
        {
            _gameFlow.Exit();
        }

        private void OnNewGame()
        {
            _gameFlow.StartLevel(0);
        }

        private void OnActiveThemeChanged()
        {
            var activeTheme = _themeProfileProvider.ActiveTheme;
            _audioController.PlayMusic(activeTheme.MainMenuBackground.Music);
        }
    }
}