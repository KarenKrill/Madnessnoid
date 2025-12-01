#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;

using Madnessnoid.UI.Views.Abstractions;

namespace Madnessnoid.UI.Presenters.Abstractions
{
    public interface IMainMenuPresenter : IPresenter<IMainMenuView>
    {
        public event Action? NewGame;
        public event Action? Exit;
    }
}
