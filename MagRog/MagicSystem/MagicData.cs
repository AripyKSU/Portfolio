using System.Collections.Generic;
using UnityEngine;

namespace Magic
{
    /// <summary>
    /// 마법을 구성하는 Target, Condition, Effect 데이터를 보관하는 ScriptableObject입니다.
    /// 설정된 조합을 기반으로 런타임 Magic 객체를 생성합니다.
    /// </summary>
    [CreateAssetMenu(menuName = "Magic/MagicData")]
    public class MagicData : ScriptableObject
    {
        public string MagicName;

        /// <summary>
        /// 마법이 기본적으로 사용할 대상 선택 규칙입니다.
        /// </summary>
        public TargetSelectorData TargetSelector;

        /// <summary>
        /// 마법 사용 전에 순서대로 검사할 조건 목록입니다.
        /// </summary>
        public List<ConditionData> Conditions;

        /// <summary>
        /// 마법 사용 시 순서대로 적용할 효과 목록입니다.
        /// </summary>
        public List<EffectData> Effects;

        // ... UI, 강화, 등급 등 부가 데이터 생략 ...

        /// <summary>
        /// 현재 ScriptableObject 설정을 기반으로 런타임 Magic 객체를 생성합니다.
        /// </summary>
        public IMagic CreateMagic()
        {
            return new Magic(this);
        }
    }
}
