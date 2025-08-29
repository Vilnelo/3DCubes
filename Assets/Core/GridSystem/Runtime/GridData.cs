namespace Core.GridSystem.Runtime
{
    public class GridData
    {
        private readonly int[] m_FlatData;
        private readonly int m_Width;
        private readonly int m_Height;
        private readonly int m_TotalSize;

        public int Width => m_Width;
        public int Height => m_Height;
        public int TotalSize => m_TotalSize;

        public GridData(int[] flatData, int width, int height)
        {
            m_FlatData = flatData;
            m_Width = width;
            m_Height = height;
            m_TotalSize = flatData.Length;
        }

        public int GetValue(int index) => m_FlatData[index];

        public int GetValue(int x, int y) => m_FlatData[y * m_Width + x];

        public int MoveUp(int currentIndex)
        {
            var newIndex = currentIndex - m_Width;
            return newIndex < 0 ? newIndex + m_TotalSize : newIndex;
        }

        public int MoveDown(int currentIndex)
        {
            var newIndex = currentIndex + m_Width;
            return newIndex >= m_TotalSize ? newIndex - m_TotalSize : newIndex;
        }

        public int MoveLeft(int currentIndex)
        {
            var currentRow = currentIndex / m_Width;
            var currentCol = currentIndex % m_Width;
            var newCol = currentCol == 0 ? m_Width - 1 : currentCol - 1;
            return currentRow * m_Width + newCol;
        }

        public int MoveRight(int currentIndex)
        {
            var currentRow = currentIndex / m_Width;
            var currentCol = currentIndex % m_Width;
            var newCol = (currentCol + 1) % m_Width;
            return currentRow * m_Width + newCol;
        }

        public int[] GetAreaAround(int centerIndex, int areaSize)
        {
            var result = new int[areaSize * areaSize];
            var halfSize = areaSize / 2;
            var centerX = centerIndex % m_Width;
            var centerY = centerIndex / m_Width;

            var resultIndex = 0;
            for (var dy = -halfSize; dy <= halfSize; dy++)
            {
                for (var dx = -halfSize; dx <= halfSize; dx++)
                {
                    var targetX = WrapCoordinate(centerX + dx, m_Width);
                    var targetY = WrapCoordinate(centerY + dy, m_Height);
                    var targetIndex = targetY * m_Width + targetX;

                    result[resultIndex] = m_FlatData[targetIndex];
                    resultIndex++;
                }
            }

            return result;
        }

        private int WrapCoordinate(int coordinate, int maxValue)
        {
            if (coordinate < 0)
                return coordinate + maxValue;
            if (coordinate >= maxValue)
                return coordinate - maxValue;
            return coordinate;
        }
    }
}