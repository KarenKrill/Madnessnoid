using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;

namespace KarenKrill.ContentLoading
{
    using Abstractions;

    public class SceneLoader : ISceneLoader
    {
        public async Task LoadAsync(string sceneName,
            SceneLoadParameters? loadParameters = null,
            CancellationToken cancellationToken = default)
            => await LoadUniAsync(sceneName, loadParameters ?? SceneLoadParameters.Default, cancellationToken).AsTask();

        public async Task LoadAsync(int sceneBuildIndex,
            SceneLoadParameters? loadParameters = null,
            CancellationToken cancellationToken = default)
            => await LoadUniAsync(sceneBuildIndex, loadParameters ?? SceneLoadParameters.Default, cancellationToken).AsTask();

        private const float _MaxDelayedActivationSceneLoadingProgress = 0.9f;

        private async UniTask LoadUniAsync(string sceneName, SceneLoadParameters loadParameters, CancellationToken cancellationToken)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadParameters.mode);
            asyncOperation.priority = loadParameters.priority;
            asyncOperation.allowSceneActivation = loadParameters.activationRequestAction is null;
            await WaitForAsyncOperation(asyncOperation,
                loadParameters.progressAction,
                loadParameters.activationRequestAction,
                cancellationToken);
        }

        private async UniTask LoadUniAsync(int sceneBuildIndex, SceneLoadParameters loadParameters, CancellationToken cancellationToken)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, loadParameters.mode);
            asyncOperation.priority = loadParameters.priority;
            asyncOperation.allowSceneActivation = loadParameters.activationRequestAction is null;
            await WaitForAsyncOperation(asyncOperation,
                loadParameters.progressAction,
                loadParameters.activationRequestAction,
                cancellationToken);
        }

        private async UniTask WaitForAsyncOperation(AsyncOperation asyncOperation, Action<float> progressAction, ActivationRequestHandler activationRequestAction, CancellationToken cancellationToken)
        {
            Progress<float> progressReporter = null;
            if (activationRequestAction is not null)
            {
                progressReporter = new FrozableProgress<float>(progress =>
                {
                    OnProgressChanged((FrozableProgress<float>)progressReporter, progress, asyncOperation, progressAction, activationRequestAction);
                });
            }
            else if (progressAction is not null)
            {
                progressReporter = new Progress<float>(progressAction);
            }
            await asyncOperation.ToUniTask(progressReporter, cancellationToken: cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnProgressChanged(FrozableProgress<float> progress,
            float progressValue,
            AsyncOperation asyncOperation,
            Action<float> progressAction,
            ActivationRequestHandler activationRequestAction)
        {
            progressAction?.Invoke(progressValue / _MaxDelayedActivationSceneLoadingProgress);
            if (progressValue >= _MaxDelayedActivationSceneLoadingProgress)
            { // Scene is loaded and waiting to be activated
                // Freeze the progress change while waiting for the scene to activate,
                // since the AsyncOperation UniTask wrapper doesn't handle this situation and continues to raise events
                progress.isFrozen = true;
                activationRequestAction.Invoke(() => OnActivationAllowed(asyncOperation));
            }
        }

        private void OnActivationAllowed(AsyncOperation asyncOperation)
        {
            asyncOperation.allowSceneActivation = true;
        }
    }
}
