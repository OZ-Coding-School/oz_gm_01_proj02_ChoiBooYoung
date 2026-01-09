using UnityEngine;
using UnityEngine.Tilemaps;


// 현재 조작 중인 블록이 바닥에 닿을 위치를 미리 보여준다
public class Ghost : MonoBehaviour
{
    // 고스트 블록에 사용할 타일
    public Tile tile;

    // 실제 게임 보드
    public Board mainBoard;

    // 현재 조작 중인 블록
    public Piece trackingPiece;

    // 고스트 블록을 그릴 타일맵
    public Tilemap tilemap { get; private set; }

    // 고스트 블록을 구성하는 셀 좌표
    public Vector3Int[] cells { get; private set; }

    // 고스트 블록의 위치
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        // 자식 오브젝트에서 Tilemap 가져오기
        tilemap = GetComponentInChildren<Tilemap>();

        // 고스트 블록은 항상 4칸
        cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        // 이전 프레임의 고스트 블록 제거
        Clear();

        // 현재 블록의 모양 복사
        Copy();

        // 바닥까지 떨어질 위치 계산
        Drop();

        // 계산된 위치에 고스트 블록 표시
        Set();
    }

    // 고스트 블록을 타일맵에서 제거
    private void Clear()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    // 현재 조작 중인 블록의 셀 모양을 복사
    private void Copy()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = trackingPiece.cells[i];
        }
    }

    // 고스트 블록이 내려갈 수 있는 최하단 위치 계산
    private void Drop()
    {
        // 현재 블록의 위치 가져오기
        Vector3Int position = trackingPiece.position;

        // 현재 y 위치
        int current = position.y;

        // 보드의 최하단 y 값
        int bottom = -mainBoard.boardSize.y / 2 - 1;

        // 충돌 검사를 위해 잠시 실제 블록을 보드에서 제거
        mainBoard.Clear(trackingPiece);

        // 현재 위치부터 아래로 한 줄씩 검사
        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            // 해당 위치가 유효하면 고스트 위치 갱신
            if (mainBoard.IsValidPosition(trackingPiece, position))
            {
                this.position = position;
            }
            else
            {
                // 더 이상 내려갈 수 없으면 중단
                break;
            }
        }

        // 실제 블록을 다시 보드에 표시
        mainBoard.Set(trackingPiece);
    }

    // 계산된 위치에 고스트 블록 그리기
    private void Set()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }
}