using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 벽을 고려한 최단 거리 BFS로 물감 확산 계획을 계산합니다.
/// BFS 탐색 결과를 거리별 Wave로 묶어 계산과 연출 순서를 분리합니다.
/// </summary>
public sealed class PaintSpreadCalculator
{
    private readonly struct SearchNode
    {
        public Vector2Int Position { get; }
        public int Distance { get; }

        public SearchNode(Vector2Int position, int distance)
        {
            Position = position;
            Distance = distance;
        }
    }

    // 물감은 대각선 없이 상하좌우 네 방향으로만 확산한다.
    private static readonly GridDirection[] Directions =
    {
        GridDirection.UP,
        GridDirection.RIGHT,
        GridDirection.DOWN,
        GridDirection.LEFT,
    };

    /// <summary>
    /// 시작 Cell을 거리 0으로 두고 BFS를 수행하여
    /// 벽을 통과하지 않는 최단거리 확산 계획을 생성합니다.
    /// </summary>
    public PaintApplicationPlan Calculate(
        GridState gridState,
        Vector2Int origin,
        int range,
        PaintType paintType)
    {
        // ... null, 좌표, range 검증 생략 ...

        int cellCount = gridState.Width * gridState.Height;
        int maxDistance = range - 1;

        // 각 Cell까지 발견된 최단거리를 저장한다.
        int[] distances = new int[cellCount];

        // 같은 최단거리로 여러 방향에서 도달할 경우 유입 방향을 함께 저장한다.
        PaintIncomingDirection[] incomingDirections =
            new PaintIncomingDirection[cellCount];

        // BFS 결과를 같은 거리끼리 묶어 Wave로 만들기 위한 버킷.
        List<Vector2Int>[] positionsByDistance =
            new List<Vector2Int>[maxDistance + 1];

        Array.Fill(distances, -1);
        for (int distance = 0; distance <= maxDistance; distance++)
        {
            positionsByDistance[distance] = new List<Vector2Int>();
        }

        Queue<SearchNode> queue = new();

        int originIndex = GridIndexUtility.ToIndex(
            origin,
            gridState.Width,
            gridState.Height);

        distances[originIndex] = 0;
        positionsByDistance[0].Add(origin);

        // 시작 Cell을 BFS Queue에 넣는다.
        queue.Enqueue(new SearchNode(origin, 0));

        while (queue.Count > 0)
        {
            SearchNode current = queue.Dequeue();

            // 최대 확산 거리에 도달한 Cell은 결과에 포함하되 더 확장하지 않는다.
            if (current.Distance >= maxDistance)
                continue;

            foreach (GridDirection direction in Directions)
            {
                // GridState에서 범위와 벽을 확인해 이동 가능한 방향만 탐색한다.
                if (!gridState.CanMove(current.Position, direction))
                    continue;

                Vector2Int next = current.Position + direction.ToOffset();
                int nextDistance = current.Distance + 1;
                int nextIndex = GridIndexUtility.ToIndex(
                    next,
                    gridState.Width,
                    gridState.Height);

                PaintIncomingDirection incoming =
                    ToIncomingDirection(direction);

                // 처음 방문한 Cell은 BFS 특성상 현재 거리가 최단거리다.
                if (distances[nextIndex] < 0)
                {
                    distances[nextIndex] = nextDistance;
                    incomingDirections[nextIndex] = incoming;
                    positionsByDistance[nextDistance].Add(next);

                    queue.Enqueue(new SearchNode(next, nextDistance));
                    continue;
                }

                // 더 긴 경로는 버리고, 같은 최단거리로 들어온 방향만 누적한다.
                if (distances[nextIndex] == nextDistance)
                {
                    incomingDirections[nextIndex] |= incoming;
                }
            }
        }

        PaintState addedPaint = ToPaintState(paintType);
        List<PaintSpreadWave> waves = new(positionsByDistance.Length);

        // BFS 방문 순서를 그대로 연출하지 않고 같은 최단거리의 Cell을 하나의 Wave로 묶는다.
        for (int distance = 0; distance < positionsByDistance.Length; distance++)
        {
            List<PaintSpreadCellStep> steps =
                new(positionsByDistance[distance].Count);

            foreach (Vector2Int position in positionsByDistance[distance])
            {
                int index = GridIndexUtility.ToIndex(
                    position,
                    gridState.Width,
                    gridState.Height);

                PaintState previous = gridState.GetPaint(position);
                PaintState result = paintType == PaintType.Clear
                    ? PaintState.Empty
                    : previous | addedPaint;

                steps.Add(new PaintSpreadCellStep(
                    position,
                    distance,
                    incomingDirections[index],
                    previous,
                    result));
            }

            waves.Add(new PaintSpreadWave(distance, steps));
        }

        return new PaintApplicationPlan(waves);
    }

    // ... PaintType 변환 및 유입 방향 변환 헬퍼 생략 ...
}