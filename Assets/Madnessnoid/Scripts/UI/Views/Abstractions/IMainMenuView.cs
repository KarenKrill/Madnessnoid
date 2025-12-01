#nullable enable

using System;

using UnityEngine;

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace Madnessnoid.UI.Views.Abstractions
{
    public interface IMainMenuView : IView
    {
        public string MoneyText { set; }
        public Sprite MoneyIcon { set; }

        public event Action? NewGameRequested;
        public event Action? ExitRequested;
        public event Action? SettingsOpenRequested;
    }
}