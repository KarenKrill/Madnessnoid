using UnityEngine;
using TMPro;

using KarenKrill.UniCore.UI.Views;

namespace Madnessnoid.UI.Views
{
    using Abstractions;
    
    public class DiagnosticInfoView : ViewBehaviour, IDiagnosticsView
    {
        public string FpsText { set => _fpsText.text = value; }

        [SerializeField]
        private TextMeshProUGUI _fpsText;
    }
}