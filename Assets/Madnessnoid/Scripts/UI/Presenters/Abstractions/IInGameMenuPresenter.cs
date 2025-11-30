#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;

namespace Madnessnoid.UI.Presenters.Abstractions
{
    using Views.Abstractions;

    public interface IInGameMenuPresenter : IPresenter<IInGameMenuView>
    {
        public event Action? Pause;
    }
}
