using System.Collections.Generic;
using UnityEngine;

namespace Magic
{
    [CreateAssetMenu(menuName = "Status/Status Data")]
    public class StatusData : ScriptableObject
    {
        public string StatusId;
        public string DisplayName;

        /// <summary>
        /// 상태이상이 가질 수 있는 최대 Stack입니다.
        /// </summary>
        public int MaxStack = 99;

        /// <summary>
        /// 상태이상에 조합할 행동 Logic 목록입니다.
        /// 각 LogicData는 런타임에서 실제 StatusEffectLogicBase로 변환됩니다.
        /// </summary>
        public List<StatusEffectLogicData> LogicDatas;

        // ... Icon, Tooltip 등 UI 데이터 생략 ...
    }
}
