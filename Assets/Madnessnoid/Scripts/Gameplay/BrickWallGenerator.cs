using System.Collections.Generic;

using UnityEngine;

using Zenject;

using KarenKrill.UniCore.Instantiattion;

namespace Madnessnoid
{
    using Abstractions;
    
    public class BrickWallGenerator : MonoBehaviour
    {
        [Inject]
        public void Initialize(ILevelSession levelSession,
            IGameConfig gameConfig,
            IThemeProfileProvider themeProfileProvider,
            IAudioController audioController)
        {
            _levelSession = levelSession;
            _gameConfig = gameConfig;
            _themeProfileProvider = themeProfileProvider;
            _audioController = audioController;
        }

        [SerializeField]
        private BrickBehaviour _brickPrefab;
        [SerializeField]
        private Rect _minBounds;
        [SerializeField]
        private Rect _maxBounds;
        [SerializeField]
        private Vector2 _maxOffset;
        [SerializeField]
        private Gradient _gradient;
        [SerializeField]
        private int _startBrickPoolCapacity = 10;

        private const int _MaxElementsCountForMinBoundsUsage = 1;
        private const int _MinElementsCountForMaxBoundsUsage = 50;

        private ILevelSession _levelSession;
        private IGameConfig _gameConfig;
        private IThemeProfileProvider _themeProfileProvider;
        private IAudioController _audioController;
        private ComponentPool<BrickBehaviour> _brickPool;
        private readonly List<BrickBehaviour> _brickInstances = new();
        private int _levelId = 0;

        private void Awake()
        {
            _brickPool = new(_brickPrefab, transform, _startBrickPoolCapacity);
        }

        private void OnDestroy()
        {
            _brickPool.Dispose();
        }

        private void OnEnable()
        {
            _levelSession.LevelChanged += OnLevelChanged;
            _themeProfileProvider.ActiveThemeChanged += OnActiveThemeChanged;
        }

        private void OnDisable()
        {
            _levelSession.LevelChanged -= OnLevelChanged;
            _themeProfileProvider.ActiveThemeChanged -= OnActiveThemeChanged;
        }

        private void GenerateLevelInternal(int bricksCount)
        {
            ClearBrickPool();
            var brickBounds = _brickPrefab.gameObject.GetComponentInChildren<SpriteRenderer>().bounds;
            var positions = LayoutElements(bricksCount, brickBounds.size, _minBounds, _maxBounds, _maxOffset, out var bounds);
            foreach (var pos in positions)
            {
                var brick = _brickPool.Get();
                _brickInstances.Add(brick);
                brick.Initialize(_audioController);
                brick.transform.localPosition = pos;
                brick.GetComponent<SpriteRenderer>().color = _gradient.Evaluate(pos.y / (bounds.height - 1));
                brick.Died += OnBrickDied;
            }
            UpdateBrickThemes();
            return;
        }

        private void UpdateBrickThemes()
        {
            var activeTheme = _themeProfileProvider.ActiveTheme;
            var levelTheme = activeTheme.LevelThemes[_levelId];
            foreach (var brick in _brickInstances)
            {
                var brickThemeIndex = Random.Range(0, levelTheme.BrickThemes.Count);
                brick.BrickTheme = levelTheme.BrickThemes[brickThemeIndex];
            }
        }

        private void ClearBrickPool()
        {
            foreach (var brick in _brickInstances)
            {
                _brickPool.Release(brick);
            }
            _brickInstances.Clear();
        }

        private static Rect LerpRect(Rect a, Rect b, float t)
        {
            return new Rect(
                Mathf.Lerp(a.x, b.x, t),
                Mathf.Lerp(a.y, b.y, t),
                Mathf.Lerp(a.width, b.width, t),
                Mathf.Lerp(a.height, b.height, t)
            );
        }

        private static Vector2[] LayoutElements(
            int count,
            Vector2 elementSize,
            Rect minBounds,
            Rect maxBounds,
            Vector2 maxOffset,
            out Rect bounds)
        {
            // 1. Bounds interpolation

            // 0 → min bounds, 1 → max bounds
            float itemsCountWeight = Mathf.InverseLerp(_MaxElementsCountForMinBoundsUsage, _MinElementsCountForMaxBoundsUsage, count);
            bounds = LerpRect(minBounds, maxBounds, itemsCountWeight);

            // 2. Calculating the number of columns and rows

            int maxColumnsCount = Mathf.FloorToInt(bounds.width / (elementSize.x + maxOffset.x));
            maxColumnsCount = Mathf.Max(1, maxColumnsCount);
            int columnsCount = Mathf.Min(count, maxColumnsCount);
            int rowsCount = Mathf.CeilToInt(count / (float)columnsCount);

            // 3. Calculation of the final spacing (in fact, it may be less than maxOffset)

            float spacingX = Mathf.Min(
                (bounds.width - columnsCount * elementSize.x) / Mathf.Max(1, columnsCount - 1),
                maxOffset.x
            );
            float spacingY = Mathf.Min(
                (bounds.height - rowsCount * elementSize.y) / Mathf.Max(1, rowsCount - 1),
                maxOffset.y
            );
            // If there are few elements, spacing can be negative → set to 0
            spacingX = Mathf.Max(0, spacingX);
            spacingY = Mathf.Max(0, spacingY);

            // 4. The center of the global grid is inside bounds

            float totalWidth = columnsCount * elementSize.x + (columnsCount - 1) * spacingX;
            float totalHeight = rowsCount * elementSize.y + (rowsCount - 1) * spacingY;
            float startX = bounds.center.x - totalWidth / 2f;
            float startY = bounds.center.y + totalHeight / 2f; // top down

            // 5. Layout of elements

            Vector2[] positions = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                int row = i / columnsCount;
                int col = i % columnsCount;

                float x = startX + col * (elementSize.x + spacingX) + elementSize.x / 2f;
                float y = startY - row * (elementSize.y + spacingY) - elementSize.y / 2f;

                positions[i] = new Vector2(x, y);
            }
            return positions;
        }

        private void OnBrickDied(IDamagable damagable)
        {
            if (damagable is BrickBehaviour brick)
            {
                _brickInstances.Remove(brick);
                _brickPool.Release(brick);
            }
        }

        private void OnLevelChanged(int levelId)
        {
            _levelId = levelId;
            var levelConfig = _gameConfig.LevelsConfig[levelId];
            var blocksCount = levelConfig.BlocksCount;
            GenerateLevelInternal(blocksCount);
        }

        private void OnActiveThemeChanged() => UpdateBrickThemes();
    }
}
