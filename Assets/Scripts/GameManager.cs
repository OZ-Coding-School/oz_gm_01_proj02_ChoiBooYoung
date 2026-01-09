using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Reference")]
    [SerializeField] private Board board;
    [SerializeField] private UIManager ui;

    [Header("Game State")]
    public int score { get; private set; }
    public int level { get; private set; }
    public int lines { get; private set; }

    [Header("Level Config")]
    [SerializeField] private int linesPerLevel = 10;
    [SerializeField] private float baseStepDelay = 1f;

    private TetrominoData nextData;
    private TetrominoData? holdData = null;
    private bool canHold = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        score = 0;
        level = 0;
        lines = 0;

        UpdateUI();

        GenerateNext();
        SpawnFromNext();
    }

    /* ---------- Spawn ---------- */

    private void GenerateNext()
    {
        nextData = board.tetrominoes[
            Random.Range(0, board.tetrominoes.Length)
        ];

        ui.SetNextPiece(nextData);
    }

    public void SpawnFromNext()
    {
        board.SpawnPiece(nextData);
        GenerateNext();
        canHold = true;

        UpdateSpeed();
    }

    /* ---------- Score ---------- */

    public void OnLinesCleared(int cleared)
    {
        if (cleared == 0) return;

        lines += cleared;
        score += CalculateScore(cleared);

        level = lines / linesPerLevel;

        UpdateSpeed();
        UpdateUI();
    }

    private int CalculateScore(int cleared)
    {
        int baseScore = cleared switch
        {
            1 => 100,
            2 => 300,
            3 => 500,
            4 => 800,
            _ => 0
        };

        return baseScore * (level + 1);
    }

    private void UpdateSpeed()
    {
        if (board.activePiece == null) return;

        board.activePiece.stepDelay =
            Mathf.Max(0.1f, baseStepDelay - level * 0.1f);
    }

    private void UpdateUI()
    {
        ui.UpdateScore(score);
        ui.UpdateLevel(level);
        ui.UpdateLines(lines);
    }

    /* ---------- Game Over ---------- */

    public void GameOver()
    {
        ui.ShowGameOver();
    }
}
