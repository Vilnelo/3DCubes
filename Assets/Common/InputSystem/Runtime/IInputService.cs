using System;
using UniRx;

namespace Common.InputSystem.Runtime
{
    public interface IInputService
    {
        IObservable<Unit> OnUpPressed { get; }
        IObservable<Unit> OnDownPressed { get; }
        IObservable<Unit> OnLeftPressed { get; }
        IObservable<Unit> OnRightPressed { get; }
    }
}