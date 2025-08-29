using Common.AssetManagement.External;
using Core.GridSystem.Runtime;
using UnityEngine;
using Utils.Result;
using Zenject;

namespace Core.GridSystem.External
{
    public class GridConfigSystem : MonoBehaviour, IGridConfigSystem, IInitializable
    {
        private const string m_GridDataKey = "GridData";

        [Inject] private IAssetLoader m_AssetLoader;

        private GridTextParser m_TextParser;
        private GridData m_GridData;
        private bool m_IsLoaded;

        public bool IsLoaded => m_IsLoaded;
        public GridData Data => m_GridData;

        public void Initialize()
        {
            m_TextParser = new GridTextParser();
            LoadGridData();
        }

        public void LoadGridData()
        {
            if (m_GridData != null)
                return;

            var textAsset = m_AssetLoader.LoadSync<TextAsset>(m_GridDataKey);
            if (textAsset == null)
            {
                Debug.LogError("[GridConfigSystem] Failed to load grid data TextAsset");
                return;
            }

            var parseResult = m_TextParser.Parse(textAsset.text);
            if (!parseResult.IsExist)
            {
                Debug.LogError("[GridConfigSystem] Failed to parse grid data - invalid format or content");
                return;
            }

            m_GridData = parseResult.Object;
            m_IsLoaded = true;
            
            Debug.Log($"[GridConfigSystem] Loaded grid data: {m_GridData.Width}x{m_GridData.Height}");
        }

        public Result<int> GetRandomValidIndex()
        {
            if (m_GridData == null)
            {
                Debug.LogError("[GridConfigSystem] Grid data is not loaded");
                return Result<int>.Fail();
            }

            var randomIndex = Random.Range(0, m_GridData.TotalSize);
            return Result<int>.Success(randomIndex);
        }

        public Result<int[]> GetViewportData(int centerIndex, int viewportSize)
        {
            if (m_GridData == null)
            {
                Debug.LogError("[GridConfigSystem] Grid data is not loaded");
                return Result<int[]>.Fail();
            }

            if (centerIndex < 0 || centerIndex >= m_GridData.TotalSize)
            {
                Debug.LogError($"[GridConfigSystem] Invalid center index: {centerIndex}. Valid range: 0-{m_GridData.TotalSize - 1}");
                return Result<int[]>.Fail();
            }

            var viewportData = m_GridData.GetAreaAround(centerIndex, viewportSize);
            return Result<int[]>.Success(viewportData);
        }

        public Result<int> NavigateUp(int currentIndex)
        {
            if (m_GridData == null)
            {
                Debug.LogError("[GridConfigSystem] Grid data is not loaded");
                return Result<int>.Fail();
            }

            if (currentIndex < 0 || currentIndex >= m_GridData.TotalSize)
            {
                Debug.LogError($"[GridConfigSystem] Invalid current index: {currentIndex}. Valid range: 0-{m_GridData.TotalSize - 1}");
                return Result<int>.Fail();
            }

            var newIndex = m_GridData.MoveUp(currentIndex);
            return Result<int>.Success(newIndex);
        }

        public Result<int> NavigateDown(int currentIndex)
        {
            if (m_GridData == null)
            {
                Debug.LogError("[GridConfigSystem] Grid data is not loaded");
                return Result<int>.Fail();
            }

            if (currentIndex < 0 || currentIndex >= m_GridData.TotalSize)
            {
                Debug.LogError($"[GridConfigSystem] Invalid current index: {currentIndex}. Valid range: 0-{m_GridData.TotalSize - 1}");
                return Result<int>.Fail();
            }

            var newIndex = m_GridData.MoveDown(currentIndex);
            return Result<int>.Success(newIndex);
        }

        public Result<int> NavigateLeft(int currentIndex)
        {
            if (m_GridData == null)
            {
                Debug.LogError("[GridConfigSystem] Grid data is not loaded");
                return Result<int>.Fail();
            }

            if (currentIndex < 0 || currentIndex >= m_GridData.TotalSize)
            {
                Debug.LogError($"[GridConfigSystem] Invalid current index: {currentIndex}. Valid range: 0-{m_GridData.TotalSize - 1}");
                return Result<int>.Fail();
            }

            var newIndex = m_GridData.MoveLeft(currentIndex);
            return Result<int>.Success(newIndex);
        }

        public Result<int> NavigateRight(int currentIndex)
        {
            if (m_GridData == null)
            {
                Debug.LogError("[GridConfigSystem] Grid data is not loaded");
                return Result<int>.Fail();
            }

            if (currentIndex < 0 || currentIndex >= m_GridData.TotalSize)
            {
                Debug.LogError($"[GridConfigSystem] Invalid current index: {currentIndex}. Valid range: 0-{m_GridData.TotalSize - 1}");
                return Result<int>.Fail();
            }

            var newIndex = m_GridData.MoveRight(currentIndex);
            return Result<int>.Success(newIndex);
        }
    }
}