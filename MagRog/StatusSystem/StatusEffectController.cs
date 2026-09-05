using System;
using System.Collections.Generic;
using UnityEngine;

namespace Magic
{
    /// <summary>
    /// 캐릭터가 보유한 런타임 상태이상 목록을 관리하고,
    /// 상태 적용·Stack 변경·제거 시점을 중앙에서 제어합니다.
    /// </summary>
    public class StatusEffectController : MonoBehaviour
    {
        [SerializeField] private Character owner;

        private readonly List<StatusEffect> statuses = new();

        // Logic 순회 중에는 List를 직접 변경하지 않고,
        // 모든 중첩 처리가 끝난 뒤 예약된 변경을 반영한다.
        private int statusProcessingDepth;
        private bool statusChangedDuringProcessing;
        private readonly HashSet<string> pendingRemoveStatusIds = new();

        private bool IsProcessingStatuses => statusProcessingDepth > 0;

        public event Action<IReadOnlyList<StatusEffect>> StatusesChanged;
        public event Action<Character, StatusData, int> StatusApplied;
        public event Action<Character, StatusData, int> StatusStackChanged;
        public event Action<Character, StatusData> StatusRemoved;

        // ... BattleCombatantQuery 바인딩, Unity LifeCycle, 조회 API 등 생략 ...

        /// <summary>
        /// 캐릭터에게 상태이상을 부여합니다.
        /// 기존 상태가 있다면 Stack을 누적하고,
        /// 없다면 새로운 StatusEffect를 생성합니다.
        /// </summary>
        public void ApplyStatus(StatusData data, Character source, int stack)
        {
            if (data == null || owner == null)
                return;

            int modifiedStack = stack;

            // 상태를 부여하는 쪽의 Logic에 따라 최종 부여 Stack을 먼저 보정한다.
            StatusEffectController sourceStatusController =
                source == null ? null : source.GetComponent<StatusEffectController>();

            if (sourceStatusController != null)
            {
                modifiedStack = sourceStatusController.ModifyOutgoingStatusStack(
                    data,
                    owner,
                    modifiedStack);
            }

            // ... source가 보유한 Item에 의한 Stack 보정 생략 ...

            if (modifiedStack <= 0)
                return;

            /*
             * 현재 상태들의 Logic을 순회하는 동안 Stack 감소나 상태 제거 요청이
             * 발생할 수 있으므로 Processing Scope 안에서 처리한다.
             * finally에서 Scope 종료를 보장해 예외가 발생해도 지연 삭제가 남지 않도록 한다.
             */
            BeginStatusProcessing();
            try
            {
                int statusCount = statuses.Count;

                for (int i = 0; i < statusCount; i++)
                {
                    modifiedStack = statuses[i].ModifyIncomingStatusStack(
                        data,
                        source,
                        modifiedStack);
                }
            }
            finally
            {
                EndStatusProcessing();
            }

            // ... owner가 보유한 Item에 의한 Stack 보정 생략 ...

            if (modifiedStack <= 0)
                return;

            BeginStatusProcessing();
            try
            {
                StatusEffect existing = FindStatus(data.StatusId);

                // 동일한 상태가 이미 존재하면 새 객체를 만들지 않고 Stack만 누적한다.
                if (existing != null)
                {
                    AddStack(existing, modifiedStack);
                }
                else
                {
                    StatusEffect status = new StatusEffect(
                        data,
                        owner,
                        source,
                        modifiedStack,
                        this);

                    AddStatus(status);
                    StatusApplied?.Invoke(owner, status.Data, status.Stack);

                    // 새로 추가된 상태의 시작 Logic을 실행한다.
                    status.OnApplied();
                }

                // 상태 부여가 끝난 뒤 반응해야 하는 Logic에 결과를 전달한다.
                int statusCount = statuses.Count;

                for (int i = 0; i < statusCount; i++)
                {
                    statuses[i].OnAfterReceiveStatus(
                        data,
                        source,
                        modifiedStack);
                }

                // ... Item의 상태 수신 후 처리 생략 ...
            }
            finally
            {
                EndStatusProcessing();
            }
        }

        /// <summary>
        /// 상태를 제거합니다.
        /// Logic 순회 중이라면 즉시 List를 수정하지 않고 제거 요청만 예약합니다.
        /// </summary>
        public bool RemoveStatus(string statusId)
        {
            if (string.IsNullOrEmpty(statusId))
                return false;

            if (IsProcessingStatuses)
            {
                // 순회 중 컬렉션이 변경되지 않도록 실제 삭제를 뒤로 미룬다.
                pendingRemoveStatusIds.Add(statusId);
                statusChangedDuringProcessing = true;
                return FindStatus(statusId) != null;
            }

            return RemoveStatusImmediately(statusId);
        }

