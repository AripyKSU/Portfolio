# MixterPiece

## 🎥 Gameplay Video

[![MixterPiece Gameplay Video](https://img.youtube.com/vi/CswE8Itpmho/hqdefault.jpg)](https://youtu.be/CswE8Itpmho)

[▶ YouTube에서 보기](https://youtu.be/CswE8Itpmho) · [WebGL 플레이](https://chaaaron000.github.io/nan2026/)

**MixterPiece**는 Grid 위에 물감을 배치하고 색의 확산과 혼합을 이용해 문제를 해결하는 퍼즐 게임입니다.

이 폴더에는 물감 확산 규칙과 Undo / Clear를 구현하면서 사용한 핵심 알고리즘 및 상태 관리 코드를 정리합니다.

## Code Samples

### [PaintSpread](./PaintSpread)
벽을 고려한 BFS로 물감이 실제 이동 가능한 경로를 따라 확산되도록 구현하고, 계산 결과를 거리별 Wave로 나누어 연출에 사용한 시스템입니다.

- `GridState.cs`
  - Grid Cell 사이의 벽 정보를 관리하고 `CanMove()`로 실제 이동 가능 여부 판단
- `PaintSpreadCalculator.cs`
  - `Queue<SearchNode>`를 사용해 상하좌우 방향으로 BFS 수행
  - 벽을 통과하지 않는 최단거리를 계산하고, 같은 거리의 Cell을 하나의 Wave로 분류
- `PaintApplicationPlan.cs`
  - 한 번의 물감 사용으로 발생하는 전체 확산 결과를 거리별 Wave로 보관
  - 각 Cell의 `PreviousState`와 `ResultState`를 함께 저장해 연출과 Undo가 같은 계산 결과를 공유하도록 구성
- `PaintSpreadSequencePlayer.cs`
  - 계산된 Wave를 거리 순서대로 재생해 같은 거리에 있는 Cell이 함께 퍼지는 형태로 표현

초기에는 Manhattan Distance를 기준으로 범위를 계산했지만, 벽 반대편 Cell까지 물감이 퍼지는 문제가 있었습니다. 이를 해결하기 위해 Grid의 연결 관계를 직접 탐색하는 BFS로 변경했고, 이후 BFS 탐색 순서를 그대로 연출하지 않고 최단거리 기준으로 Cell을 다시 묶어 자연스러운 Wave 확산을 구현했습니다.

```text
GridState.CanMove
        ↓
Queue 기반 BFS
        ↓
PaintApplicationPlan
        ↓
거리별 PaintSpreadWave
        ↓
PaintSpreadSequencePlayer
```

### [CommandSystem](./CommandSystem)
물감통 한 번의 사용으로 함께 변경되는 여러 상태를 하나의 행동 단위로 묶어 Undo / Clear를 처리한 Command 기반 시스템입니다.

- `ICommand.cs`
  - 게임플레이 행동의 `Execute / Undo` 공통 계약 정의
- `PaintBucketUseCommand.cs`
  - 물감 확산 결과 적용과 물감통 소비를 하나의 실행 단위로 처리
  - `PaintApplicationPlan`에 저장된 각 Cell의 이전 상태를 이용해 Grid와 물감통 상태를 함께 복원
- `CommandController.cs`
  - 성공적으로 실행된 Command만 `Stack<ICommand>`에 기록
  - Undo는 가장 최근 Command부터 복원하고, Clear는 Stack이 빌 때까지 동일한 Undo 흐름을 반복

이 구조를 통해 Grid Cell의 색상 변화와 물감통 소비처럼 서로 다른 상태 변경을 하나의 플레이어 행동으로 관리하고, 별도의 초기화 로직을 중복 구현하지 않고 동일한 복원 로직을 Undo와 Clear에 재사용했습니다.

```text
Paint Bucket Use
        ↓
PaintBucketUseCommand.Execute()
        ↓
CommandController
        ↓
Stack<ICommand>
        ↓
Undo() / Clear
```

## Tech Stack

- C#
- Unity
- BFS
- Queue
- Command Pattern

## Note

- 전체 프로젝트가 아닌 알고리즘과 시스템 설계를 확인할 수 있는 핵심 구현 코드만 선별하여 포함합니다.
- 프로젝트 종속 UI, 이펙트, 입력 처리 등은 가독성을 위해 일부 생략했습니다.
- 필요한 부분에는 XML 주석과 핵심 동작 주석을 보강했습니다.
