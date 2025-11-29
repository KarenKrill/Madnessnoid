using System.Collections.Generic;

using UnityEngine;

using Zenject;

using KarenKrill.Utilities;

namespace Madnessnoid
{
    using Abstractions;
    
    public class BallController : MonoBehaviour
    {
        public bool IsPushed { get; private set; } = false;

        [Inject]
        public void Initialize(IGameConfig gameConfig,
            ILevelSession levelSession,
            IAudioController audioController)
        {
            _gameConfig = gameConfig;
            _levelSession = levelSession;
            _audioController = audioController;
        }

        public void Push(Vector2 direction, float magnitude)
        {
            IsPushed = true;
            var force = direction * magnitude;
            _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        }

        [SerializeField]
        private Rigidbody2D _rigidbody2D;
        [SerializeField]
        private float _velocityMagnitude = 10;
        [SerializeField]
        private float _angularVelocity = 180;
        [SerializeField]
        private float _velocityEpsilon = 0.0001f;
        [SerializeField]
        private Transform _spawnPoint;
        [SerializeField]
        private LayerMask _deathZoneLayer;
        [SerializeField]
        private LayerMask _breakableLayer;
        [SerializeField]
        private List<AudioClip> _deathSounds = new();
        [SerializeField]
        private List<AudioClip> _collisionSounds = new();
        [SerializeField]
        private AudioClip _loseHitPointClip;
        
        private IGameConfig _gameConfig;
        private ILevelSession _levelSession;
        private IAudioController _audioController;

        private void OnEnable()
        {
            _levelSession.LevelCompleted += OnLevelCompleted;
            _levelSession.LevelChanged += OnLevelChanged;
            OnLevelChanged(_levelSession.LevelId);
        }

        private void OnDisable()
        {
            _levelSession.LevelCompleted -= OnLevelCompleted;
            _levelSession.LevelChanged -= OnLevelChanged;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_deathZoneLayer.Contains(collision.gameObject.layer))
            {
                _rigidbody2D.linearVelocity = Vector2.zero;
                _rigidbody2D.angularVelocity = 0;
                _rigidbody2D.rotation = 0;
                _rigidbody2D.position = _spawnPoint.position;
                IsPushed = false;
                _levelSession.TakeDamage();

                if (_loseHitPointClip != null)
                {
                    _audioController.PlaySfx(_loseHitPointClip);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_breakableLayer.Contains(collision.gameObject.layer))
            {
                if (_deathSounds.Count > 0)
                {
                    var clipIndex = Random.Range(0, _deathSounds.Count);
                    _audioController.PlaySfx(_deathSounds[clipIndex]);
                }
                collision.gameObject.SetActive(false);
                Destroy(collision.gameObject);
                _levelSession.BreakTheBlock(0);
            }
            if (_collisionSounds.Count > 0)
            {
                var clipIndex = Random.Range(0, _collisionSounds.Count);
                _audioController.PlaySfx(_collisionSounds[clipIndex]);
            }
        }

        private void FixedUpdate()
        {
            if (Mathf.Abs(_rigidbody2D.angularVelocity) > _velocityEpsilon
                && Mathf.Abs(_rigidbody2D.angularVelocity - _angularVelocity) > float.Epsilon)
            {
                _rigidbody2D.angularVelocity = _angularVelocity;
            }
            if ((Mathf.Abs(_rigidbody2D.linearVelocityX) > _velocityEpsilon
                || Mathf.Abs(_rigidbody2D.linearVelocityY) > _velocityEpsilon)
                && Mathf.Abs(_rigidbody2D.linearVelocity.magnitude - _velocityMagnitude) > float.Epsilon)
            {
                var targetVelocity = _rigidbody2D.linearVelocity.normalized * _velocityMagnitude;
                _rigidbody2D.linearVelocity = targetVelocity;
            }
        }

        private void OnLevelCompleted(LevelCompletionResult result)
        {
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        private void OnLevelChanged(int levelId)
        {
            if (levelId >= 0)
            {
                var levelConfig = _gameConfig.LevelsConfig[levelId];
                _velocityMagnitude = levelConfig.BallVelocity;
                _angularVelocity = levelConfig.BallAngularVelocity;
            }
        }
    }
}
