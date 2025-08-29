using UniRx;

namespace Core.GridSystem.Runtime
{
    public interface IGridNavigationService
    {
        IReadOnlyReactiveProperty<int> CurrentIndex { get; }

        void SetRandomInitialIndex();
        bool NavigateUp();
        bool NavigateDown();
        bool NavigateLeft();
        bool NavigateRight();
    }
}