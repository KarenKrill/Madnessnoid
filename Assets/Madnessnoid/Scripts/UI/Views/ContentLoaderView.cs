using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;

using KarenKrill.UniCore.UI.Views;

namespace Madnessnoid.UI.Views
{
    using Abstractions;

    public class ContentLoaderView : ViewBehaviour, IContentLoaderView
    {
        public float ProgressValue
        {
            set
            {
                if (_useProgressAnimation)
                {
                    var progressDiff = value - _progressValue;
                    var progressDelta = Mathf.Abs(progressDiff);
                    if (progressDelta > float.Epsilon)
                    {
                        _progressValue = value;
                        _progressImage.DOKill();
                        if (progressDiff < 0)
                        {
                            _progressImage.fillAmount = value;
                        }
                        else
                        {
                            _progressImage.DOFillAmount(value, _fullProgressDuration * progressDelta);
                        }
                    }
                }
                else
                {
                    _progressImage.fillAmount = value;
                }
            }
        }
        public Color ProgressColor { set => _progressImage.color = value; }
        public Color ProgressBackColor { set => _progressBackImage.color = value; }
        public string ProgressText { set => _progressText.text = value; }
        public string StageText { set => _stageText.text = value; }
        public Color TextColor { set { _progressText.color = value; _stageText.color = value; } }
        public Sprite BackgroundImage { set => _backgroundImage.sprite = value; }
        public string StatusText { set => _statusText.text = value; }
        public Color StatusTextColor { set { _statusText.color = value; } }
        public bool EnableContinue
        {
            set
            {
                if (value)
                {
                    if (Application.isMobilePlatform)
                    {
                        _mobileContinueAction.Enable();
                    }
                    else
                    {
                        _continueAction.Enable();
                    }
                    if (_useStatusAnimation)
                    {
                        var color = _statusText.color;
                        _statusText.color = new Color(color.r, color.g, color.b, _minStatusFadeAlpha);
                        _statusText.DOFade(_maxStatusFadeAlpha, _statusFadeDuration).SetLoops(-1, LoopType.Yoyo);
                    }
                }
                else
                {
                    if (Application.isMobilePlatform)
                    {
                        _mobileContinueAction.Disable();
                    }
                    else
                    {
                        _continueAction.Disable();
                    }
                    if (_useStatusAnimation)
                    {
                        _statusText.DOKill();
                    }
                }
            }
        }

        public event Action ContinueRequested;

        [SerializeField]
        private Image _progressImage;
        [SerializeField]
        private Image _progressBackImage;
        [SerializeField]
        TextMeshProUGUI _progressText;
        [SerializeField]
        private bool _useProgressAnimation = true;
        [SerializeField]
        private float _fullProgressDuration = 1f;
        [SerializeField]
        TextMeshProUGUI _stageText;
        [SerializeField]
        private Image _backgroundImage;
        [SerializeField]
        private TextMeshProUGUI _statusText;
        [SerializeField]
        private bool _useStatusAnimation = true;
        [SerializeField]
        private float _statusFadeDuration = 1f;
        [SerializeField, Range(0, 1)]
        private float _minStatusFadeAlpha = 0.1f;
        [SerializeField, Range(0, 1)]
        private float _maxStatusFadeAlpha = 1f;

        [SerializeField]
        private InputAction _continueAction = new("AnyKey", InputActionType.PassThrough, binding: "*/<Button>");
        private InputAction _mobileContinueAction = new("AnyTouch", InputActionType.Button, binding: "<Touchscreen>/touch0/press");
        private float _progressValue = 0;

        private void Awake()
        {
            if (Application.isMobilePlatform)
            {
                _mobileContinueAction.Disable();
            }
            else
            {
                _continueAction.Disable();
            }
        }
        private void OnEnable()
        {
            if (Application.isMobilePlatform)
            {
                _mobileContinueAction.performed += OnContinueActionPerformed;
            }
            else
            {
                _continueAction.performed += OnContinueActionPerformed;
            }
        }
        private void OnDisable()
        {
            if (Application.isMobilePlatform)
            {
                _mobileContinueAction.performed -= OnContinueActionPerformed;
            }
            else
            {
                _continueAction.performed -= OnContinueActionPerformed;
            }
        }

        private void OnContinueActionPerformed(InputAction.CallbackContext ctx)
        {
            ContinueRequested?.Invoke();
        }
    }
}
