using System.Collections.Generic;
using System.Linq;

namespace Magic
{
    public class Magic : IMagic
    {
        public MagicData Data { get; private set; }

        private readonly ITargetSelector _targetSelector;
        private readonly List<IMagicCondition> _conditions;
        private readonly List<MagicEffectBase> _effects;

        /// <summary>
        /// MagicData에 설정된 Target, Condition, Effect 데이터를
        /// 실제 전투에서 사용할 런타임 객체로 변환합니다.
        /// </summary>
        public Magic(MagicData data)
        {
            Data = data;

            // 각 ScriptableObject 설정을 독립적인 런타임 객체로 생성한다.
            _targetSelector = data.TargetSelector.CreateSelector();
            _conditions = data.Conditions
                .Select(condition => condition.CreateCondition())
                .ToList();
            _effects = data.Effects
                .Select(effect => effect.CreateEffect())
                .ToList();

            // ... RuntimeData 및 쿨타임 초기화 생략 ...
        }

        /// <summary>
        /// 설정된 TargetSelector를 이용해 현재 마법의 대상을 결정합니다.
        /// </summary>
        public IReadOnlyList<Character> ResolveTargets(TargetSelectRequest request)
        {
            return _targetSelector.SelectTargets(request);
        }

        /// <summary>
        /// 등록된 모든 Condition을 순서대로 검사하고 마법 사용 가능 여부를 반환합니다.
        /// </summary>
        public MagicConditionCheckResult CheckConditions(MagicContext context)
        {
            foreach (IMagicCondition condition in _conditions)
            {
                // 하나라도 만족하지 못하면 해당 마법은 사용할 수 없다.
                if (!condition.IsSatisfied(context))
                {
                    return MagicConditionCheckResult.Failed(
                        condition,
                        condition.GetFailReason());
                }
            }

            return MagicConditionCheckResult.Passed();
        }

        /// <summary>
        /// 마법에 조립된 Effect들을 순서대로 적용합니다.
        /// </summary>
        public void Use(MagicContext context)
        {
            foreach (MagicEffectBase effect in _effects)
            {
                // Effect마다 필요할 경우 별도의 Target을 적용한 Context를 생성한다.
                MagicContext effectContext = CreateEffectContext(context, effect);

                if (effectContext == null)
                    continue;

                // ... 전투 연출 및 Feedback 처리 생략 ...

                effect.Apply(effectContext);
            }
        }

        // ... Effect별 Target Override를 처리하는 CreateEffectContext 구현 생략 ...
    }
}
