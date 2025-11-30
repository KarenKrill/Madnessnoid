using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views.Abstractions;

using Madnessnoid.Abstractions;

namespace Madnessnoid.UI.Presenters
{
    using Abstractions;
    using Views.Abstractions;

    public class MainMenuPresenter : PresenterBase<IMainMenuView>, IMainMenuPresenter, IPresenter<IMainMenuView>
    {
        public event Action NewGame;
        public event Action Exit;

        public MainMenuPresenter(IViewFactory viewFactory,
            IPresenterNavigator navigator,
            GameSettings gameSettings,
            IPlayerSession playerSession,
            IThemeProfileProvider themeProfileProvider) : base(viewFactory, navigator)
        {
            _settingsPresenter = new SettingsMenuPresenter(viewFactory, navigator, gameSettings);
            _playerSession = playerSession;
            _themeProfileProvider = themeProfileProvider;
        }

        protected override void Subscribe()
        {
            View.NewGameRequested += OnNewGame;
            View.SettingsOpenRequested += OnSettings;
            View.ExitRequested += OnExit;
            _playerSession.MoneyChanged += OnMoneyChanged;
            _themeProfileProvider.ActiveThemeChanged += OnActiveThemeChanged;
            OnMoneyChanged();
            OnActiveThemeChanged();
        }
        protected override void Unsubscribe()
        {
            View.NewGameRequested -= OnNewGame;
            View.SettingsOpenRequested -= OnSettings;
            View.ExitRequested -= OnExit;
            _playerSession.MoneyChanged -= OnMoneyChanged;
        }

        private readonly ISettingsMenuPresenter _settingsPresenter;
        private readonly IPlayerSession _playerSession;
        private readonly IThemeProfileProvider _themeProfileProvider;

        private void OnNewGame() => NewGame?.Invoke();
        private void OnSettings()
        {
            View.Interactable = false;
            View.SetFocus(false);
            _settingsPresenter.Close += OnSettingsClose;
            Navigator.Push(_settingsPresenter);
        }
        private void OnSettingsClose()
        {
            _settingsPresenter.Close -= OnSettingsClose;
            Navigator.Pop();
            View.Interactable = true;
            View.SetFocus(true);
        }
        private void OnExit() => Exit?.Invoke();
        private void OnMoneyChanged()
        {
            View.MoneyText = _playerSession.Money.ToString();
        }
        private void OnActiveThemeChanged()
        {
            View.MoneyIcon = _themeProfileProvider.ActiveTheme.MoneyIcon;
        }
    }
}