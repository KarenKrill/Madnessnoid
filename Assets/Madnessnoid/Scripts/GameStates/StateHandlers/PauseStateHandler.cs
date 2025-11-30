#nullable enable

using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;
using KarenKrill.UniCore.StateSystem.Abstractions;

using Madnessnoid.Abstractions;
using Madnessnoid.Input.Abstractions;
using Madnessnoid.UI.Presenters.Abstractions;

namespace Madnessnoid.GameStates
{
    public class PauseStateHandler : PresentableStateHandlerBase<GameState>, IStateHandler<GameState>
    {
        public override GameState State => GameState.Pause;

        public PauseStateHandler(ILogger logger,
            IGameFlow gameFlow,
            IStateSwitcher<GameState> stateSwitcher,
            ILevelSession levelSession,
            IBasicActionsProvider actionsProvider,
            IUIActionsProvider uiActionsProvider,
            IPauseMenuPresenter pauseMenuPresenter)
            : base(pauseMenuPresenter)
        {
            _logger = logger;
            _gameFlow = gameFlow;
            _levelSession = levelSession;
            _stateSwitcher = stateSwitcher;
            _actionsProvider = actionsProvider;
            _uiActionsProvider = uiActionsProvider;
            _pauseMenuPresenter = pauseMenuPresenter;

        }
        public override void Enter(GameState prevState, object? context = null)
        {
            base.Enter(prevState);
            _previousState = prevState;
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            _logger.Log(nameof(PauseStateHandler), nameof(Enter));
            _uiActionsProvider.Cancel += OnResumeRequested;
            _pauseMenuPresenter.Resume += OnResumeRequested;
            _pauseMenuPresenter.Restart += OnRestartRequested;
            _pauseMenuPresenter.MainMenu += OnMainMenu;
            _pauseMenuPresenter.Exit += OnExit;
            _actionsProvider.SetActionMap(ActionMap.UI);
        }
        public override void Exit(GameState nextState)
        {
            _logger.Log(nameof(PauseStateHandler), nameof(Exit));
            _uiActionsProvider.Cancel -= OnResumeRequested;
            _pauseMenuPresenter.Resume -= OnResumeRequested;
            _pauseMenuPresenter.Restart -= OnRestartRequested;
            _pauseMenuPresenter.MainMenu -= OnMainMenu;
            _pauseMenuPresenter.Exit -= OnExit;
            Time.timeScale = _previousTimeScale;
            base.Exit(nextState);
        }

        private readonly ILogger _logger;
        private readonly IGameFlow _gameFlow;
        private readonly ILevelSession _levelSession;
        private readonly IStateSwitcher<GameState> _stateSwitcher;
        private GameState _previousState;
        private readonly IBasicActionsProvider _actionsProvider;
        private readonly IUIActionsProvider _uiActionsProvider;
        private readonly IPauseMenuPresenter _pauseMenuPresenter;
        private float _previousTimeScale;

        private void OnResumeRequested()
        {
            _stateSwitcher.TransitTo(_previousState);
        }
        private void OnRestartRequested()
        {
            _gameFlow.StartLevel(_levelSession.LevelId);
        }
        private void OnMainMenu() => _gameFlow.LoadMainMenu();
        private void OnExit() => _gameFlow.Exit();

    }
}
