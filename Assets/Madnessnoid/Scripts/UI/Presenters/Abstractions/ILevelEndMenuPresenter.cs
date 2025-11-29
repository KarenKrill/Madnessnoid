#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;

using Madnessnoid.UI.Views.Abstractions;

namespace Madnessnoid.UI.Presenters.Abstractions
{
    public interface ILevelEndMenuPresenter : IPresenter<ILevelEndMenuView>
    {
        public event Action? Continue;
        public event Action? Restart;
        public event Action? MainMenu;
        public event Action? Exit;
    }
}
