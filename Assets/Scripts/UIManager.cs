using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

/// <summary>
/// 테트리스 UI 전담 매니저
/// 점수 / 레벨 / 라인 / Next / Hold / GameOver UI를 관리한다
/// </summary>
public class UIManager : MonoBehaviour
{
    /* ===================== Text UI ===================== */

    [Header("Text UI")]
    // 점수 텍스트
    [SerializeField] private TextMeshProUGUI scoreText;
    // 레벨 텍스트
    [SerializeField] private TextMeshProUGUI levelText;
    // 제거한 라인 수 텍스트
    [SerializeField] private TextMeshProUGUI linesText;

    /* ===================== Next Piece UI ===================== */

    [Header("Next Piece UI")]
    // 다음 블록을 표시할 Tilemap
    [SerializeField] private Tilemap nextTilemap;
    // 다음 블록 위치 보정용 오프셋
    [SerializeField] private Vector3Int nextTileOffset = new Vector3Int(-1, -1, 0);

    /* ===================== Hold Piece UI ===================== */

    [Header("Hold Piece UI")]
    // 홀드 블록을 표시할 Tilemap
    [SerializeField] private Tilemap holdTilemap;
    // 홀드 블록 위치 보정용 오프셋
    [SerializeField] private Vector3Int holdTileOffset = new Vector3Int(-1, -1, 0);

    /* ===================== Result UI ===================== */

    [Header("Result UI")]
    // 게임 오버 패널
    [SerializeField] private GameObject gameOverPanel;

    /// <summary>
    /// 게임 시작 시 UI 초기화
    /// </summary>
    private void Awake()
    {
        ClearNext();      // Next UI 초기화
        ClearHold();      // Hold UI 초기화
        HideGameOver();   // 게임 오버 UI 숨김
    }

    /* ===================== Text ===================== */

    /// <summary>
    /// 점수 UI 갱신
    /// </summary>
    public void UpdateScore(int score)
    {
        // 항상 6자리로 표시 (000123)
        scoreText.text = $"SCORE : {score:D6}";
    }

    /// <summary>
    /// 레벨 UI 갱신
    /// </summary>
    public void UpdateLevel(int level)
    {
        // 항상 2자리로 표시
        levelText.text = $"LEVEL : {level:D2}";
    }

    /// <summary>
    /// 제거한 라인 수 UI 갱신
    /// </summary>
    public void UpdateLines(int lines)
    {
        linesText.text = $"LINES : {lines:D2}";
    }

    /* ===================== Next ===================== */

    /// <summary>
    /// 다음에 나올 테트로미노를 Next UI에 표시
    /// </summary>
    public void SetNextPiece(TetrominoData data)
    {
        // 기존 Next UI 클리어
        ClearNext();

        // 테트로미노 셀 정보를 기준으로 타일 배치
        foreach (Vector2Int cell in data.cells)
        {
            Vector3Int tilePos = new Vector3Int(
                cell.x + nextTileOffset.x,
                cell.y + nextTileOffset.y,
                0
            );

            nextTilemap.SetTile(tilePos, data.tile);
        }
    }

    /// <summary>
    /// Next UI 타일 전부 제거
    /// </summary>
    private void ClearNext()
    {
        if (nextTilemap != null)
            nextTilemap.ClearAllTiles();
    }

    /* ===================== Hold ===================== */

    /// <summary>
    /// 홀드된 테트로미노를 Hold UI에 표시
    /// </summary>
    public void SetHoldPiece(TetrominoData data)
    {
        // 기존 Hold UI 클리어
        ClearHold();

        // 테트로미노 셀 정보를 기준으로 타일 배치
        foreach (Vector2Int cell in data.cells)
        {
            Vector3Int tilePos = new Vector3Int(
                cell.x + holdTileOffset.x,
                cell.y + holdTileOffset.y,
                0
            );

            holdTilemap.SetTile(tilePos, data.tile);
        }
    }

    /// <summary>
    /// Hold UI 타일 전부 제거
    /// </summary>
    private void ClearHold()
    {
        if (holdTilemap != null)
            holdTilemap.ClearAllTiles();
    }

    /* ===================== Game State ===================== */

    /// <summary>
    /// 게임 오버 UI 표시
    /// </summary>
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// 게임 오버 UI 숨김
    /// </summary>
    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}