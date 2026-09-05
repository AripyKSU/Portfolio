using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BFS로 계산된 Cell 하나의 확산 결과와 실행 전후 상태를 보관합니다.
/// </summary>
public sealed class PaintSpreadCellStep
{
    public Vector2Int Position { get; }
    public int Distance { get; }
    public PaintIncomingDirection IncomingDirections { get; }

    // Command Undo에서 사용하는 실행 전 상태.
    public PaintState PreviousState { get; }

    // 확산 계산으로 결정된 실행 후 상태.
    public PaintState ResultState { get; }

    public PaintSpreadCellStep(
        Vector2Int position,
        int distance,
        PaintIncomingDirection incomingDirections,
        PaintState previousState,
        PaintState resultState)
    {
        Position = position;
        Distance = distance;
        IncomingDirections = incomingDirections;
        PreviousState = previousState;
        ResultState = resultState;
    }
}

/// <summary>
/// 같은 BFS 최단거리에 있는 Cell들을 하나의 연출 단위로 묶습니다.
/// </summary>
public sealed class PaintSpreadWave
{
    public int Distance { get; }
    public IReadOnlyList<PaintSpreadCellStep> Steps { get; }

    public PaintSpreadWave(
        int distance,
        IReadOnlyList<PaintSpreadCellStep> steps)
    {
        Distance = distance;
        Steps = steps;
    }

    // ... 원본에서는 외부 변경을 막기 위해 불변 Collection으로 복사 ...
}

/// <summary>
/// 한 번의 물감통 사용으로 발생하는 전체 확산 결과를
/// 거리별 Wave 순서로 보관합니다.
/// </summary>
public sealed class PaintApplicationPlan
{
    public IReadOnlyList<PaintSpreadWave> Waves { get; }

    public PaintApplicationPlan(
        IReadOnlyList<PaintSpreadWave> waves)
    {
        Waves = waves;
    }

    // ... 원본에서는 외부 변경을 막기 위해 불변 Collection으로 복사 ...
}