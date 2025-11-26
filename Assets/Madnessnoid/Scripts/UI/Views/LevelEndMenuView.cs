using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using KarenKrill.UniCore.UI.Views;

namespace Madnessnoid.UI.Views
{
    using Abstractions;

    public class LevelEndMenuView : ViewBehaviour, ILevelEndMenuView
    {
        public string TitleText { set => _titleText.text = value; }
        public Color TitleTextColor { set => _titleText.color = value; }
        public bool EnableContinue { set => _continueButton.interactable = value; }
        public string CashRewardText { set => _cashRewardText.text = value; }
        public Sprite CashRewardIcon { set => _cashRewardIcon.sprite = value; }
        public bool EnableReward { set => _rewardPanel.gameObject.SetActive(value); }

        public event Action ContinueRequested;
        public event Action RestartRequested;
        public event Action MainMenuExitRequested;
        public event Action ExitRequested;

        [SerializeField]
        private Button _continueButton;
        [SerializeField]
        private Button _restartButton;
        [SerializeField]
        private Button _mainMenuExitButton;
        [SerializeField]
        private Button _exitButton;
        [SerializeField]
        private TextMeshProUGUI _titleText;
        [SerializeField]
        private TextMeshProUGUI _cashRewardText;
        [SerializeField]
        private Image _cashRewardIcon;
        [SerializeField]
        private RectTransform _rewardPanel;

        private void Awake()
        {
            _continueButton.interactable = false;
        }
        private void OnEnable()
        {
            _continueButton.onClick.AddListener(OnContinueButtonClicked);
            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _mainMenuExitButton.onClick.AddListener(OnMainMenuExitButtonClicked);
            _exitButton.onClick.AddListener(OnExitButtonClicked);
        }
        private void OnDisable()
        {
            _continueButton.onClick.RemoveListener(OnContinueButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            _mainMenuExitButton.onClick.RemoveListener(OnMainMenuExitButtonClicked);
            _exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        private void OnContinueButtonClicked() => ContinueRequested?.Invoke();
        private void OnRestartButtonClicked() => RestartRequested?.Invoke();
        private void OnMainMenuExitButtonClicked() => MainMenuExitRequested?.Invoke();
        private void OnExitButtonClicked() => ExitRequested?.Invoke();
    }
}