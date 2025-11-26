using UnityEngine;
using Zenject;
using Madnessnoid.Abstractions;

public class BrickWallGenerator : MonoBehaviour
{
    [Inject]
    public void Initialize(ILevelSession levelSession, IGameConfig gameConfig)
    {
        _levelSession = levelSession;
        _gameConfig = gameConfig;
    }

    [SerializeField]
    private GameObject _brickPrefab;
    [SerializeField]
    private Rect _minBounds;
    [SerializeField]
    private Rect _maxBounds;
    [SerializeField]
    private Vector2 _maxOffset;
    [SerializeField]
    private Gradient _gradient;

    private ILevelSession _levelSession;
    private IGameConfig _gameConfig;

    private void OnEnable()
    {
        _levelSession.LevelChanged += OnLevelChanged;
    }
    private void OnDisable()
    {
        _levelSession.LevelChanged -= OnLevelChanged;
    }

    private void GenerateLevelInternal(int bricksCount)
    {
        var brickBounds = _brickPrefab.GetComponentInChildren<SpriteRenderer>().bounds;
        var positions = LayoutElements(bricksCount, brickBounds.size, _minBounds, _maxBounds, _maxOffset, out var bounds);
        foreach (var pos in positions)
        {
            GameObject brick = Instantiate(_brickPrefab, transform);
            brick.transform.localPosition = pos;
            brick.GetComponent<SpriteRenderer>().color = _gradient.Evaluate(pos.y / (bounds.height - 1));
        }
        return;
    }
    private void OnLevelChanged(int levelId)
    {
        var levelConfig = _gameConfig.LevelsConfig[levelId];
        var blocksCount = levelConfig.BlocksCount;
        GenerateLevelInternal(blocksCount);
    }

    public static Vector2[] LayoutElements(
        int count,
        Vector2 elementSize,
        Rect minBounds,
        Rect maxBounds,
        Vector2 maxOffset,
        out Rect bounds)
    {
        // ---------------------------
        // 1. Интерполяция bounds
        // ---------------------------

        // T = насколько много элементов (0 → минимальный bounds, 1 → максимальный)
        float t = Mathf.InverseLerp(1, 50, count);
        // Можно параметризовать: сейчас 1 элемент → minBounds, 50+ → maxBounds

        bounds = LerpRect(minBounds, maxBounds, t);

        // ---------------------------
        // 2. Вычисление количества колонок и строк
        // ---------------------------

        // Максимум элементов в строке по ширине:
        int maxColumns = Mathf.FloorToInt(bounds.width / (elementSize.x + maxOffset.x));
        maxColumns = Mathf.Max(1, maxColumns);

        // Реальные колонки — не больше count
        int columns = Mathf.Min(count, maxColumns);

        // Строки:
        int rows = Mathf.CeilToInt(count / (float)columns);

        // ---------------------------
        // 3. Расчёт итогового spacing (по факту может быть меньше maxOffset)
        // ---------------------------

        float spacingX = Mathf.Min(
            (bounds.width - columns * elementSize.x) / Mathf.Max(1, columns - 1),
            maxOffset.x
        );

        float spacingY = Mathf.Min(
            (bounds.height - rows * elementSize.y) / Mathf.Max(1, rows - 1),
            maxOffset.y
        );

        // Если элементов мало — spacing может быть отрицательным → ставим 0
        spacingX = Mathf.Max(0, spacingX);
        spacingY = Mathf.Max(0, spacingY);

        // ---------------------------
        // 4. Центр глобального грида внутри bounds
        // ---------------------------

        float totalWidth = columns * elementSize.x + (columns - 1) * spacingX;
        float totalHeight = rows * elementSize.y + (rows - 1) * spacingY;

        float startX = bounds.center.x - totalWidth / 2f;
        float startY = bounds.center.y + totalHeight / 2f; // сверху вниз

        // ---------------------------
        // 5. Раскладка элементов
        // ---------------------------

        Vector2[] positions = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            int row = i / columns;
            int col = i % columns;

            float x = startX + col * (elementSize.x + spacingX) + elementSize.x / 2f;
            float y = startY - row * (elementSize.y + spacingY) - elementSize.y / 2f;

            positions[i] = new Vector2(x, y);
        }

        return positions;
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
}
