# MagRog

## 🎥 Gameplay Video

[![MagRog Gameplay Video](https://img.youtube.com/vi/ESk9JNvt00w/hqdefault.jpg)](https://youtu.be/ESk9JNvt00w)

[▶ YouTube에서 보기](https://youtu.be/ESk9JNvt00w)

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
상태의 설정과 전투 중 실제 동작을 분리하고, 필요한 행동을 조합해 다양한 상태이상을 만들 수 있도록 구성한 시스템입니다.

상태이상은 다음 흐름으로 동작합니다.

`상태 정의 → 런타임 상태 생성 → 상태별 행동 실행 → Controller에서 Stack과 생명주기 관리`

- `StatusData.cs`
  - 상태의 이름, 최대 Stack, 어떤 행동을 가질지 정의하는 데이터
- `StatusEffect.cs`
  - 캐릭터가 전투 중 실제로 보유하는 상태 인스턴스
  - 설정된 행동들을 런타임 객체로 생성하고, 턴 종료나 피해 보정 같은 전투 이벤트를 전달
- `StatusEffectLogicBase.cs`
  - 행동 제한, 피해 보정, 턴 이벤트처럼 상태가 가질 수 있는 개별 행동의 공통 구조
  - 새로운 상태 행동은 필요한 이벤트만 선택해 확장할 수 있도록 구성
- `StatusEffectController.cs`
  - 캐릭터별 상태 부여, Stack 중첩, 제거와 생명주기를 중앙에서 관리
  - 상태 처리 중 제거 요청이 발생하면 즉시 목록을 수정하지 않고 예약한 뒤, 모든 중첩 처리가 끝난 안전한 시점에 반영

이를 통해 각 상태이상은 하나의 거대한 전용 클래스로 구현하지 않고, 필요한 행동을 조합해 구성할 수 있습니다. 또한 상태 처리 중 Stack 소모나 제거가 발생해도 순회가 깨지지 않도록 변경 시점을 안전하게 관리합니다.

## Tech Stack

- C#
- Unity
- ScriptableObject

## Note

- 전체 프로젝트가 아닌 구조와 설계 의도를 확인할 수 있는 핵심 코드만 선별해 포함합니다.
- 프로젝트 종속 UI, Item, 연출 코드 등은 가독성을 위해 일부 생략했습니다.
- 필요한 부분에는 XML 주석과 핵심 동작 주석을 보강했습니다.
