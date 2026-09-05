# MagRog

**MagRog**는 마법 덱을 구성하고 다양한 조건과 효과를 조합해 전투를 진행하는 로그라이크 전략 게임입니다.

이 폴더에는 새로운 마법과 상태이상을 데이터 조합과 런타임 객체로 분리해 확장할 수 있도록 설계한 핵심 게임플레이 시스템 코드를 정리합니다.

## Code Samples

### [MagicSystem](./MagicSystem)
`Target / Condition / Effect`를 각각 분리하고, `MagicData`에서 필요한 요소를 조합해 하나의 마법을 구성하는 시스템입니다.

- `MagicData.cs`
  - `TargetSelector`, `Conditions`, `Effects`를 ScriptableObject 데이터로 조합
  - 설정된 데이터를 기반으로 런타임 `Magic` 객체 생성
- `Magic.cs`
  - 각 Data를 실제 `TargetSelector / Condition / Effect` 런타임 객체로 변환
  - 대상 선택, 조건 검사, Effect 실행을 하나의 마법 실행 흐름으로 관리
- `MagicActionResolver.cs`
  - 덱의 마법을 순회하며 `Target 선택 → Context 생성 → Condition 검사` 순서로 현재 턴에 실행 가능한 마법 결정

이 구조를 통해 새로운 마법을 추가할 때 기존 실행 흐름을 수정하기보다, 필요한 Target·Condition·Effect를 조합해 확장할 수 있도록 구성했습니다.

### [StatusSystem](./StatusSystem)
상태이상의 정적 설정과 실제 전투 중 동작을 분리하고, 여러 `Logic`을 조합해 다양한 상태 효과를 만들 수 있도록 구성한 시스템입니다.

- `StatusData.cs`
  - 최대 Stack과 상태에 포함할 `LogicData` 목록 정의
- `StatusEffect.cs`
  - 캐릭터가 실제 전투 중 보유하는 런타임 상태
  - `LogicData`를 실제 Logic 객체로 생성하고 Stack 및 전투 Hook 전달 관리
- `StatusEffectLogicBase.cs`
  - 행동 제한, 턴 이벤트, 피해 보정, 상태 부여량 보정 등 확장 가능한 공통 Hook 제공
- `StatusEffectController.cs`
  - 캐릭터별 상태 부여, Stack 중첩, 제거와 생명주기를 중앙에서 관리
  - 상태 Logic 순회 중 제거 요청은 즉시 List를 수정하지 않고 예약한 뒤, 모든 중첩 처리가 끝난 안전한 시점에 반영

상태이상 처리 중 다른 상태가 제거되거나 Stack이 소모될 수 있기 때문에 `statusProcessingDepth`와 예약 삭제 목록을 사용해 순회 중 컬렉션 변경을 방지했습니다.

## Tech Stack

- C#
- Unity
- ScriptableObject

## Note

- 전체 프로젝트가 아닌 구조와 설계 의도를 확인할 수 있는 핵심 코드만 선별해 포함합니다.
- 프로젝트 종속 UI, Item, 연출 코드 등은 가독성을 위해 일부 생략했습니다.
- 필요한 부분에는 XML 주석과 핵심 동작 주석을 보강했습니다.
