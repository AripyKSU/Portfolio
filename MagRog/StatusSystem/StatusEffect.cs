using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magic
{
    /// <summary>
    /// 캐릭터가 실제 전투 중 보유하는 런타임 상태이상입니다.
    /// StatusData의 설정을 기반으로 Logic을 생성하고 현재 Stack을 관리합니다.
    /// </summary>
    public class StatusEffect
    {
        private readonly StatusEffectController controller;
        private readonly List<StatusEffectLogicBase> logics;

        public StatusData Data { get; }
        public Character Owner { get; }
        public Character Source { get; }

        public int Stack { get; private set; }
        public bool IsExpired => Stack <= 0;

        public StatusEffect(
            StatusData data,
            Character owner,
            Character source,
            int stack,
            StatusEffectController controller)
        {
            Data = data;
            Owner = owner;
            Source = source;
            this.controller = controller;

            // Stack은 StatusData에 정의된 최대치를 넘지 않도록 제한한다.
            Stack = Mathf.Clamp(stack, 1, data.MaxStack);

            // SO에 조합된 LogicData를 실제 전투에서 사용할 Logic 객체로 변환한다.
            logics = data.LogicDatas
                .Where(logicData => logicData != null)
                .Select(logicData => logicData.CreateLogic())
                .Where(logic => logic != null)
                .ToList();
        }

        /// <summary>
        /// 상태 Stack 감소 요청을 Controller에 위임합니다.
        /// 실제 목록 변경과 만료 처리는 Controller가 담당합니다.
        /// </summary>
        public void ReduceStack(int amount)
        {
            controller.ReduceStack(this, amount);
        }

        internal void SetStack(int stack)
        {
            Stack = Mathf.Clamp(stack, 0, Data.MaxStack);
        }

        /// <summary>
        /// 조합된 Logic들을 검사해 상태 소유자가 행동 가능한지 판단합니다.
        /// </summary>
        public bool CanAct()
        {
            foreach (StatusEffectLogicBase logic in logics)
            {
                if (!logic.CanAct(this))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 상태가 처음 적용되었음을 모든 Logic에 전달합니다.
        /// </summary>
        public void OnApplied()
        {
            foreach (StatusEffectLogicBase logic in logics)
                logic.OnApplied(this);
        }

        /// <summary>
        /// 상태가 제거되었음을 모든 Logic에 전달합니다.
        /// </summary>
        public void OnRemoved()
        {
            foreach (StatusEffectLogicBase logic in logics)
                logic.OnRemoved(this);
        }

        /// <summary>
        /// 턴 종료 이벤트를 조합된 모든 Logic에 전달합니다.
        /// </summary>
        public void OnTurnEnd()
        {
            foreach (StatusEffectLogicBase logic in logics)
                logic.OnTurnEnd(this);
        }

        /// <summary>
        /// 각 Logic의 피해 보정 결과를 순차적으로 누적하여 반환합니다.
        /// </summary>
        public int ModifyIncomingDamage(
            int damage,
            DamageCalculationMode mode,
            MagicContext context)
        {
            int result = damage;

            foreach (StatusEffectLogicBase logic in logics)
                result = logic.ModifyIncomingDamage(this, result, mode, context);

            return result;
        }

        /// <summary>
        /// 새로 들어오는 상태이상 Stack에 각 Logic의 보정값을 순차적으로 적용합니다.
        /// </summary>
        public int ModifyIncomingStatusStack(
            StatusData incomingStatusData,
            Character source,
            int stack)
        {
            int result = stack;

            foreach (StatusEffectLogicBase logic in logics)
            {
                result = logic.ModifyIncomingStatusStack(
                    this,
                    incomingStatusData,
                    source,
                    result);
            }

            return result;
        }

        /// <summary>
        /// 상태 부여가 완료된 뒤 반응해야 하는 Logic에 결과를 전달합니다.
        /// </summary>
        public void OnAfterReceiveStatus(
            StatusData incomingStatusData,
            Character source,
            int appliedStack)
        {
            foreach (StatusEffectLogicBase logic in logics)
            {
                logic.OnAfterReceiveStatus(
                    this,
                    incomingStatusData,
                    source,
                    appliedStack);
            }
        }

        // ... OnTurnStart, 공격/피격 후 이벤트, 상태 부여량 보정, 사망 이벤트 등 추가 Hook 생략 ...
    }
}
