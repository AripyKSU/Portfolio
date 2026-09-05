# MixterPiece

**MixterPiece**는 Grid 위에 물감을 배치하고 색의 확산과 혼합을 이용해 문제를 해결하는 퍼즐 게임입니다.

이 폴더에는 Grid 탐색과 상태 관리 과정에서 구현한 핵심 시스템 코드를 정리합니다.

## Code Samples

### [PaintSpread](./PaintSpread)
벽을 고려하면서 물감이 실제 이동 가능한 경로를 따라 확산되도록 구현한 시스템입니다.

- 초기 Manhattan Distance 방식에서 발생한 벽 통과 문제 분석
- BFS를 이용해 연결된 Grid Cell만 탐색하도록 개선
- 탐색 결과를 거리별 Wave로 분류
- 확산 범위 계산과 시각적 확산 연출을 분리

이를 통해 동일한 거리에 있는 Cell이 같은 단계에서 퍼지는 형태로 연출할 수 있도록 구성했습니다.

### [CommandSystem](./CommandSystem)
물감통 사용으로 동시에 변경되는 여러 상태를 하나의 행동 단위로 관리하기 위한 Command 기반 시스템입니다.

- Grid Cell 색상 변경
- 물감 혼합 결과 변경
- 물감통 개수 변경
- 실행 전 상태 저장 및 복원
- Undo / Clear 기능에서 동일한 복원 구조 활용

여러 상태 변경을 하나의 Command로 묶어 플레이어의 행동 전체를 일관된 단위로 되돌릴 수 있도록 설계했습니다.

## Tech Stack

- C#
- Unity
- BFS
- Command Pattern

## Note

본 저장소에는 전체 프로젝트가 아닌 알고리즘과 시스템 설계를 확인할 수 있는 핵심 구현 코드만 선별하여 포함합니다.
