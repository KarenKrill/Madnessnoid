#nullable enable

using System;

using KarenKrill.UniCore.UI.Presenters.Abstractions;

using Madnessnoid.UI.Views.Abstractions;

namespace Madnessnoid.UI.Presenters.Abstractions
{
    public interface ISettingsMenuPresenter : IPresenter<ISettingsMenuView>
    {
        public event Action? Close;
    }
}
