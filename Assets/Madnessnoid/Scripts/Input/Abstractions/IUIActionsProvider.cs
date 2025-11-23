#nullable enable

using System;

using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;

namespace Madnessnoid.Input.Abstractions
{
    public interface IUIActionsProvider
    {
        public Vector2 LastNavigateValue { get; }
        public Vector2 LastPointValue { get; }
        public Vector2 LastScrollWheelValue { get; }

        public event NavigateDelegate? Navigate;
        public event Action? NavigateCancel;
        public event PointDelegate? Point;
        public event Action? PointCancel;
        public event ScrollWheelDelegate? ScrollWheel;
        public event Action? ScrollWheelCancel;
        public event Action? Submit;
        public event Action? Cancel;
        public event Action? Click;
        public event Action? RightClick;
        public event Action? MiddleClick;
    }
}
