#nullable enable

using System;
using UnityEngine;
using KarenKrill.UniCore.UI.Views.Abstractions;

namespace Madnessnoid.UI.Views.Abstractions
{
    public interface ILevelEndMenuView : IView
    {
        public string TitleText { set; }
        public Color TitleTextColor { set; }
        public bool EnableContinue { set; }
        public string CashRewardText { set; }
        public Sprite CashRewardIcon { set; }
        public bool EnableReward { set; }

        public event Action? ContinueRequested;
        public event Action? RestartRequested;
        public event Action? MainMenuExitRequested;
        public event Action? ExitRequested;
    }
}