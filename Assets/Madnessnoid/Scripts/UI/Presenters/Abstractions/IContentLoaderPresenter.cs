#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;

namespace Madnessnoid.UI.Presenters.Abstractions
{
    using Views.Abstractions;

    public interface IContentLoaderPresenter : IPresenter<IContentLoaderView>
    {
        public string StageText { set; }
        public float ProgressValue { set; }
        public string StatusText { set; }
        public bool EnableContinue { set; }

        public event Action? Continue;
    }
}
