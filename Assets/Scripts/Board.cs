using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

// 이 스크립트는 다른 스크립트보다 먼저 실행되도록 설정
// Piece가 Update되기 전에 Board가 준비되게 하기 위함
[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    // 실제 블록을 그릴 타일맵
    public Tilemap tilemap { get; private set; }

    // 현재 조작 중인 블록(Piece)
    public Piece activePiece { get; private set; }

    // 모든 테트로미노 데이터 배열
    public TetrominoData[] tetrominoes;

    // 보드 크기 (가로 10, 세로 20)
    public Vector2Int boardSize = new Vector2Int(10, 20);

    // 새 블록이 생성될 위치
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    // 게임 매니저 참조
    [SerializeField] private GameManager gameManager;

    // 보드의 유효 범위를 Rect 형태로 반환
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(
                -boardSize.x / 2,
                -boardSize.y / 2
            );

            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        // 자식 오브젝트에서 Tilemap 컴포넌트 가져오기
        tilemap = GetComponentInChildren<Tilemap>();

        // 자식 오브젝트에서 Piece 컴포넌트 가져오기
        activePiece = GetComponentInChildren<Piece>();

        // 모든 테트로미노 데이터 초기화
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        // 게임 시작 시 첫 블록 생성
        SpawnPiece();
    }

    // 새로운 테트로미노 블록 생성
    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    // 게임 오버 처리
    public void GameOver()
    {
        tilemap.ClearAllTiles();

        // 게임 상태 처리는 GameManager에게 위임
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }

    // 블록을 타일맵에 그리기
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    // 블록을 타일맵에서 제거
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    // 특정 위치가 유효한지 검사
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
                return false;

            if (tilemap.HasTile(tilePosition))
                return false;
        }

        return true;
    }

    // 꽉 찬 줄들을 검사하고 삭제
    // 👉 삭제한 줄 수를 반환
    public int ClearLines()
    {
        RectInt bounds = Bounds;
        int clearedLines = 0;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                clearedLines++;
            }
            else
            {
                row++;
            }
        }

        // GameManager에 줄 제거 결과 전달
        if (clearedLines > 0 && gameManager != null)
        {
            gameManager.OnLinesCleared(clearedLines);
        }

        return clearedLines;
    }

    // 특정 줄이 꽉 찼는지 확인
    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
                return false;
        }

        return true;
    }

    // 한 줄 삭제 및 위 줄 내리기
    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // 해당 줄 제거
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // 위 줄들 아래로 내리기
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int abovePos = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(abovePos);

                Vector3Int curPos = new Vector3Int(col, row, 0);
                tilemap.SetTile(curPos, above);
            }

            row++;
        }
    }
}
