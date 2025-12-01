#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;

using Madnessnoid.UI.Views.Abstractions;

namespace Madnessnoid.UI.Presenters.Abstractions
{
    public interface IInGameMenuPresenter : IPresenter<IInGameMenuView>
    {
        public event Action? Pause;
    }
}
