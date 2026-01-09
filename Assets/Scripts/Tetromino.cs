using UnityEngine;
using UnityEngine.Tilemaps;

// 테트로미노 종류 정의 (I, J, L, O, S, T, Z)
public enum Tetromino
{
    I, J, L, O, S, T, Z
}

// 테트로미노 하나의 데이터를 담는 구조체
// 인스펙터에서 설정 가능하도록 Serializable 사용
[System.Serializable]
public struct TetrominoData
{
    // 해당 테트로미노에 사용할 타일
    public Tile tile;

    // 테트로미노 종류
    public Tetromino tetromino;

    // 테트로미노를 구성하는 기본 셀 좌표들
    public Vector2Int[] cells { get; private set; }

    // 회전 시 벽에 걸렸을 때 보정하는 벽킥 데이터
    public Vector2Int[,] wallKicks { get; private set; }

    // Data 클래스에 정의된 값을 불러와 초기화
    public void Initialize()
    {
        // 선택된 테트로미노의 기본 모양 좌표 설정
        cells = Data.Cells[tetromino];

        // 선택된 테트로미노의 벽킥 규칙 설정
        wallKicks = Data.WallKicks[tetromino];
    }
}