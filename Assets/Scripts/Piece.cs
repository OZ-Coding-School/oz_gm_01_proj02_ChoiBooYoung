using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    [Header("Timing")]
    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float moveTime;
    private float lockTime;

    /* ---------- Init ---------- */

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        if (cells == null)
            cells = new Vector3Int[data.cells.Length];

        for (int i = 0; i < cells.Length; i++)
            cells[i] = (Vector3Int)data.cells[i];
    }

    /* ---------- Update ---------- */

    private void Update()
    {
        board.Clear(this);
        lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q)) Rotate(-1);
        else if (Input.GetKeyDown(KeyCode.E)) Rotate(1);

        if (Input.GetKeyDown(KeyCode.Space))
            HardDrop();

        if (Time.time > moveTime)
            HandleMoveInputs();

        if (Time.time > stepTime)
            Step();

        board.Set(this);
    }

    /* ---------- Movement ---------- */

    private void HandleMoveInputs()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (Move(Vector2Int.down))
                stepTime = Time.time + stepDelay;
        }

        if (Input.GetKey(KeyCode.A))
            Move(Vector2Int.left);
        else if (Input.GetKey(KeyCode.D))
            Move(Vector2Int.right);
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;
        Move(Vector2Int.down);

        if (lockTime >= lockDelay)
            Lock();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f;
        }

        return valid;
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down)) { }
        Lock();
    }

    /* ---------- Lock ---------- */

    private void Lock()
    {
        board.Set(this);

        int clearedLines = board.ClearLines();
        GameManager.Instance.OnLinesCleared(clearedLines);
        GameManager.Instance.SpawnFromNext();
    }   // ⭐ 이 중괄호가 빠져있었음

    /* ---------- Rotation ---------- */

    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];
            int x, y;

            if (data.tetromino == Tetromino.I || data.tetromino == Tetromino.O)
            {
                cell.x -= 0.5f;
                cell.y -= 0.5f;
                x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
            }
            else
            {
                x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            if (Move(data.wallKicks[wallKickIndex, i]))
                return true;
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int index = rotationIndex * 2;
        if (rotationDirection < 0) index--;
        return Wrap(index, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
            return max - (min - input) % (max - min);
        return min + (input - min) % (max - min);
    }
}