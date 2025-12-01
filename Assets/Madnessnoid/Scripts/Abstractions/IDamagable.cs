#nullable enable

using System;

namespace Madnessnoid.Abstractions
{
    public interface IDamagable
    {
        public float Health { get; }

        public event Action<IDamagable>? Damaged;
        public event Action<IDamagable>? Died;

        public void Damage(float value);
    }
}
