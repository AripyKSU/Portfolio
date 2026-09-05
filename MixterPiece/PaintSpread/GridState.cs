using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 Grid의 물감 상태와 Cell 사이 벽 정보를 관리합니다.
/// PaintSpreadCalculator는 CanMove를 통해 벽을 통과하지 않는 방향만 탐색합니다.
/// </summary>
public sealed class GridState
{
    // Cell 사이 벽을 빠르게 조회하기 위한 집합.
    private readonly HashSet<Vector2Int> walls;

    public int Width { get; }
    public int Height { get; }

    // ... PaintState 배열, 생성자, GetPaint/SetPaint 등 상태 관리 코드 생략 ...

    /// <summary>
    /// 서로 인접한 두 Cell 사이로 이동할 수 있는지 확인합니다.
    /// Grid 범위를 벗어나거나 사이에 벽이 있으면 이동할 수 없습니다.
    /// </summary>
    public bool CanMove(Vector2Int from, Vector2Int to)
    {
        if (!IsInside(from) || !IsInside(to))
            return false;

        Vector2Int delta = to - from;

        // 상하좌우로 한 칸 떨어진 Cell만 이동 대상으로 인정한다.
        if (Mathf.Abs(delta.x) + Mathf.Abs(delta.y) != 1)
            return false;

        /*
         * Cell 좌표를 2배 좌표계로 표현하면
         * 두 인접 Cell 좌표의 합이 사이 벽의 위치가 된다.
         */
        Vector2Int wallPosition = from + to;

        return !walls.Contains(wallPosition);
    }

    /// <summary>
    /// 지정한 Cell에서 한 방향으로 물감이 확산될 수 있는지 확인합니다.
    /// </summary>
    public bool CanMove(Vector2Int position, GridDirection direction)
    {
        Vector2Int destination = position + direction.ToOffset();
        return CanMove(position, destination);
    }

    // ... IsInside 및 벽 좌표 검증 구현 생략 ...
}