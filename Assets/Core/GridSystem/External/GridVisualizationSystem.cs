using System;
using Common.AssetManagement.External;
using Common.ConfigSystem.Runtime;
using Common.InputSystem.Runtime;
using Core.GridSystem.Runtime;
using Core.GridSystem.Runtime.Dto;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.GridSystem.External
{
    public class GridVisualizationSystem : MonoBehaviour, IGridVisualizationSystem, IInitializable, IDisposable
    {
        private const string m_GridContainerKey = "GridContainer";
        private const string m_CubePrefabKey = "CubePrefab";
        private const string m_ConfigKey = "GridConfig";

        [Inject] private IAssetLoader m_AssetLoader;
        [Inject] private IConfigLoader m_ConfigLoader;
        [Inject] private IGridConfigSystem m_GridSystem;
        [Inject] private IGridNavigationService m_NavigationService;
        [Inject] private IInputService m_InputService;

        private GridVisualConfigDto m_VisualConfig;
        private MaterialCache m_MaterialCache;
        private GameObject m_GridContainer;
        private GameObject[] m_CubeInstances;
        private CompositeDisposable m_Disposables;
        private bool m_IsInitialized;

        public bool IsInitialized => m_IsInitialized;

        public void Initialize()
        {
            Debug.Log("[GridVisualizationSystem] Initializing...");

            m_MaterialCache = new MaterialCache();
            m_Disposables = new CompositeDisposable();
            LoadVisualConfig();

            if (m_VisualConfig != null)
            {
                CreateGridContainer();
                CreateCubeGrid();
                SetupInputListeners();
                m_IsInitialized = true;
                SetupNavigationSubscription();

                Debug.Log("[GridVisualizationSystem] Initialization complete");
            }
            else
            {
                Debug.LogError("[GridVisualizationSystem] Failed to initialize - config not loaded");
            }
        }

        private void LoadVisualConfig()
        {
            try
            {
                m_VisualConfig = m_ConfigLoader.GetConfig<GridVisualConfigDto>(m_ConfigKey);
                if (m_VisualConfig != null)
                {
                    Debug.Log($"[GridVisualizationSystem] Visual config loaded - Grid Size: {m_VisualConfig.GridSize}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GridVisualizationSystem] Failed to load visual config: {ex.Message}");
            }
        }

        private void CreateGridContainer()
        {
            var containerPrefab = m_AssetLoader.LoadSync<GameObject>(m_GridContainerKey);
            if (containerPrefab == null)
            {
                Debug.LogError("[GridVisualizationSystem] Failed to load GridContainer prefab");
                return;
            }

            m_GridContainer = Instantiate(containerPrefab);
            m_GridContainer.name = "GridContainer_Instance";

            CameraSetup();

            Debug.Log("[GridVisualizationSystem] Grid container created");
        }

        private void CameraSetup()
        {
            m_GridContainer.transform.position = new Vector3(0, 0, 0);

            var camera = Camera.main;
            if (camera != null)
            {
                PositionCamera(camera);
            }
        }

        private void PositionCamera(Camera camera)
        {
            var gridWorldSize = (m_VisualConfig.GridSize - 1) * m_VisualConfig.Spacing;
            var cameraDistance = gridWorldSize * 1.5f;

            camera.transform.position = new Vector3(0, cameraDistance, -cameraDistance);
            camera.transform.LookAt(m_GridContainer.transform.position);
        }

        private void CreateCubeGrid()
        {
            if (!m_GridSystem.IsLoaded)
            {
                Debug.LogError("[GridVisualizationSystem] GridSystem data not loaded");
                return;
            }

            var cubePrefab = m_AssetLoader.LoadSync<GameObject>(m_CubePrefabKey);
            if (cubePrefab == null)
            {
                Debug.LogError("[GridVisualizationSystem] Failed to load CubePrefab");
                return;
            }

            var totalCubes = m_VisualConfig.GridSize * m_VisualConfig.GridSize;
            m_CubeInstances = new GameObject[totalCubes];

            var halfGrid = (m_VisualConfig.GridSize - 1) * 0.5f;
            var spacing = m_VisualConfig.Spacing;

            for (var i = 0; i < totalCubes; i++)
            {
                var x = i % m_VisualConfig.GridSize;
                var z = (m_VisualConfig.GridSize - 1) - (i / m_VisualConfig.GridSize);

                var cubeInstance = Instantiate(cubePrefab, m_GridContainer.transform);
                cubeInstance.name = $"Cube_{x}_{z}";

                var worldPos = new Vector3(
                    (x - halfGrid) * spacing,
                    0f,
                    (z - halfGrid) * spacing
                );

                cubeInstance.transform.localPosition = worldPos;
                cubeInstance.transform.localScale = Vector3.one * m_VisualConfig.CubeScale;

                m_CubeInstances[i] = cubeInstance;
            }

            Debug.Log(
                $"[GridVisualizationSystem] Created {totalCubes} cubes in {m_VisualConfig.GridSize}x{m_VisualConfig.GridSize} grid");
        }

        private void SetupInputListeners()
        {
            m_InputService.OnUpPressed.Subscribe(_ => m_NavigationService.NavigateUp()).AddTo(m_Disposables);
            m_InputService.OnDownPressed.Subscribe(_ => m_NavigationService.NavigateDown()).AddTo(m_Disposables);
            m_InputService.OnLeftPressed.Subscribe(_ => m_NavigationService.NavigateLeft()).AddTo(m_Disposables);
            m_InputService.OnRightPressed.Subscribe(_ => m_NavigationService.NavigateRight()).AddTo(m_Disposables);

            Debug.Log("[GridVisualizationSystem] Input listeners setup complete");
        }

        private void SetupNavigationSubscription()
        {
            m_NavigationService.CurrentIndex
                .Subscribe(OnCurrentIndexChanged)
                .AddTo(m_Disposables);
            
            if (m_NavigationService.CurrentIndex.Value != 0 || m_GridSystem.IsLoaded)
            {
                OnCurrentIndexChanged(m_NavigationService.CurrentIndex.Value);
            }

            Debug.Log("[GridVisualizationSystem] Navigation subscription setup complete");
        }

        private void OnCurrentIndexChanged(int currentIndex)
        {
            Debug.Log($"[GridVisualizationSystem] Current index changed to: {currentIndex}");
            UpdateViewportForIndex(currentIndex);
        }

        private void UpdateViewportForIndex(int centerIndex)
        {
            var viewportResult = m_GridSystem.GetViewportData(centerIndex, m_VisualConfig.GridSize);
            if (viewportResult.IsExist)
            {
                UpdateCubesColors(viewportResult.Object);
            }
        }

        public void UpdateCubesColors(int[] gridData)
        {
            if (!m_IsInitialized || m_CubeInstances == null || gridData == null)
            {
                Debug.LogError("[GridVisualizationSystem] Cannot update colors - not initialized or invalid data");
                return;
            }

            var expectedLength = m_VisualConfig.GridSize * m_VisualConfig.GridSize;
            if (gridData.Length != expectedLength)
            {
                Debug.LogError(
                    $"[GridVisualizationSystem] Grid data length mismatch. Expected: {expectedLength}, Got: {gridData.Length}");
                return;
            }

            for (var i = 0; i < gridData.Length; i++)
            {
                var colorCode = gridData[i];
                if (TryGetColor(colorCode, out var color))
                {
                    var material = m_MaterialCache.GetMaterial(colorCode, color);
                    var renderer = m_CubeInstances[i].GetComponent<MeshRenderer>();
                    renderer.material = material;
                }
                else
                {
                    Debug.LogWarning($"[GridVisualizationSystem] Color not found for code: {colorCode}");
                }
            }
        }

        private bool TryGetColor(int digit, out Color color)
        {
            color = Color.white;

            if (m_VisualConfig.ColorMappings == null ||
                !m_VisualConfig.ColorMappings.TryGetValue(digit.ToString(), out var hexColor))
                return false;

            return ColorUtility.TryParseHtmlString(hexColor, out color);
        }

        public void Dispose()
        {
            m_Disposables?.Dispose();

            if (m_GridContainer != null)
            {
                DestroyImmediate(m_GridContainer);
                m_GridContainer = null;
            }

            m_CubeInstances = null;
            m_MaterialCache?.Clear();

            Debug.Log("[GridVisualizationSystem] Disposed");
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}