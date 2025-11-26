using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using KarenKrill.UniCore.Diagnostics;
using KarenKrill.UniCore.Interactions;
using KarenKrill.UniCore.Logging;
using KarenKrill.UniCore.StateSystem;
using KarenKrill.UniCore.StateSystem.Abstractions;
using KarenKrill.UniCore.UI.Presenters;
using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views;
using KarenKrill.UniCore.Utilities;

using KarenKrill.ContentLoading;
using KarenKrill.DataStorage;

namespace Madnessnoid
{
    using Abstractions;
    using Cysharp.Threading.Tasks;
    using GameStates;
    using Input;

    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            UniTaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            Container.BindInterfacesAndSelfTo<GameConfig>().FromInstance(_gameConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<LevelSession>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerSession>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ThemeProfileProvider>().FromInstance(_themeProfileProvider).AsSingle();
            Container.BindInterfacesAndSelfTo<CloudSaveDataStorage>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<SceneLoader>().FromNew().AsSingle();
            InstallSettings();
            InstallInput();
            InstallLogging();

            Container.BindInterfacesAndSelfTo<DiagnosticsProvider>().FromInstance(_diagnosticsProvider).AsSingle();
            Container.BindInterfacesAndSelfTo<InteractionTargetRegistry>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<GameFlow>().AsSingle();
            Container.BindInterfacesAndSelfTo<AudioController>().FromInstance(_audioController).AsSingle();
            InstallGameStateMachine();
            InstallViewFactory();
            InstallPresenterBindings();
        }

        [SerializeField]
        private Transform _uiRootTransform;
        [SerializeField]
        private List<GameObject> _uiPrefabs;
        [SerializeField]
        private DiagnosticsProvider _diagnosticsProvider;
        [SerializeField]
        private AudioController _audioController;
        [SerializeField]
        private ThemeProfileProvider _themeProfileProvider;
        [SerializeField]
        private GameConfig _gameConfig;
        private ILogger _logger;

        private void OnApplicationQuit()
        {
            var gameFlow = Container.Resolve<IGameFlow>();
            if (gameFlow.State != GameState.Exit)
            {
                gameFlow.Exit();
            }
        }
        private void OnUnobservedTaskException(System.Exception ex)
        {
            _logger ??= Container.TryResolve<ILogger>();
            _logger?.LogError(nameof(ProjectInstaller), string.Format("Unobserved task exception {0}", ex));
        }

        private void InstallLogging()
        {
#if DEBUG
            Container.Bind<ILogger>().To<Logger>().FromNew().AsSingle().WithArguments(new DebugLogHandler());
#else
            Container.Bind<ILogger>().To<StubLogger>().FromNew().AsSingle();
#endif
        }
        private void InstallSettings()
        {
            Container.Bind<GameSettings>().To<GameSettings>().FromNew().AsSingle();
        }
        private void InstallInput()
        {
            Container.BindInterfacesAndSelfTo<InputActionService>().FromNew().AsSingle();
        }
        private void InstallGameStateMachine()
        {
            Container.Bind<IStateMachine<GameState>>()
                .To<StateMachine<GameState>>()
                .AsSingle()
                .WithArguments(new GameStateGraph())
                .OnInstantiated((context, instance) =>
                {
                    if (instance is IStateMachine<GameState> stateMachine)
                    {
                        context.Container.Bind<IStateSwitcher<GameState>>().FromInstance(stateMachine.StateSwitcher);
                    }
                })
                .NonLazy();
            var stateTypes = ReflectionUtilities.GetInheritorTypes(typeof(IStateHandler<GameState>), System.Type.EmptyTypes);
            foreach (var stateType in stateTypes)
            {
                Container.BindInterfacesTo(stateType).AsSingle();
            }
            Container.BindInterfacesTo<ManagedStateMachine<GameState>>().AsSingle().OnInstantiated((context, target) =>
            {
                if (target is ManagedStateMachine<GameState> managedStateMachine)
                {
                    managedStateMachine.Start();
                }
            }).NonLazy();
        }
        private void InstallViewFactory()
        {
            if (_uiRootTransform == null)
            {
                _uiRootTransform = FindFirstObjectByType<Canvas>(FindObjectsInactive.Exclude).transform;
                if (_uiRootTransform == null)
                {
                    var canvasGO = new GameObject(nameof(Canvas));
                    var canvas = canvasGO.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasGO.AddComponent<CanvasScaler>();
                    canvasGO.AddComponent<GraphicRaycaster>();
                    _uiRootTransform = canvas.transform;
                }
            }
            Container.BindInterfacesAndSelfTo<ViewFactory>().AsSingle().WithArguments(_uiRootTransform.gameObject, _uiPrefabs);
        }
        private void InstallPresenterBindings()
        {
            Container.BindInterfacesAndSelfTo<PresenterNavigator>().AsTransient();
            var presenterTypes = ReflectionUtilities.GetInheritorTypes(typeof(IPresenter));
            foreach (var presenterType in presenterTypes)
            {
                Container.BindInterfacesTo(presenterType).FromNew().AsSingle();
            }
        }
    }
}
