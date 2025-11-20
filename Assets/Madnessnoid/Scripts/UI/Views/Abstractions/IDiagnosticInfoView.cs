#nullable enable

using KarenKrill.UniCore.UI.Views.Abstractions;

namespace Madnessnoid.UI.Views.Abstractions
{
    public interface IDiagnosticsView : IView
    {
        public string FpsText { set; }
    }
}