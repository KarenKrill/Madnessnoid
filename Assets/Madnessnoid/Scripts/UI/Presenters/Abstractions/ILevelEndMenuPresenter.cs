#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;

namespace Madnessnoid.UI.Presenters.Abstractions
{
    using Views.Abstractions;

    public interface ILevelEndMenuPresenter : IPresenter<ILevelEndMenuView>
    {
        public event Action? Continue;
        public event Action? Restart;
        public event Action? MainMenu;
        public event Action? Exit;
    }
}
