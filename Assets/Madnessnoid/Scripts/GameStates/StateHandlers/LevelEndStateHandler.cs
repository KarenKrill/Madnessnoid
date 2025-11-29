#nullable enable

using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;
using KarenKrill.UniCore.StateSystem.Abstractions;

namespace Madnessnoid.GameStates
{
    using Abstractions;
    using UI.Presenters.Abstractions;

    public class LevelEndStateHandler : PresentableStateHandlerBase<GameState>, IStateHandler<GameState>
    {
        public override GameState State => GameState.LevelEnd;

        public LevelEndStateHandler(ILogger logger,
            IGameFlow gameFlow,
            IPlayerSession playerSession,
            ILevelSession levelSession,
            IGameConfig gameConfig,
            IActionsProvider<ActionMap> actionsProvider,
            ILevelEndMenuPresenter gameEndMenuPresenter) : base(gameEndMenuPresenter)
        {
            _logger = logger;
            _gameFlow = gameFlow;
            _levelSession = levelSession;
            _playerSession = playerSession;
            _gameConfig = gameConfig;
            _levelEndMenuPresenter = gameEndMenuPresenter;
            _actionsProvider = actionsProvider;
        }

        public override void Enter(GameState prevState, object? context = null)
        {
            _levelEndMenuPresenter.Continue += OnContinue;
            _levelEndMenuPresenter.Restart += OnRestart;
            _levelEndMenuPresenter.MainMenu += OnMainMenu;
            _levelEndMenuPresenter.Exit += OnExit;
            base.Enter(prevState);
            _actionsProvider.SetActionMap(ActionMap.UI);
            if (_levelSession.LevelState == LevelState.Won)
            {
                var levelConfig = _gameConfig.LevelsConfig[_levelSession.LevelId];
                var cashReward = levelConfig.BaseCashReward + _levelSession.HitPointsCount * levelConfig.HitPointCashRewardBonus;
                _playerSession.AddMoney(cashReward);
            }
            _logger.Log(nameof(LevelEndStateHandler), nameof(Enter));
        }

        public override void Exit(GameState nextState)
        {
            _levelEndMenuPresenter.Continue -= OnContinue;
            _levelEndMenuPresenter.Restart -= OnRestart;
            _levelEndMenuPresenter.MainMenu -= OnMainMenu;
            _levelEndMenuPresenter.Exit -= OnExit;
            base.Exit(nextState);
            _logger.Log(nameof(LevelEndStateHandler), nameof(Exit));
        }

        private readonly ILogger _logger;
        private readonly IGameFlow _gameFlow;
        private readonly IPlayerSession _playerSession;
        private readonly ILevelSession _levelSession;
        private readonly IGameConfig _gameConfig;
        private readonly IActionsProvider<ActionMap> _actionsProvider;
        private readonly ILevelEndMenuPresenter _levelEndMenuPresenter;

        private void OnContinue()
        {
            if (_levelSession.LevelState == LevelState.Won)
            {
                var nextLevelId = _levelSession.LevelId + 1;
                if (nextLevelId < _gameConfig.LevelsConfig.Count)
                {
                    _gameFlow.StartLevel(nextLevelId);
                }
            }
        }

        private void OnRestart()
        {
            _gameFlow.StartLevel(_levelSession.LevelId);
        }

        private void OnMainMenu() => _gameFlow.LoadMainMenu();

        private void OnExit() => _gameFlow.Exit();
    }
}
