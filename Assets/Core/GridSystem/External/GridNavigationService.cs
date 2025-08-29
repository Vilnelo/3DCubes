using System;
using Core.GridSystem.Runtime;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.GridSystem.External
{
    public class GridNavigationService : IGridNavigationService, IInitializable, IDisposable
    {
        [Inject] private IGridConfigSystem m_GridSystem;

        private readonly ReactiveProperty<int> m_CurrentIndex = new ReactiveProperty<int>(0);
        private CompositeDisposable m_Disposables;

        public IReadOnlyReactiveProperty<int> CurrentIndex => m_CurrentIndex;

        public void Initialize()
        {
            m_Disposables = new CompositeDisposable();

            SetRandomInitialIndex();
        }

        public void SetRandomInitialIndex()
        {
            if (!m_GridSystem.IsLoaded)
            {
                Debug.LogError("[GridNavigationService] Grid system is not loaded");
                return;
            }

            var indexResult = m_GridSystem.GetRandomValidIndex();
            if (indexResult.IsExist)
            {
                m_CurrentIndex.Value = indexResult.Object;
                Debug.Log($"[GridNavigationService] Initial index set to: {m_CurrentIndex.Value}");
            }
        }

        public bool NavigateUp()
        {
            var result = m_GridSystem.NavigateUp(m_CurrentIndex.Value);
            if (result.IsExist)
            {
                m_CurrentIndex.Value = result.Object;
                Debug.Log($"[GridNavigationService] UP - New index: {m_CurrentIndex.Value}");
                return true;
            }

            return false;
        }

        public bool NavigateDown()
        {
            var result = m_GridSystem.NavigateDown(m_CurrentIndex.Value);
            if (result.IsExist)
            {
                m_CurrentIndex.Value = result.Object;
                Debug.Log($"[GridNavigationService] DOWN - New index: {m_CurrentIndex.Value}");
                return true;
            }

            return false;
        }

        public bool NavigateLeft()
        {
            var result = m_GridSystem.NavigateLeft(m_CurrentIndex.Value);
            if (result.IsExist)
            {
                m_CurrentIndex.Value = result.Object;
                Debug.Log($"[GridNavigationService] LEFT - New index: {m_CurrentIndex.Value}");
                return true;
            }

            return false;
        }

        public bool NavigateRight()
        {
            var result = m_GridSystem.NavigateRight(m_CurrentIndex.Value);
            if (result.IsExist)
            {
                m_CurrentIndex.Value = result.Object;
                Debug.Log($"[GridNavigationService] RIGHT - New index: {m_CurrentIndex.Value}");
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            m_Disposables?.Dispose();
            m_CurrentIndex?.Dispose();
        }
    }
}