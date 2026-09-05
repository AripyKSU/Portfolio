namespace Magic
{
    /// <summary>
    /// 상태이상의 실제 행동을 정의하기 위한 공통 Logic 클래스입니다.
    /// 필요한 전투 이벤트만 선택적으로 Override하여 구현합니다.
    /// </summary>
    public abstract class StatusEffectLogicBase
    {
        // 상태 생명주기
        public virtual void OnApplied(StatusEffect status) { }
        public virtual void OnRemoved(StatusEffect status) { }

        // 턴 이벤트
        public virtual void OnTurnStart(StatusEffect status) { }
        public virtual void OnTurnEnd(StatusEffect status) { }

        // 행동 제어
        public virtual bool CanAct(StatusEffect status)
        {
            return true;
        }

        // 피해량 보정
        public virtual int ModifyOutgoingDamage(
            StatusEffect status,
            int damage,
            DamageCalculationMode mode,
            MagicContext context)
        {
            return damage;
        }

        public virtual int ModifyIncomingDamage(
            StatusEffect status,
            int damage,
            DamageCalculationMode mode,
            MagicContext context)
        {
            return damage;
        }

        // 상태이상 부여량 보정
        public virtual int ModifyIncomingStatusStack(
            StatusEffect status,
            StatusData incomingStatusData,
            Character source,
            int stack)
        {
            return stack;
        }

        public virtual int ModifyOutgoingStatusStack(
            StatusEffect status,
            StatusData outgoingStatusData,
            Character target,
            int stack)
        {
            return stack;
        }

        // 상태 부여 및 전투 결과 이벤트
        public virtual void OnAfterReceiveStatus(
            StatusEffect status,
            StatusData incomingStatusData,
            Character source,
            int incomingStack) { }

        public virtual void OnAfterDealDamage(
            StatusEffect status,
            DamageResult result) { }

        public virtual void OnAfterTakeDamage(
            StatusEffect status,
            DamageResult result) { }

        // ... 사망 이벤트 등 추가 Hook 생략 ...
    }
}
