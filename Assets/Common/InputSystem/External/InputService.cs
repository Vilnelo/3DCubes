using System;
using Common.InputSystem.Runtime;
using UniRx;
using UnityEngine;
using Zenject;

namespace Common.InputSystem.External
{
    public class InputService : MonoBehaviour, IInputService, IInitializable, IDisposable
    {
        private InputController m_InputController;
        private CompositeDisposable m_Disposables;

        private readonly Subject<Unit> m_OnUpPressed = new Subject<Unit>();
        private readonly Subject<Unit> m_OnDownPressed = new Subject<Unit>();
        private readonly Subject<Unit> m_OnLeftPressed = new Subject<Unit>();
        private readonly Subject<Unit> m_OnRightPressed = new Subject<Unit>();

        public IObservable<Unit> OnUpPressed => m_OnUpPressed.AsObservable();
        public IObservable<Unit> OnDownPressed => m_OnDownPressed.AsObservable();
        public IObservable<Unit> OnLeftPressed => m_OnLeftPressed.AsObservable();
        public IObservable<Unit> OnRightPressed => m_OnRightPressed.AsObservable();

        public void Initialize()
        {
            m_InputController = new InputController();
            m_Disposables = new CompositeDisposable();
            
            Debug.Log("InputService: Initialized");
        }

        private void Update()
        {
            if (m_InputController == null) return;

            var inputResult = m_InputController.GetInput();

            if (inputResult != InputResult.None)
            {
                ProcessInputResult(inputResult);
            }
        }

        private void ProcessInputResult(InputResult inputResult)
        {
            switch (inputResult)
            {
                case InputResult.Up:
                    m_OnUpPressed.OnNext(Unit.Default);
                    break;

                case InputResult.Down:
                    m_OnDownPressed.OnNext(Unit.Default);
                    break;

                case InputResult.Left:
                    m_OnLeftPressed.OnNext(Unit.Default);
                    break;

                case InputResult.Right:
                    m_OnRightPressed.OnNext(Unit.Default);
                    break;
            }
        }

        public void Dispose()
        {
            m_Disposables?.Dispose();
            
            m_OnUpPressed?.Dispose();
            m_OnDownPressed?.Dispose();
            m_OnLeftPressed?.Dispose();
            m_OnRightPressed?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}