        /// <summary>
        /// StatusEffect가 요청한 Stack 감소를 적용합니다.
        /// Stack이 0이 되더라도 순회 중에는 즉시 제거하지 않습니다.
        /// </summary>
        internal void ReduceStack(StatusEffect status, int amount)
        {
            if (status == null || amount <= 0)
                return;

            int previous = status.Stack;
            status.SetStack(status.Stack - amount);

            if (status.Stack != previous)
            {
                StatusStackChanged?.Invoke(owner, status.Data, status.Stack - previous);
                MarkStatusesChanged();
            }
        }

        private StatusEffect FindStatus(string statusId)
        {
            for (int i = 0; i < statuses.Count; i++)
            {
                if (statuses[i].Data.StatusId == statusId)
                    return statuses[i];
            }

            return null;
        }

        private void AddStatus(StatusEffect status)
        {
            statuses.Add(status);
            MarkStatusesChanged();
        }

        private void AddStack(StatusEffect status, int amount)
        {
            if (status == null || amount <= 0)
                return;

            int previous = status.Stack;
            status.SetStack(status.Stack + amount);

            if (status.Stack != previous)
            {
                StatusStackChanged?.Invoke(owner, status.Data, status.Stack - previous);
                MarkStatusesChanged();
            }
        }

        /// <summary>
        /// 상태 Logic 순회가 시작되었음을 기록합니다.
        /// 상태 처리 안에서 또 다른 상태 처리가 발생할 수 있어 bool 대신 depth를 사용합니다.
        /// </summary>
        private void BeginStatusProcessing()
        {
            statusProcessingDepth++;
        }

        /// <summary>
        /// 모든 중첩된 상태 처리가 끝난 시점에
        /// 예약된 삭제와 만료된 상태를 한 번에 정리합니다.
        /// </summary>
        private void EndStatusProcessing()
        {
            statusProcessingDepth = Mathf.Max(0, statusProcessingDepth - 1);

            // 아직 바깥쪽 상태 처리 Scope가 남아 있으면 List를 수정하지 않는다.
            if (statusProcessingDepth > 0)
                return;

            if (!statusChangedDuringProcessing)
                return;

            statusChangedDuringProcessing = false;
            RemovePendingOrExpiredStatuses();
            NotifyStatusesChanged();
        }

        /// <summary>
        /// 상태 목록 변경을 기록합니다.
        /// 순회 중이면 정리를 지연하고, 안전한 시점이면 즉시 만료 상태를 정리합니다.
        /// </summary>
        private void MarkStatusesChanged()
        {
            if (IsProcessingStatuses)
            {
                statusChangedDuringProcessing = true;
                return;
            }

            RemovePendingOrExpiredStatuses();
            NotifyStatusesChanged();
        }

        /// <summary>
        /// Stack이 모두 소모되었거나 제거 예약된 상태를 안전한 시점에 실제로 제거합니다.
        /// </summary>
        private void RemovePendingOrExpiredStatuses()
        {
            // 제거 중 인덱스가 밀리지 않도록 뒤에서부터 순회한다.
            for (int i = statuses.Count - 1; i >= 0; i--)
            {
                StatusEffect status = statuses[i];

                bool shouldRemove =
                    status == null ||
                    status.Data == null ||
                    status.IsExpired ||
                    pendingRemoveStatusIds.Contains(status.Data.StatusId);

                if (!shouldRemove)
                    continue;

                RemoveStatusAtCore(i);
            }

            pendingRemoveStatusIds.Clear();
        }

        private bool RemoveStatusImmediately(string statusId)
        {
            for (int i = statuses.Count - 1; i >= 0; i--)
            {
                StatusEffect status = statuses[i];

                if (status == null || status.Data == null || status.Data.StatusId != statusId)
                    continue;

                RemoveStatusAtCore(i);
                NotifyStatusesChanged();
                return true;
            }

            return false;
        }

        private void RemoveStatusAtCore(int index)
        {
            StatusEffect status = statuses[index];

            // 실제 제거 직전에 상태 종료 Logic과 외부 이벤트를 처리한다.
            status.OnRemoved();
            StatusRemoved?.Invoke(owner, status.Data);
            statuses.RemoveAt(index);
        }

        private void NotifyStatusesChanged()
        {
            StatusesChanged?.Invoke(statuses);
        }

        // ... Damage/Turn 이벤트 전달 및 기타 전투 Hook 생략 ...
    }
}
