/// <summary>
/// 실행과 되돌리기가 가능한 게임플레이 행동의 공통 계약입니다.
/// </summary>
public interface ICommand
{
    bool Execute();
    void Undo();
}
