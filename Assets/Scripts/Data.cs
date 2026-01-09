using System.Collections.Generic;
using UnityEngine;

// 테트리스에서 사용하는 공용 데이터 모음 클래스
// (블록 모양, 회전 계산, WallKick 정보)
public static class Data
{
    // 90도 회전에 사용되는 cos, sin 값
    public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);

    // 2D 회전 행렬 (x', y' 계산용)
    public static readonly float[] RotationMatrix = new float[]
    {
        cos, sin,
       -sin, cos
    };

    // 각 테트로미노를 구성하는 기본 셀 좌표 정보
    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells
        = new Dictionary<Tetromino, Vector2Int[]>()
    {
        // I 모양
        { Tetromino.I, new Vector2Int[]
            {
                new Vector2Int(-1, 1),
                new Vector2Int( 0, 1),
                new Vector2Int( 1, 1),
                new Vector2Int( 2, 1)
            }
        },

        // J 모양
        { Tetromino.J, new Vector2Int[]
            {
                new Vector2Int(-1, 1),
                new Vector2Int(-1, 0),
                new Vector2Int( 0, 0),
                new Vector2Int( 1, 0)
            }
        },

        // L 모양
        { Tetromino.L, new Vector2Int[]
            {
                new Vector2Int( 1, 1),
                new Vector2Int(-1, 0),
                new Vector2Int( 0, 0),
                new Vector2Int( 1, 0)
            }
        },

        // O 모양 (정사각형)
        { Tetromino.O, new Vector2Int[]
            {
                new Vector2Int( 0, 1),
                new Vector2Int( 1, 1),
                new Vector2Int( 0, 0),
                new Vector2Int( 1, 0)
            }
        },

        // S 모양
        { Tetromino.S, new Vector2Int[]
            {
                new Vector2Int( 0, 1),
                new Vector2Int( 1, 1),
                new Vector2Int(-1, 0),
                new Vector2Int( 0, 0)
            }
        },

        // T 모양
        { Tetromino.T, new Vector2Int[]
            {
                new Vector2Int( 0, 1),
                new Vector2Int(-1, 0),
                new Vector2Int( 0, 0),
                new Vector2Int( 1, 0)
            }
        },

        // Z 모양
        { Tetromino.Z, new Vector2Int[]
            {
                new Vector2Int(-1, 1),
                new Vector2Int( 0, 1),
                new Vector2Int( 0, 0),
                new Vector2Int( 1, 0)
            }
        },
    };

    // I 테트로미노 전용 WallKick 데이터
    private static readonly Vector2Int[,] WallKicksI = new Vector2Int[,]
    {
        // 회전 시 벽에 걸렸을 때 시도할 이동 값들
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) },
    };

    // J, L, O, S, T, Z 공용 WallKick 데이터
    private static readonly Vector2Int[,] WallKicksJLOSTZ = new Vector2Int[,]
    {
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) },
    };

    // 테트로미노 종류별로 사용할 WallKick 데이터 매핑
    public static readonly Dictionary<Tetromino, Vector2Int[,]> WallKicks
        = new Dictionary<Tetromino, Vector2Int[,]>()
    {
        { Tetromino.I, WallKicksI },
        { Tetromino.J, WallKicksJLOSTZ },
        { Tetromino.L, WallKicksJLOSTZ },
        { Tetromino.O, WallKicksJLOSTZ },
        { Tetromino.S, WallKicksJLOSTZ },
        { Tetromino.T, WallKicksJLOSTZ },
        { Tetromino.Z, WallKicksJLOSTZ },
    };
}