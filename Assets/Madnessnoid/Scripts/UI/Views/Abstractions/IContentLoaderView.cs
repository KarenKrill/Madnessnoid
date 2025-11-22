#nullable enable

using System;

using UnityEngine;

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace Madnessnoid.UI.Views.Abstractions
{
    public interface IContentLoaderView : IView
    {
        /// <summary>
        /// Progress value between 0 and 1
        /// </summary>
        public float ProgressValue { set; }
        public Color ProgressColor { set; }
        public Color ProgressBackColor { set; }
        public string ProgressText { set; }
        public string StageText { set; }
        public Color TextColor { set; }
        public Sprite BackgroundImage { set; }
        public string StatusText { set; }
        public Color StatusTextColor { set; }
        public bool EnableContinue { set; }

        public event Action? ContinueRequested;
    }
}