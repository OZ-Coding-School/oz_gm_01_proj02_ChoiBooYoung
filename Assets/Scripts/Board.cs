using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

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
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++)
            tetrominoes[i].Initialize();
    }

    /* ---------- Spawn ---------- */

    public void SpawnPiece(TetrominoData data)
    {
        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
            Set(activePiece);
        else
            GameOver();
    }

    private void GameOver()
    {
        tilemap.ClearAllTiles();
        GameManager.Instance.GameOver();
    }

    /* ---------- Tile ---------- */

    public void Set(Piece piece)
    {
        foreach (Vector3Int cell in piece.cells)
            tilemap.SetTile(cell + piece.position, piece.data.tile);
    }

    public void Clear(Piece piece)
    {
        foreach (Vector3Int cell in piece.cells)
            tilemap.SetTile(cell + piece.position, null);
    }

    /* ---------- Check ---------- */

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        foreach (Vector3Int cell in piece.cells)
        {
            Vector3Int tilePos = cell + position;

            if (!bounds.Contains((Vector2Int)tilePos))
                return false;

            if (tilemap.HasTile(tilePos))
                return false;
        }

        return true;
    }

    /* ---------- Line ---------- */

    public int ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int cleared = 0;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                cleared++;
            }
            else
            {
                row++;
            }
        }

        return cleared;
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            if (!tilemap.HasTile(new Vector3Int(col, row, 0)))
                return false;
        }

        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
            tilemap.SetTile(new Vector3Int(col, row, 0), null);

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int above = new Vector3Int(col, row + 1, 0);
                TileBase tile = tilemap.GetTile(above);
                tilemap.SetTile(new Vector3Int(col, row, 0), tile);
            }
            row++;
        }
    }
}
