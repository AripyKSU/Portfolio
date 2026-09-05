using System.Collections;
using Fusion;

// ... 기타 using문 및 데이터 구조(Area, ItemConfig) 생략 ...

public class MapCreator : NetworkBehaviour
{
    // ... 네트워크 상태값, 맵 설정 및 스폰 관련 필드 생략 ...

    /// <summary>
    /// 네트워크 객체가 생성되면 State Authority에서만 절차적 맵 생성을 시작한다.
    /// </summary>
    public override void Spawned()
    {
        base.Spawned();

        // 맵 생성은 State Authority에서만 수행해 중복 생성을 방지한다.
        if (!HasStateAuthority)
        {
            return;
        }

        StartCoroutine(CreateMap());
    }

    /// <summary>
    /// 영역별 타일을 순차적으로 배치한 뒤 NavMesh와 게임플레이 오브젝트를 초기화한다.
    /// </summary>
    private IEnumerator CreateMap()
    {
        // 각 영역의 타일을 무작위 위치와 회전으로 순차 배치한다.
        yield return StartCoroutine(ShuffleAreaCoroutine(m_area4));
        yield return StartCoroutine(ShuffleAreaCoroutine(m_area3));
        yield return StartCoroutine(ShuffleAreaCoroutine(m_area2));

        // 배치가 끝난 맵을 기준으로 런타임 NavMesh를 생성한다.
        navMeshSurface.BuildNavMesh();

        // 생성된 타일에서 수집한 후보 위치를 이용해 게임플레이 오브젝트를 배치한다.
        m_mobSpawner.SpawnMobs();
        StartCoroutine(SetItems());
        StartCoroutine(SetExit());

        bMapCreated = true;

        // 맵 구성이 끝난 뒤 플레이어를 최종 스폰 위치에 배치한다.
        StartCoroutine(SetPlayers());
    }

    // ... ShuffleAreaCoroutine, SetPlayers, SetItems, SetExit 등 세부 구현 생략 ...
}
