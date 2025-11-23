using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views.Abstractions;

namespace Madnessnoid.UI.Presenters
{
    using Abstractions;
    using Madnessnoid.Abstractions;
    using Views.Abstractions;

    public class MainMenuPresenter : PresenterBase<IMainMenuView>, IMainMenuPresenter, IPresenter<IMainMenuView>
    {
        public event Action NewGame;
        public event Action Exit;

        public MainMenuPresenter(IViewFactory viewFactory,
            IPresenterNavigator navigator,
            GameSettings gameSettings) : base(viewFactory, navigator)
        {
            _settingsPresenter = new SettingsMenuPresenter(viewFactory, navigator, gameSettings);
        }

        protected override void Subscribe()
        {
            View.NewGameRequested += OnNewGame;
            View.SettingsOpenRequested += OnSettings;
            View.ExitRequested += OnExit;
        }
        protected override void Unsubscribe()
        {
            View.NewGameRequested -= OnNewGame;
            View.SettingsOpenRequested -= OnSettings;
            View.ExitRequested -= OnExit;
        }

        private readonly ISettingsMenuPresenter _settingsPresenter;

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
    }
}