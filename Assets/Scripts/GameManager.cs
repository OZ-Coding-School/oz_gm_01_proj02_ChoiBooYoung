using UnityEngine;

/// <summary>
/// 테트리스 게임 규칙 / 점수 / 레벨 / 상태 관리
/// Board 기준으로 동작하도록 제작됨
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Board board;

    [Header("Game State")]
    public int score { get; private set; }
    public int level { get; private set; }
    public int lines { get; private set; }

    private bool isGameOver;

    private void Start()
    {
        StartGame();
    }

    /// <summary>
    /// 게임 시작 / 재시작
    /// </summary>
    public void StartGame()
    {
        score = 0;
        level = 1;
        lines = 0;
        isGameOver = false;

        uiManager.UpdateScore(score);
        uiManager.UpdateLevel(level);
        uiManager.UpdateLines(lines);
    }

    /// <summary>
    /// Board에서 줄 제거 후 호출
    /// </summary>
    public void OnLinesCleared(int clearedLines)
    {
        if (isGameOver || clearedLines == 0) return;

        lines += clearedLines;
        score += CalculateScore(clearedLines);
        level = (lines / 10) + 1;

        uiManager.UpdateScore(score);
        uiManager.UpdateLevel(level);
        uiManager.UpdateLines(lines);
    }

    /// <summary>
    /// 점수 계산 규칙
    /// </summary>
    private int CalculateScore(int clearedLines)
    {
        switch (clearedLines)
        {
            case 1: return 100 * level;
            case 2: return 300 * level;
            case 3: return 500 * level;
            case 4: return 800 * level;
            default: return 0;
        }
    }

    /// <summary>
    /// 게임 오버 처리
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        uiManager.ShowGameOver();
    }
}
