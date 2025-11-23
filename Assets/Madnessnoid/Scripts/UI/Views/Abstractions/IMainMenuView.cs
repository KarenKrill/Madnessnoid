#nullable enable

using System;

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace Madnessnoid.UI.Views.Abstractions
{
    public interface IMainMenuView : IView
    {
        public event Action? NewGameRequested;
        public event Action? ExitRequested;
        public event Action? SettingsOpenRequested;
    }
}