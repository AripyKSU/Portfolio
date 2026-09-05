using UnityEngine;

/// <summary>
/// 물감통 한 번의 사용으로 발생하는 여러 상태 변경을
/// 하나의 실행/복원 단위로 관리합니다.
/// </summary>
public sealed class PaintBucketUseCommand : ICommand
{
    private readonly int bucketId;
    private readonly PaintBucket bucket;
    private readonly Vector2Int origin;

    private readonly GridState gridState;
    private readonly GridView gridView;
    private readonly PaintBucketController bucketController;
    private readonly PaintSpreadCalculator spreadCalculator;

    private bool isExecuted;

    /// <summary>
    /// 실행 시 계산된 확산 결과와 각 Cell의 이전/결과 상태를 보관합니다.
    /// </summary>
    public PaintApplicationPlan Plan { get; private set; }

    // ... 생성자 및 의존 객체 초기화 코드 생략 ...

    /// <summary>
    /// 물감 확산 결과를 계산하고,
    /// Grid 상태 변경과 물감통 소비를 하나의 행동으로 실행합니다.
    /// </summary>
    public bool Execute()
    {
        if (isExecuted)
            return false;

        // 실행 전 상태와 최종 상태가 포함된 확산 계획을 계산한다.
        PaintApplicationPlan calculatedPlan = spreadCalculator.Calculate(
            gridState,
            origin,
            bucket.Range,
            bucket.PaintType);

        // 물감통을 사용할 수 없다면 Grid 상태도 변경하지 않는다.
        if (!bucketController.Consume(bucketId))
            return false;

        Plan = calculatedPlan;

        // 계산된 최종 결과를 논리 GridState에 반영한다.
        foreach (PaintSpreadWave wave in Plan.Waves)
        {
            foreach (PaintSpreadCellStep step in wave.Steps)
            {
                gridState.SetPaint(
                    step.Position,
                    step.ResultState);
            }
        }

        isExecuted = true;
        return true;
    }

    /// <summary>
    /// 실행 전 저장된 Cell 상태와 사용한 물감통을 함께 복원합니다.
    /// </summary>
    public void Undo()
    {
        if (!isExecuted || Plan == null)
            return;

        // 확산 계획에 저장된 각 Cell의 실행 이전 상태를 복원한다.
        foreach (PaintSpreadWave wave in Plan.Waves)
        {
            foreach (PaintSpreadCellStep step in wave.Steps)
            {
                gridState.SetPaint(
                    step.Position,
                    step.PreviousState);

                gridView.SetCellPaint(
                    step.Position,
                    step.PreviousState);
            }
        }

        // 동일한 Command에서 소비했던 물감통까지 함께 되돌린다.
        bucketController.Restore(bucketId);

        isExecuted = false;
    }
}
