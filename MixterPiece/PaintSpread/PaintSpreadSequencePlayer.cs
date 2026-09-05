using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 계산된 PaintApplicationPlan을 거리별 Wave 순서로 화면에 재생합니다.
/// BFS 계산과 시각적 연출의 책임을 분리합니다.
/// </summary>
public sealed class PaintSpreadSequencePlayer
{
    private readonly GridView gridView;

    // ... 이펙트 설정, Object Pool, 취소 처리 필드 생략 ...

    /// <summary>
    /// 같은 BFS 거리의 Cell을 함께 연출한 뒤 다음 Wave로 진행합니다.
    /// </summary>
    public IEnumerator Play(
        PaintApplicationPlan plan,
        PaintType paintType,
        Action completed)
    {
        if (plan == null)
            throw new ArgumentNullException(nameof(plan));

        // ... Clear Paint 전용 처리 생략 ...

        foreach (PaintSpreadWave wave in plan.Waves)
        {
            // 같은 거리의 Cell 이펙트를 한 번에 시작한다.
            List<GameObject> waveEffects = new();
            SpawnWaveEffects(wave, waveEffects);

            // 현재 Wave 연출이 진행된 뒤 다음 거리 Wave로 넘어간다.
            if (waveEffects.Count > 0)
            {
                float duration = GetWaveAdvanceSeconds(waveEffects);
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }

            ApplyWaveResults(wave);
        }

        completed?.Invoke();
    }

    /// <summary>
    /// 같은 거리 Wave의 모든 Cell 결과를 화면에 함께 반영합니다.
    /// </summary>
    private void ApplyWaveResults(PaintSpreadWave wave)
    {
        foreach (PaintSpreadCellStep step in wave.Steps)
        {
            gridView.SetCellPaint(
                step.Position,
                step.ResultState);
        }
    }

    // ... Particle Effect 생성, 재생 시간 계산, Object Pool 구현 생략 ...
}