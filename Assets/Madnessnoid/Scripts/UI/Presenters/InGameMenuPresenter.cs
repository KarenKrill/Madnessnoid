using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views.Abstractions;

using Madnessnoid.Abstractions;
using Madnessnoid.UI.Views.Abstractions;

namespace Madnessnoid.UI.Presenters
{
    using Abstractions;

    public class InGameMenuPresenter : PresenterBase<IInGameMenuView>, IInGameMenuPresenter, IPresenter<IInGameMenuView>
    {
        public event Action Pause;

        public InGameMenuPresenter(IViewFactory viewFactory,
            IPresenterNavigator navigator,
            IThemeProfileProvider themeProfileProvider,
            ILevelSession levelSession) : base(viewFactory, navigator)
        {
            _themeProfileProvider = themeProfileProvider;
            _levelSession = levelSession;
        }

        protected override void Subscribe()
        {
            View.PauseRequested += OnPause;
            _themeProfileProvider.ActiveThemeChanged += OnActiveThemeChanged;
            _levelSession.HitPointsCountChanged += OnHitPointsCountChanged;
            _levelSession.LevelScoreChanged += OnLevelScoreChanged;
            OnHitPointsCountChanged(_levelSession.HitPointsCount);
            OnLevelScoreChanged(_levelSession.LevelScore);
            OnActiveThemeChanged();
        }

        protected override void Unsubscribe()
        {
            View.PauseRequested -= OnPause;
            _themeProfileProvider.ActiveThemeChanged -= OnActiveThemeChanged;
            _levelSession.HitPointsCountChanged -= OnHitPointsCountChanged;
            _levelSession.LevelScoreChanged -= OnLevelScoreChanged;
        }
        
        private readonly IThemeProfileProvider _themeProfileProvider;
        private readonly ILevelSession _levelSession;

        private void OnPause() => Pause?.Invoke();

        private void OnActiveThemeChanged()
        {
            var activeTheme = _themeProfileProvider.ActiveTheme;
            View.HitPointIcon = activeTheme.HitPointIcon;
            View.ScoreIcon = activeTheme.ScoreIcon;
        }

        private void OnHitPointsCountChanged(int hitPointsCount)
        {
            View.HitPointsCountText = hitPointsCount.ToString();
        }

        private void OnLevelScoreChanged(int score)
        {
            View.ScoreText = score.ToString();
        }
    }
}