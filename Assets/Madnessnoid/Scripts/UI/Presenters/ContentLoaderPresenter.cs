using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views.Abstractions;

using Madnessnoid.UI.Views.Abstractions;

namespace Madnessnoid.UI.Presenters
{
    using Abstractions;

    public class ContentLoaderPresenter : PresenterBase<IContentLoaderView>, IContentLoaderPresenter
    {
        public string StageText { set => View.StageText = value; }
        public string StatusText { set => View.StatusText = value; }
        public bool EnableContinue { set => View.EnableContinue = value; }
        public float ProgressValue
        {
            set
            {
                View.ProgressText = $"{100 * value:0.00} %";
                View.ProgressValue = value;
            }
        }

        public event Action Continue;

        public ContentLoaderPresenter(IViewFactory viewFactory,
            IPresenterNavigator navigator) : base(viewFactory, navigator)
        {
        }

        protected override void Subscribe()
        {
            View.ContinueRequested += OnViewContinueRequested;
            View.StatusText = string.Empty;
            View.ProgressValue = 0;
            View.EnableContinue = false;
            View.StageText = string.Empty;
        }

        protected override void Unsubscribe()
        {
            View.ContinueRequested -= OnViewContinueRequested;
        }

        private void OnViewContinueRequested() => Continue?.Invoke();
    }
}