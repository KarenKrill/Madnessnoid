#nullable enable

using KarenKrill.UniCore.UI.Views.Abstractions;
using System;
using UnityEngine;

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