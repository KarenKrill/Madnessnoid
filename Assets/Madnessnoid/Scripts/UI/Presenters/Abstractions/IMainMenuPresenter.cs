#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;

namespace Madnessnoid.UI.Presenters.Abstractions
{
    using Views.Abstractions;

    public interface IMainMenuPresenter : IPresenter<IMainMenuView>
    {
        public event Action? NewGame;
        public event Action? Exit;
    }
}
