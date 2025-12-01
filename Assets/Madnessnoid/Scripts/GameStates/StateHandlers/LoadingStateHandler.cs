#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

using Cysharp.Threading.Tasks;

using KarenKrill.UniCore.StateSystem.Abstractions;

using KarenKrill.ContentLoading.Abstractions;
using KarenKrill.DataStorage.Abstractions;
using Madnessnoid.Abstractions;
using Madnessnoid.UI.Presenters.Abstractions;

namespace Madnessnoid.GameStates
{
    public class LoadingStateHandler : PresentableStateHandlerBase<GameState>, IStateHandler<GameState>
    {
        public override GameState State => GameState.Loading;

        public LoadingStateHandler(ILogger logger,
            IStateSwitcher<GameState> stateSwitcher,
            GameSettings gameSettings,
            IPlayerSession playerSession,
            IContentLoaderPresenter contentLoaderPresenter,
            IDataStorage dataStorage,
            ISceneLoader sceneLoader) : base(contentLoaderPresenter)
        {
            _logger = logger;
            _stateSwitcher = stateSwitcher;
            _gameSettings = gameSettings;
            _playerSession = playerSession;
            _saveSettingsData[_settingsDataKey] = _gameSettings;
            _savePlayerSessionData[_playerSessionDataKey] = _playerSessionData;
            _contentLoaderPresenter = contentLoaderPresenter;
            _dataStorage = dataStorage;
            _sceneLoader = sceneLoader;
        }

        public override void Enter(GameState prevState, object? context = null)
        {
            _logger.Log(nameof(LoadingStateHandler), nameof(Enter));
            base.Enter(prevState);

            // Speeding up scene loading by reducing the frame rate
            _initialThreadPriority = Application.backgroundLoadingPriority;
            Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.High;

            var cancellationToken = Application.exitCancellationToken;
            LoadLoadingSceneAsync(cancellationToken).ContinueWith(() =>
            {
                if (prevState == GameState.Initial)
                {
                    InitialMenuSceneLoad(context, cancellationToken).Forget();
                }
                else
                {
                    LoadSceneAsync(context, cancellationToken).Forget();
                }
            }).Forget();
        }

        public override void Exit(GameState nextState)
        {
            base.Exit(nextState);
            _logger.Log(nameof(LoadingStateHandler), nameof(Exit));
            Application.backgroundLoadingPriority = _initialThreadPriority;
        }

        private static readonly string _settingsDataKey = "Settings";
        private static readonly string _playerSessionDataKey = "PlayerSession";
        private static readonly string _loadingSceneName = "LoadingScene";
        private static readonly string _mainMenuSceneName = "MainMenuScene";
        private static readonly string _levelSceneBaseName = "LevelSceneBase";
        private static readonly string _connectingToServerStageText = "Connecting to the server...";
        private static readonly string _dataLoadingStageText = "Loading profile data...";
        private static readonly string _menuLoadingStageText = "Loading main menu...";
        private static readonly string _levelLoadingStageText = "Loading a level...";
        private static readonly string _successStatusText = "Press any key to start";
        private static readonly string _failureStatusText = "Failed to connect to the server. Please try again later.";

        private readonly ILogger _logger;
        private readonly IStateSwitcher<GameState> _stateSwitcher;
        private readonly GameSettings _gameSettings;
        private readonly IPlayerSession _playerSession;
        private readonly IContentLoaderPresenter _contentLoaderPresenter;
        private readonly IDataStorage _dataStorage;
        private readonly ISceneLoader _sceneLoader;

        private readonly Dictionary<string, object?> _saveSettingsData = new() { { _settingsDataKey, null } };
        private readonly Dictionary<string, Type> _loadSettingsMetadata = new() { { _settingsDataKey, typeof(GameSettings) } };
        private readonly Dictionary<string, object?> _savePlayerSessionData = new() { { _playerSessionDataKey, null } };
        private readonly Dictionary<string, Type> _loadPlayerSessionMetadata = new() { { _playerSessionDataKey, typeof(PlayerSessionData) } };
        private readonly PlayerSessionData _playerSessionData = new();
        private UnityEngine.ThreadPriority _initialThreadPriority;
        private bool _isInternalChange = false;

        private async UniTask InitialMenuSceneLoad(object? context, CancellationToken cancellationToken)
        {
            try
            {
                _contentLoaderPresenter.ProgressValue = 0;
                _contentLoaderPresenter.StageText = _connectingToServerStageText;
                await _dataStorage.InitializeAsync();
                _contentLoaderPresenter.StageText = _dataLoadingStageText;
                _gameSettings.SettingsChanged += OnSettingsChanged;
                _playerSession.MoneyChanged += OnPlayerSessionMoneyChanged;
                await LoadDataAsync(cancellationToken);
                await LoadPlayerSessionDataAsync(cancellationToken);
                await LoadSceneAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(LoadingStateHandler), $"{ex.GetType()} occured while trying to initialize data storage: {ex}");
                _contentLoaderPresenter.StatusText = _failureStatusText;
            }
        }

