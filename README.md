# Portfolio

게임 프로그래머 **김승욱**이 프로젝트에서 직접 설계하고 구현한 핵심 코드 샘플을 정리한 저장소입니다.

전체 Unity 프로젝트를 그대로 공개하기보다, 게임플레이 시스템 설계와 문제 해결 과정을 확인할 수 있는 코드 중심으로 구성합니다.

## Projects

### [Nostalgia](./Nostalgia)
2인 협동 온라인 공포 게임

- Procedural Map Generation
- Monster State AI
- Photon Fusion 2 Multiplayer

### [MagRog](./MagRog)
로그라이크 전략 게임

- ScriptableObject-based Modular Magic System
- Target / Condition / Effect Architecture
- Status System

### [MixterPiece](./MixterPiece)
Grid 기반 퍼즐 게임

- BFS-based Paint Spread
- Wave-based Spread Presentation
- Command-based Undo / Clear

## Repository Structure

```text
Portfolio/
├─ Nostalgia/
│  ├─ ProceduralMap/
│  ├─ MonsterAI/
│  └─ Multiplayer/
├─ MagRog/
│  ├─ MagicSystem/
│  └─ StatusSystem/
└─ MixterPiece/
   ├─ PaintSpread/
   └─ CommandSystem/
```

## Note

- 각 폴더에는 실제 프로젝트에서 사용한 코드 중 핵심 구현을 선별하여 정리합니다.
- 일부 코드는 프로젝트 종속 클래스, 에셋, 설정을 포함하지 않아 단독으로 실행되지 않을 수 있습니다.
- 각 프로젝트의 설계 의도와 코드 구성은 프로젝트별 `README.md`에서 설명합니다.
