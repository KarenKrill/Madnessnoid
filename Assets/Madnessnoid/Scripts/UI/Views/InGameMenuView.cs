using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using KarenKrill.UniCore.UI.Views;

namespace Madnessnoid.UI.Views
{
    using Abstractions;

    public class InGameMenuView : ViewBehaviour, IInGameMenuView
    {
        public string HitPointsCountText { set => _hitPointsCountText.text = value; }
        public Sprite HitPointIcon { set => _hitPointImage.sprite = value; }
        public string ScoreText { set => _scoreText.text = value; }
        public Sprite ScoreIcon { set => _scoreImage.sprite = value; }

        public event Action PauseRequested;

        [SerializeField]
        private TextMeshProUGUI _hitPointsCountText;
        [SerializeField]
        private Image _hitPointImage;
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        [SerializeField]
        private Image _scoreImage;
        [SerializeField]
        private Button _pauseButton;

        private void OnEnable()
        {
            _pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        }

        private void OnPauseButtonClicked() => PauseRequested?.Invoke();
    }
}