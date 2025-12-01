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
            IAudioController audioController,
            IThemeProfileProvider themeProfileProvider)
        {
            _gameConfig = gameConfig;
            _levelSession = levelSession;
            _audioController = audioController;
            _themeProfileProvider = themeProfileProvider;
        }

        public void Push(Vector2 direction, float magnitude)
        {
            IsPushed = true;
            var force = direction * magnitude;
            _rigidbody.AddForce(force, ForceMode2D.Impulse);
        }

        [SerializeField]
        private Rigidbody2D _rigidbody;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
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
        
        private IGameConfig _gameConfig;
        private ILevelSession _levelSession;
        private IAudioController _audioController;
        private IThemeProfileProvider _themeProfileProvider;
        private ILevelTheme _activeLevelTheme = null;
        private int _levelId = 0;

        private void UpdateTheme()
        {
            var newTheme = _themeProfileProvider.ActiveTheme.LevelThemes[_levelId];
            if (_activeLevelTheme != newTheme)
            {
                _activeLevelTheme = newTheme;
                _spriteRenderer.sprite = _activeLevelTheme.BallSprite;
            }
        }

        private void OnEnable()
        {
            _levelSession.LevelCompleted += OnLevelCompleted;
            _levelSession.LevelChanged += OnLevelChanged;
            _themeProfileProvider.ActiveThemeChanged += OnActiveThemeChanged;
            OnLevelChanged(_levelSession.LevelId);
            OnActiveThemeChanged();
        }

        private void OnDisable()
        {
            _levelSession.LevelCompleted -= OnLevelCompleted;
            _levelSession.LevelChanged -= OnLevelChanged;
            _themeProfileProvider.ActiveThemeChanged -= OnActiveThemeChanged;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_deathZoneLayer.Contains(collision.gameObject.layer))
            {
                _rigidbody.linearVelocity = Vector2.zero;
                _rigidbody.angularVelocity = 0;
                _rigidbody.rotation = 0;
                _rigidbody.position = _spawnPoint.position;
                IsPushed = false;
                _levelSession.TakeDamage();

                if (_activeLevelTheme.LosingHitPointSounds.Count > 0)
                {
                    var clipIndex = Random.Range(0, _activeLevelTheme.LosingHitPointSounds.Count);
                    _audioController.PlaySfx(_activeLevelTheme.LosingHitPointSounds[clipIndex]);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_breakableLayer.Contains(collision.gameObject.layer))
            {
                if (collision.gameObject.TryGetComponent<BrickBehaviour>(out var brick))
                {
                    brick.Damage(1);
                }
                else
                {
                    collision.gameObject.SetActive(false);
                    Destroy(collision.gameObject);
                }
                _levelSession.BreakTheBlock(0);
            }
            else if (_activeLevelTheme.WallCollisionSounds.Count > 0)
            {
                var clipIndex = Random.Range(0, _activeLevelTheme.WallCollisionSounds.Count);
                _audioController.PlaySfx(_activeLevelTheme.WallCollisionSounds[clipIndex]);
            }
        }

        private void FixedUpdate()
        {
            if (Mathf.Abs(_rigidbody.angularVelocity) > _velocityEpsilon
                && Mathf.Abs(_rigidbody.angularVelocity - _angularVelocity) > float.Epsilon)
            {
                _rigidbody.angularVelocity = _angularVelocity;
            }
            if ((Mathf.Abs(_rigidbody.linearVelocityX) > _velocityEpsilon
                || Mathf.Abs(_rigidbody.linearVelocityY) > _velocityEpsilon)
                && Mathf.Abs(_rigidbody.linearVelocity.magnitude - _velocityMagnitude) > float.Epsilon)
            {
                var targetVelocity = _rigidbody.linearVelocity.normalized * _velocityMagnitude;
                _rigidbody.linearVelocity = targetVelocity;
            }
        }

        private void OnLevelCompleted(LevelCompletionResult result)
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        private void OnLevelChanged(int levelId)
        {
            if (levelId >= 0)
            {
                _levelId = levelId;
                var levelConfig = _gameConfig.LevelsConfig[levelId];
                _velocityMagnitude = levelConfig.BallVelocity;
                _angularVelocity = levelConfig.BallAngularVelocity;
                UpdateTheme();
            }
        }

        private void OnActiveThemeChanged() => UpdateTheme();
    }
}
