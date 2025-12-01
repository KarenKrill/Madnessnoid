#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views.Abstractions;

using Madnessnoid.Abstractions;
using Madnessnoid.UI.Views.Abstractions;

namespace Madnessnoid.UI.Presenters
{
    using Abstractions;

    public class SettingsMenuPresenter : PresenterBase<ISettingsMenuView>, ISettingsMenuPresenter, IPresenter<ISettingsMenuView>
    {
        public event Action? Close;

        public SettingsMenuPresenter(IViewFactory viewFactory,
            IPresenterNavigator navigator,
            GameSettings gameSettings) : base(viewFactory, navigator)
        {
            _gameSettings = gameSettings;
        }

        protected override void Subscribe()
        {
            _gameSettings.ShowFpsChanged += OnModelShowFpsChanged;
            _gameSettings.MusicVolumeChanged += OnModelMusicVolumeChanged;
            _gameSettings.PussyModeChanged += OnPussyModeChanged;
            View.ShowFps = _gameSettings.ShowFps;
            View.MusicVolume = _gameSettings.MusicVolume;
            View.PussyMode = _gameSettings.PussyMode;
            View.ApplyRequested += OnApply;
            View.CancelRequested += OnCancel;
        }

        protected override void Unsubscribe()
        {
            _gameSettings.ShowFpsChanged -= OnModelShowFpsChanged;
            _gameSettings.MusicVolumeChanged -= OnModelMusicVolumeChanged;
            _gameSettings.PussyModeChanged -= OnPussyModeChanged;
            View.ApplyRequested -= OnApply;
            View.CancelRequested -= OnCancel;
        }

        private readonly GameSettings _gameSettings;

        private void OnModelShowFpsChanged(bool state) => View.ShowFps = state;

        private void OnModelMusicVolumeChanged(float musicVolume) => View.MusicVolume = musicVolume;

        private void OnPussyModeChanged(bool state) => View.PussyMode = state;

        private void OnApply()
        {
            _gameSettings.ShowFps = View.ShowFps;
            _gameSettings.MusicVolume = View.MusicVolume;
            _gameSettings.PussyMode = View.PussyMode;
            Close?.Invoke();
        }

        private void OnCancel() => Close?.Invoke();
    }
}