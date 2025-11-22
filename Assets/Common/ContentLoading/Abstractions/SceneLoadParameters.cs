using System;

using UnityEngine.SceneManagement;

namespace KarenKrill.ContentLoading.Abstractions
{
    public delegate void ActivationRequestHandler(Action allowActivationAction);

    public readonly struct SceneLoadParameters
    {
        public static readonly SceneLoadParameters Default = new();

        public readonly LoadSceneMode mode;
        public readonly int priority;
        public readonly Action<float> progressAction;
        public readonly ActivationRequestHandler activationRequestAction;

        public SceneLoadParameters(LoadSceneMode mode = LoadSceneMode.Single,
            int priority = 0,
            Action<float> progressAction = null,
            ActivationRequestHandler activationRequestAction = null)
        {
            this.mode = mode;
            this.priority = priority;
            this.progressAction = progressAction;
            this.activationRequestAction = activationRequestAction;
        }
    }
}
