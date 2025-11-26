using System;

using UnityEngine;

using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views.Abstractions;

using Madnessnoid.Abstractions;

namespace Madnessnoid.UI.Presenters
{
    using Abstractions;
    using Views.Abstractions;

    public class LevelEndMenuPresenter : PresenterBase<ILevelEndMenuView>, ILevelEndMenuPresenter, IPresenter<ILevelEndMenuView>
    {
        public event Action Continue;
        public event Action Restart;
        public event Action MainMenu;
        public event Action Exit;

        public LevelEndMenuPresenter(IViewFactory viewFactory,
            IPresenterNavigator navigator,
            IGameConfig gameConfig,
            ILevelSession levelSession,
            IThemeProfileProvider themeProfileProvider) : base(viewFactory, navigator)
        {
            _gameConfig = gameConfig;
            _levelSession = levelSession;
            _themeProfileProvider = themeProfileProvider;
        }

        protected override void Subscribe()
        {
            View.ContinueRequested += OnContinueRequested;
            View.RestartRequested += OnRestartRequested;
            View.MainMenuExitRequested += OnMainMenuExitRequested;
            View.ExitRequested += OnExitRequested;
            _themeProfileProvider.ActiveThemeChanged += OnActiveThemeChanged;
            UpdateView();
        }
        protected override void Unsubscribe()
        {
            View.ContinueRequested -= OnContinueRequested;
            View.RestartRequested -= OnRestartRequested;
            View.MainMenuExitRequested -= OnMainMenuExitRequested;
            View.ExitRequested -= OnExitRequested;
            _themeProfileProvider.ActiveThemeChanged -= OnActiveThemeChanged;
        }

        private static readonly string _gameWinText = "You are a winner!";
        private static readonly string _levelWinText = "Level completed!";
        private static readonly string _levelLoseText = "You are a loser!";

        private readonly IGameConfig _gameConfig;
        private readonly ILevelSession _levelSession;
        private readonly IThemeProfileProvider _themeProfileProvider;

        private void UpdateView()
        {
            if (_levelSession.LevelState == LevelState.Won)
            {
                View.TitleTextColor = new Color(1, (float)0xAC / 0xFF, (float)0x40 / 0xFF, 1);
                if (_levelSession.LevelId < _gameConfig.LevelsConfig.Count - 1)
                {
                    View.TitleText = _levelWinText;
                    View.EnableContinue = true;
                }
                else
                {
                    View.TitleText = _gameWinText;
                    View.EnableContinue = false;
                }
                View.EnableReward = true;
                View.CashRewardText = $"Reward: +{_levelSession.LevelScore}";
                OnActiveThemeChanged();
            }
            else
            {
                View.TitleText = _levelLoseText;
                View.TitleTextColor = new Color((float)0xE1 / 0xFF, (float)0x24 / 0xFF, 0);
                View.EnableContinue = false;
                View.EnableReward = false;
            }
        }
        private void OnContinueRequested() => Continue?.Invoke();
        private void OnRestartRequested() => Restart?.Invoke();
        private void OnMainMenuExitRequested() => MainMenu?.Invoke();
        private void OnExitRequested() => Exit?.Invoke();
        private void OnActiveThemeChanged()
        {
            View.CashRewardIcon = _themeProfileProvider.ActiveTheme.MoneyIcon;
        }

    }
}