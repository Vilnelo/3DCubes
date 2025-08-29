﻿using System;
using Common.LocalizationSystem.Runtime;
using UniRx;
using UnityEngine;
using Zenject;

namespace Common.LocalizationSystem.External
{
    public class LocalizationController : IInitializable, IDisposable
    {
        private const string LANGUAGE_PREFS_KEY = "Language";
        private const string DEFAULT_LANGUAGE = "EN";
        
        [Inject] private ILocalizationService m_LocalizationService;
        [Inject] private LocalizationDataLoader m_DataLoader;
        
        private readonly CompositeDisposable m_Disposables = new();

        public void Initialize()
        {
            Debug.Log("[LocalizationController] Initializing...");
            
            LocalizationExtensions.Initialize(m_LocalizationService);
            
            var localizationData = m_DataLoader.LoadLocalizationData();
            string savedLanguage = PlayerPrefs.GetString(LANGUAGE_PREFS_KEY, DEFAULT_LANGUAGE);
            
            m_LocalizationService.Initialize(localizationData, savedLanguage);
            
            m_LocalizationService.CurrentLanguage
                .Subscribe(OnLanguageChanged)
                .AddTo(m_Disposables);
            
            Debug.Log($"[LocalizationController] Initialized with language: {m_LocalizationService.CurrentLanguage.Value}");
            
            // TODO: Remove this hardcoded RU setting and implement device language detection
            m_LocalizationService.SetLanguage("RU");
        }

        private void OnLanguageChanged(string newLanguage)
        {
            PlayerPrefs.SetString(LANGUAGE_PREFS_KEY, newLanguage);
            PlayerPrefs.Save();
            Debug.Log($"[LocalizationController] Language changed to: {newLanguage}");
        }

        public void Dispose()
        {
            m_Disposables?.Dispose();
            m_DataLoader?.Dispose();
            Debug.Log("[LocalizationController] Disposed");
        }
    }
}