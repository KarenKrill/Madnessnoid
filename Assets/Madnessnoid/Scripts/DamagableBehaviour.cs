using System;

using UnityEngine;

namespace Madnessnoid
{
    using Abstractions;
    
    public class DamagableBehaviour : MonoBehaviour, IDamagable
    {
        public float Health => _health;

        public event Action<IDamagable> Died;

        public void Damage(float value)
        {
            _health -= value;
            if (_health <= float.Epsilon)
            {
                try
                {
                    OnDied();
                }
                finally
                {
                    Died?.Invoke(this);
                }
            }
        }

        protected virtual void OnDied() { }

        [SerializeField]
        private float _health = 1;
    }
}
