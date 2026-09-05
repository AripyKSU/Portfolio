using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임플레이 Command를 실행하고 Undo 이력을 관리합니다.
/// 구체적인 행동의 내용은 알지 않고 ICommand를 통해서만 제어합니다.
/// </summary>
public sealed class CommandController : MonoBehaviour
{
    // 성공적으로 실행된 Command를 최근 순서대로 저장한다.
    private readonly Stack<ICommand> undoStack = new();

    public bool CanUndo => undoStack.Count > 0;

    // ... UI 입력 활성화 및 버튼 처리 코드 생략 ...

    /// <summary>
    /// Command를 실행하고 성공한 경우에만 Undo 이력에 기록합니다.
    /// </summary>
    public bool Execute(ICommand command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        // 실행에 실패한 행동은 Undo 이력에 남기지 않는다.
        if (!command.Execute())
            return false;

        undoStack.Push(command);
        return true;
    }

    /// <summary>
    /// 가장 최근에 실행된 행동 하나를 되돌립니다.
    /// </summary>
    private bool UndoLast()
    {
        if (undoStack.Count == 0)
            return false;

        // Stack을 사용해 가장 최근 행동부터 역순으로 복원한다.
        ICommand command = undoStack.Pop();
        command.Undo();

        return true;
    }

    /// <summary>
    /// 모든 Command를 최근 실행 순서부터 차례대로 되돌립니다.
    /// Clear 기능에서 사용합니다.
    /// </summary>
    private void UndoAll()
    {
        while (undoStack.Count > 0)
        {
            undoStack.Pop().Undo();
        }
    }

    // ... Sound, 입력 잠금, History 초기화 코드 생략 ...
}
