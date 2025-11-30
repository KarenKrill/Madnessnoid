using System;

using UnityEngine;

namespace Madnessnoid
{
    using Abstractions;

    [CreateAssetMenu(fileName = nameof(ThemeProfileProvider), menuName = "Scriptable Objects/" + nameof(ThemeProfileProvider))]
    public class ThemeProfileProvider : ScriptableObject, IThemeProfileProvider
    {
        public IThemeProfile ActiveTheme => _activeTheme;

        public event Action ActiveThemeChanged;

        [SerializeField]
        private ThemeProfile _activeTheme;
        [SerializeField, HideInInspector]
        private ThemeProfile _previousTheme;

#if !UNITY_EDITOR
        private void Awake() => OnValidate();
#endif

        private void OnValidate()
        {
            if (_previousTheme != _activeTheme)
            {
                _previousTheme = _activeTheme;
                ActiveThemeChanged?.Invoke();
            }
        }
    }
}