using Utils.Result;

namespace Core.GridSystem.Runtime
{
    public interface IGridConfigSystem
    {
        bool IsLoaded { get; }
        GridData Data { get; }
        
        void LoadGridData();
        
        Result<int> GetRandomValidIndex();
        Result<int[]> GetViewportData(int centerIndex, int viewportSize);
        
        Result<int> NavigateUp(int currentIndex);
        Result<int> NavigateDown(int currentIndex);
        Result<int> NavigateLeft(int currentIndex);
        Result<int> NavigateRight(int currentIndex);
    }
}