        private async UniTask LoadDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                var data = await _dataStorage.LoadAsync(_loadSettingsMetadata);
#if !UNITY_WEBGL
                try
                {
                    await UniTask.SwitchToMainThread(cancellationToken);
                }
                catch (OperationCanceledException) { }
#endif
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning(nameof(LoadingStateHandler), "Data loading canceled");
                    return;
                }
                if (data.TryGetValue(_settingsDataKey, out var settingsObj) && settingsObj is GameSettings settings)
                {
                    _gameSettings.FreezeSettingsChanged = true;
                    try
                    {
                        _gameSettings.ShowFps = settings.ShowFps;
                        _gameSettings.MusicVolume = Mathf.Clamp01(settings.MusicVolume);
                        _gameSettings.QualityLevel = settings.QualityLevel;
                        _gameSettings.PussyMode = settings.PussyMode;
                    }
                    finally
                    {
                        _gameSettings.FreezeSettingsChanged = false;
                    }
                }
                else
                {
                    _logger.LogWarning(nameof(LoadingStateHandler), $"No saved \"{_settingsDataKey}\" data key");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(LoadingStateHandler), $"Player data loading failed: {ex}");
            }
        }

        private async UniTask LoadPlayerSessionDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                var data = await _dataStorage.LoadAsync(_loadPlayerSessionMetadata);
#if !UNITY_WEBGL
                try
                {
                    await UniTask.SwitchToMainThread(cancellationToken);
                }
                catch (OperationCanceledException) { }
#endif
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning(nameof(LoadingStateHandler), "Player session data loading canceled");
                    return;
                }
                if (data.TryGetValue(_playerSessionDataKey, out var playerSessionDataObj) && playerSessionDataObj is PlayerSessionData playerSessionData)
                {
                    _isInternalChange = true;
                    try
                    {
                        _playerSession.AddMoney(playerSessionData.Money);
                    }
                    finally
                    {
                        _isInternalChange = false;
                    }
                }
                else
                {
                    _logger.LogWarning(nameof(LoadingStateHandler), $"No saved \"{_playerSessionDataKey}\" data key");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(LoadingStateHandler), $"Player data loading failed: {ex}");
            }
        }

        private async UniTask LoadLoadingSceneAsync(CancellationToken cancellationToken)
        {
            await _sceneLoader.LoadAsync(_loadingSceneName, cancellationToken: cancellationToken);
        }

        private async UniTask LoadSceneAsync(object? context, CancellationToken cancellationToken)
        {
            _contentLoaderPresenter.ProgressValue = 0;
            if (context is not LoadingStateContext loadingContext || loadingContext.LevelIndex < 0)
            {
                _contentLoaderPresenter.StageText = _menuLoadingStageText;
                await _sceneLoader.LoadAsync(_mainMenuSceneName,
                    new SceneLoadParameters(progressAction: OnSceneLoadProgressChanged,
                        activationRequestAction: OnActivationRequested),
                    cancellationToken);
                _stateSwitcher.TransitTo(GameState.MainMenu, null);
            }
            else
            {
                _contentLoaderPresenter.StageText = _levelLoadingStageText;
                await _sceneLoader.LoadAsync(_levelSceneBaseName,
                    new SceneLoadParameters(progressAction: OnSceneLoadProgressChanged,
                        activationRequestAction: OnActivationRequested),
                    cancellationToken);
                _stateSwitcher.TransitTo(GameState.Gameplay, new GameplayStateContext(false, loadingContext.LevelIndex));
            }
        }

        private void OnSettingsChanged()
        {
            _dataStorage.SaveAsync(_saveSettingsData).AsUniTask().Forget();
        }

        private void OnPlayerSessionMoneyChanged()
        {
            _playerSessionData.Money = _playerSession.Money;
            if (!_isInternalChange)
            {
                _dataStorage.SaveAsync(_savePlayerSessionData).AsUniTask().Forget();
            }
        }

        private void OnSceneLoadProgressChanged(float progress)
        {
            _contentLoaderPresenter.ProgressValue = progress;
        }

        private void OnActivationRequested(Action allowActivationAction)
        {
            _contentLoaderPresenter.ProgressValue = 1;
            void OnContentLoaderPresenterContinue()
            {
                _contentLoaderPresenter.EnableContinue = false;
                _contentLoaderPresenter.Continue -= OnContentLoaderPresenterContinue;
                allowActivationAction?.Invoke();
            }
            _contentLoaderPresenter.Continue += OnContentLoaderPresenterContinue;
            _contentLoaderPresenter.StatusText = _successStatusText;
            _contentLoaderPresenter.EnableContinue = true;
        }

        [Serializable]
        private class PlayerSessionData
        {
            [field: SerializeField]
            public int Money { get; set; } = 0;
        }
    }
}