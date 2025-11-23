#nullable enable

using System;

using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;

namespace Madnessnoid.Input.Abstractions
{
    public interface IPlayerActionsProvider
    {
        public Vector2 LastMoveDelta { get; }
        public bool IsSprintActive { get; }
        public bool IsAttackActive { get; }

        public event Action? MoveStarted;
        public event MoveDelegate? Move;
        public event Action? MoveCancel;
        public event Action? Sprint;
        public event Action? SprintCancel;
        public event Action? Attack;
        public event Action? AttackCancel;
        public event Action? Pause;
    }
}
