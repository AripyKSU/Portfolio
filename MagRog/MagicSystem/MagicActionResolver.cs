using System.Collections.Generic;

namespace Magic
{
    /// <summary>
    /// 덱의 마법을 순서대로 검사해 현재 턴에 사용할 수 있는 마법을 결정합니다.
    /// </summary>
    public class MagicActionResolver
    {
        /// <summary>
        /// 각 마법의 Target을 결정하고 Condition을 검사하여
        /// 현재 턴에 실행 가능한 첫 번째 마법의 Context를 반환합니다.
        /// </summary>
        public MagicActionResolveResult Resolve(
            Character caster,
            StageManager stage,
            BattleCombatantQuery combatantQuery,
            IReadOnlyList<IMagic> magics)
        {
            if (caster == null)
                return MagicActionResolveResult.CreateFail("시전자가 없습니다.");

            if (stage == null)
                return MagicActionResolveResult.CreateFail("스테이지 정보가 없습니다.");

            if (magics == null || magics.Count == 0)
                return MagicActionResolveResult.CreateFail("덱에 마법이 없습니다.");

            // 덱의 마법을 순서대로 검사해 현재 사용할 수 있는 마법을 탐색한다.
            foreach (IMagic magic in magics)
            {
                if (magic == null)
                    continue;

                TargetSelectRequest request =
                    new TargetSelectRequest(caster, stage, combatantQuery, magic);

                // 각 마법의 TargetSelector를 통해 이번 실행의 대상을 결정한다.
                IReadOnlyList<Character> targets = magic.ResolveTargets(request);
                if (targets == null)
                    continue;

                MagicContext context =
                    new MagicContext(caster, targets, stage, combatantQuery, magic);

                // 조립된 Condition을 검사해 현재 사용 가능한 마법인지 판단한다.
                MagicConditionCheckResult checkResult = magic.CheckConditions(context);
                if (!checkResult.Success)
                    continue;

                return MagicActionResolveResult.CreateSuccess(context);
            }

            return MagicActionResolveResult.CreateFail(
                "이번 턴 실행 가능한 마법이 없습니다.");
        }

        // ... 이전 Target 선택 헬퍼 등 비핵심 구현 생략 ...
    }
}
