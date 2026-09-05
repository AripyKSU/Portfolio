# Nostalgia

**Nostalgia**는 서로 다른 역할을 가진 두 플레이어가 협력해 스테이지를 진행하는 온라인 협동 공포 게임입니다.

이 폴더에는 프로젝트에서 직접 설계하고 구현한 시스템 중, 게임 프로그래밍 역량을 보여줄 수 있는 핵심 코드 샘플을 정리합니다.

## Code Samples

### [ProceduralMap](./ProceduralMap)
플레이마다 달라지는 스테이지를 구성하기 위한 절차적 맵 생성 흐름입니다.

- State Authority에서만 맵 생성을 시작해 중복 생성을 방지
- 영역별 타일 배치 후 런타임 NavMesh 생성
- 몬스터, 아이템, 출구, 플레이어 배치를 순차적으로 초기화

### [MonsterAI](./MonsterAI)
몬스터 행동을 상태 단위로 분리하고, 각 상태의 진입·실행·종료 로직을 독립적으로 관리한 코드입니다.

- `OnEnterState`, `OnFixedUpdate`, `OnExitState`로 상태 생명주기 분리
- 감지된 소리 위치를 기준으로 NavMesh 탐색 수행
- AI 판단과 이동은 State Authority에서만 수행
- 애니메이션과 필요한 결과는 RPC를 통해 동기화

## Tech Stack

- C#
- Unity
- Photon Fusion 2
- NavMesh

## Note

본 저장소에는 전체 프로젝트가 아닌 핵심 구현 코드만 선별하여 포함합니다. 현재 샘플은 전체 클래스보다 구현 의도가 잘 드러나는 함수 중심의 발췌본이며, 프로젝트 종속 코드와 비핵심 로직은 생략될 수 있습니다.

멀티플레이 전용 API 호출 예제는 별도 샘플로 두지 않고, 실제 게임플레이 로직에 적용된 State Authority 및 RPC 처리만 포함합니다.
