using System;

using UnityEngine;

namespace Madnessnoid
{
    public class BrickBehaviour : MonoBehaviour
    {
        public float Health => _health;

        public event Action<BrickBehaviour> Died;

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

        protected virtual void OnDied()
        {
            gameObject.SetActive(false);
        }

        [SerializeField]
        private float _health = 1;
    }
}
