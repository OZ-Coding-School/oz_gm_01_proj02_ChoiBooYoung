using UnityEngine;

public class Piece : MonoBehaviour
{
    // 현재 블록이 놓일 게임 보드
    public Board board { get; private set; }

    // 이 블록의 테트로미노 정보 (모양, 타일, 회전 데이터 등)
    public TetrominoData data { get; private set; }

    // 블록을 구성하는 각 칸들의 위치 정보
    public Vector3Int[] cells { get; private set; }

    // 보드 상에서 블록의 기준 위치
    public Vector3Int position { get; private set; }

    // 현재 회전 상태 (0~3)
    public int rotationIndex { get; private set; }

    // 자동으로 한 칸 내려가는 시간 간격
    public float stepDelay = 1f;

    // 좌우 이동 / 소프트 드롭 시 입력 딜레이
    public float moveDelay = 0.1f;

    // 바닥에 닿은 후 고정되기까지 시간(바닥에 닿아도 잠깐 좌,우이동이나 회전 가능)
    public float lockDelay = 0.5f;

    // 다음 자동 낙하 시간
    private float stepTime;

    // 다음 이동 가능 시간
    private float moveTime;

    // 블록이 멈춰있는 시간 누적
    private float lockTime;

    // 블록 생성 시 초기화 함수
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.data = data;
        this.board = board;
        this.position = position;

        // 회전 상태 초기화
        rotationIndex = 0;

        // 자동 낙하 및 이동 시간 설정
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;

        // 고정 타이머 초기화
        lockTime = 0f;

        // 셀 배열이 없으면 새로 생성
        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        // 테트로미노 데이터에 있는 셀 정보를 복사
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        // 이전 프레임에 그려진 블록을 보드에서 지운다
        board.Clear(this);

        // 블록이 움직이지 않고 머문 시간을 누적
        lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 반시계 방향 회전
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            // 시계 방향 회전
            Rotate(1);
        }

        // 스페이스바를 누르면 즉시 바닥까지 떨어뜨림
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        // 이동 딜레이가 지난 경우에만 입력 허용
        if (Time.time > moveTime)
        {
            HandleMoveInputs();
        }

        // 일정 시간이 지나면 자동으로 한 칸 아래로 이동
        if (Time.time > stepTime)
        {
            Step();
        }

        // 현재 위치에 블록을 다시 그린다
        board.Set(this);
    }

    // 좌우 이동 및 소프트 드롭 입력 처리
    private void HandleMoveInputs()
    {
        // S 키를 누르면 소프트 드롭 (천천히 아래 이동)
        if (Input.GetKey(KeyCode.S))
        {
            if (Move(Vector2Int.down))
            {
                // 자동 낙하와 중복되지 않도록 시간 갱신
                stepTime = Time.time + stepDelay;
            }
        }

        // A / D 키로 좌우 이동
        if (Input.GetKey(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
    }

    // 자동 낙하 처리 함수
    private void Step()
    {
        // 다음 자동 낙하 시간 설정
        stepTime = Time.time + stepDelay;

        // 한 칸 아래로 이동 시도
        Move(Vector2Int.down);

        // 블록이 오래 멈춰 있으면 고정
        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    // 하드 드롭 처리
    private void HardDrop()
    {
        // 더 이상 내려갈 수 없을 때까지 계속 아래로 이동
        while (Move(Vector2Int.down))
        {
            continue;
        }

        // 즉시 고정
        Lock();
    }

    // 블록 고정 처리
    private void Lock()
    {
        // 보드에 최종 위치로 블록 고정
        board.Set(this);

        // 완성된 줄 제거
        board.ClearLines();

        // 새로운 블록 생성
        board.SpawnPiece();
    }

    // 실제 이동 처리 함수
    private bool Move(Vector2Int translation)
    {
        // 이동할 새로운 위치 계산
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        // 해당 위치가 보드 안에서 유효한지 검사
        bool valid = board.IsValidPosition(this, newPosition);

        // 이동이 가능할 때만 위치 갱신
        if (valid)
        {
            position = newPosition;

            // 다음 이동 딜레이 설정
            moveTime = Time.time + moveDelay;

            // 움직였으므로 고정 타이머 리셋
            lockTime = 0f;
        }

        return valid;
    }

    // 회전 처리 함수
    private void Rotate(int direction)
    {
        // 회전 실패 시 되돌리기 위해 기존 회전값 저장
        int originalRotation = rotationIndex;

        // 회전 인덱스 변경 (0~3 범위 유지)
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);

        // 실제 셀 좌표 회전
        ApplyRotationMatrix(direction);

        // 벽킥 테스트 실패 시 회전 취소
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    // 회전 행렬을 이용해 셀 좌표 계산
    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];
            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // I, O 블록은 중심 보정 후 회전
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    // 일반 블록 회전
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    // 벽에 걸렸을 때 회전을 보정하는 테스트
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            // 보정 이동이 성공하면 회전 성공
            if (Move(translation))
            {
                return true;
            }
        }

        // 모든 보정 실패
        return false;
    }

    // 현재 회전에 맞는 벽킥 인덱스 계산
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    // 값이 범위를 넘지 않도록 순환 처리
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}