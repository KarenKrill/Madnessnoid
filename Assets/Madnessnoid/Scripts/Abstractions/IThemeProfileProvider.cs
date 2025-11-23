#nullable enable

using System;

namespace Madnessnoid.Abstractions
{
    public interface IThemeProfileProvider
    {
        public IThemeProfile ActiveTheme { get; }

        public event Action? ActiveThemeChanged;
    }
}
