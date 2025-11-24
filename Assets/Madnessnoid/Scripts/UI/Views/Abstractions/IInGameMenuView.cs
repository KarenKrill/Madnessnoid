#nullable enable

using System;

using UnityEngine;

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace Madnessnoid.UI.Views.Abstractions
{
    public interface IInGameMenuView : IView
    {
        public string HitPointsCountText { set; }
        public Sprite HitPointIcon { set; }

        public event Action? PauseRequested;
    }
}