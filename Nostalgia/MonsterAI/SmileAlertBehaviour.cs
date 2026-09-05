using UnityEngine;

public class SmileAlertBehaviour : MobStateBehaviour
{
    private const float TARGET_CHANGE_WAIT_TIME = 3f;
    private const float MAX_ALERT_DURATION = 10f;

    [SerializeField] private SoundEvent m_soundEvent;

    private Vector3 m_previousSoundPosition;
    private float m_targetChangeTimer;

    // ... 기타 세부 구현 생략 ...

    /// <summary>
    /// Alert 상태 진입 시 상태값과 탐색 타이머를 초기화하고
    /// 상태에 맞는 애니메이션으로 전환한다.
    /// </summary>
    protected override void OnEnterState()
    {
        m_mobAI.CurrentState = MobState.Alert;
        m_targetChangeTimer = TARGET_CHANGE_WAIT_TIME;

        // 네트워크 상의 모든 클라이언트에 Alert 애니메이션을 동기화한다.
        SetAnimatorIntRpc("CurrentState", (int)MobState.Alert);
    }

    /// <summary>
    /// 감지된 소리의 위치를 추적하며 탐색하고,
    /// 제한 시간이 지나면 Alert 상태를 종료한다.
    /// </summary>
    protected override void OnFixedUpdate()
    {
        // AI의 이동과 상태 판단은 State Authority에서만 수행한다.
        if (!HasStateAuthority)
        {
            return;
        }

        // 탐색 시간이 초과되면 Alert 상태를 종료한다.
        if (Machine.StateTime > MAX_ALERT_DURATION)
        {
            Machine.TryDeactivateState(StateId);
            return;
        }

        m_targetChangeTimer += Runner.DeltaTime;

        // 새로운 소리를 감지하면 해당 위치를 NavMesh 이동 목표로 설정한다.
        if (m_previousSoundPosition != m_soundEvent.position)
        {
            m_previousSoundPosition = m_soundEvent.position;
            m_mobAI.SetNavMeshDestination(m_soundEvent.position);
            return;
        }

        // 목표 지점에 도착한 뒤 일정 시간이 지나면 다시 순찰을 시작한다.
        if (m_targetChangeTimer < TARGET_CHANGE_WAIT_TIME ||
            m_mobAI.NavMeshRemainingDistance > 1.0f)
        {
            return;
        }

        m_targetChangeTimer = 0;
        m_mobAI.SetNextPatrolPoint();
        SetAnimatorTriggerRpc("Stop");
    }

    /// <summary>
    /// Alert 상태 종료 시 처리한 소리 감지 이벤트를 초기화한다.
    /// </summary>
    protected override void OnExitState()
    {
        // 처리한 소리 이벤트를 초기화한다.
        m_soundEvent.soundFlag = false;
    }

    // ... 기타 세부 구현 생략 ...
}
