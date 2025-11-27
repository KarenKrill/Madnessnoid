using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using Cysharp.Threading.Tasks;
using Zenject;

using Madnessnoid.Input.Abstractions;

namespace Madnessnoid
{
    public class PaddleMovement : MonoBehaviour
    {
        [Inject]
        public void Initialize(ILogger logger,
            IPlayerActionsProvider playerActionsProvider)
        {
            _logger = logger;
            _playerActionsProvider = playerActionsProvider;
        }

        [SerializeField]
        private Transform _paddleTransform;
        [SerializeField]
        private SpriteRenderer _paddleRenderer;
        [SerializeField]
        private BallController _ballController;
        [SerializeField, Min(0)]
        private float _moveSpeed = 5;
        [SerializeField, Min(0)]
        private float _startImpulseStrenth = 5;
        [SerializeField]
        private float _minX = -5;
        [SerializeField]
        private float _maxX = 5;
        [SerializeField]
        private Camera _camera;

        private ILogger _logger;
        private IPlayerActionsProvider _playerActionsProvider;
        private readonly InputAction _touchPosAction = new(type: InputActionType.Value, binding: "<Touchscreen>/primarytouch/position");
        private readonly InputAction _touch0PosAction = new(type: InputActionType.Value, binding: "<Touchscreen>/touch0/position");
        private readonly InputAction _touch1PosAction = new(type: InputActionType.Value, binding: "<Touchscreen>/touch1/position");
        private readonly InputAction _touch2PosAction = new(type: InputActionType.Value, binding: "<Touchscreen>/touch2/position");
        private readonly InputAction _touch3PosAction = new(type: InputActionType.Value, binding: "<Touchscreen>/touch3/position");
        private readonly InputAction _touch4PosAction = new(type: InputActionType.Value, binding: "<Touchscreen>/touch4/position");
        private CancellationTokenSource _moveCts;
        private float _touchPosition = 0;
        private bool _isLeftTouchDirection = false;
        private float _touchDirection = 0;
        private Vector2 _paddleSize;

        private void Awake()
        {
            _paddleSize = GetRendererWorldSize(_paddleRenderer);
        }
        private void OnEnable()
        {
            if (Application.isMobilePlatform)
            {
                _touchPosAction.performed += OnTouchPosActionPerformed;
                _touch0PosAction.performed += OnTouchPosActionPerformed;
                _touch1PosAction.performed += OnTouchPosActionPerformed;
                _touch2PosAction.performed += OnTouchPosActionPerformed;
                _touch3PosAction.performed += OnTouchPosActionPerformed;
                _touch4PosAction.performed += OnTouchPosActionPerformed;
                _touchPosAction.Enable();
                _touch0PosAction.Enable();
                _touch1PosAction.Enable();
                _touch2PosAction.Enable();
                _touch3PosAction.Enable();
                _touch4PosAction.Enable();
                OnMoveStarted();
            }
            else
            {
                _playerActionsProvider.MoveStarted += OnMoveStarted;
                _playerActionsProvider.MoveCancel += OnMoveCancel;
                _playerActionsProvider.Attack += OnAttack;
            }
        }
        private void OnDisable()
        {
            if (Application.isMobilePlatform)
            {
                _touchPosAction.performed -= OnTouchPosActionPerformed;
                _touch0PosAction.performed -= OnTouchPosActionPerformed;
                _touch1PosAction.performed -= OnTouchPosActionPerformed;
                _touch2PosAction.performed -= OnTouchPosActionPerformed;
                _touch3PosAction.performed -= OnTouchPosActionPerformed;
                _touch4PosAction.performed -= OnTouchPosActionPerformed;
                _touchPosAction.Disable();
                _touch0PosAction.Disable();
                _touch1PosAction.Disable();
                _touch2PosAction.Disable();
                _touch3PosAction.Disable();
                _touch4PosAction.Disable();
                OnMoveCancel();
            }
            else
            {
                _playerActionsProvider.MoveStarted -= OnMoveStarted;
                _playerActionsProvider.MoveCancel -= OnMoveCancel;
                _playerActionsProvider.Attack -= OnAttack;
            }
        }
        private bool IsTouchToTheLeftFromPaddleCenter(Vector2 paddlePos)
        {
            return _touchPosition < (paddlePos.x - _paddleSize.x / 2);
        }
        Vector2 GetRendererWorldSize(SpriteRenderer sr) => Vector2.Scale(sr.size, sr.transform.lossyScale);
        private void UpdateTouchDirection(Vector2 touchPos)
        {
            var pos = ScreenToWorld2D(touchPos);
            _touchPosition = pos.x;
            _isLeftTouchDirection = IsTouchToTheLeftFromPaddleCenter(_paddleTransform.position);
            _touchDirection = _isLeftTouchDirection ? -1 : 1;
        }
        private Vector3 ScreenToWorld2D(Vector2 touchPos)
        {
            var rect = _camera.pixelRect;
            // поправка на letterbox или другие искажения экрана
            touchPos.x = Mathf.Clamp(touchPos.x - rect.x, 0, rect.width);
            touchPos.y = Mathf.Clamp(touchPos.y - rect.y, 0, rect.height);
            float distance = _camera.orthographic ? Mathf.Abs(_camera.transform.position.z) : 0;
            return _camera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, distance));
        }
        private void OnMoveStarted()
        {
            if (!Application.isMobilePlatform && !_ballController.IsPushed)
            {
                var moveDeltaX = _playerActionsProvider.LastMoveDelta.x;
                var direction = new Vector2(moveDeltaX, 1).normalized;
                _ballController.Push(direction, _startImpulseStrenth);
            }
            _moveCts = CancellationTokenSource.CreateLinkedTokenSource(
                destroyCancellationToken,
                _paddleTransform.gameObject.GetCancellationTokenOnDestroy(),
                Application.exitCancellationToken);
            var localMoveCts = _moveCts;
            MoveTaskAsync(_moveCts.Token).ContinueWith(() =>
            {
                localMoveCts?.Dispose();
                if (_moveCts == localMoveCts)
                {
                    _moveCts = null;
                }
            }).Forget();
        }
        private void OnMoveCancel() => _moveCts?.Cancel();
        private void OnTouchPosActionPerformed(InputAction.CallbackContext ctx)
        {
            if (ctx.control.parent is TouchControl touchControl)
            {
                var pos = touchControl.position.value;
                UpdateTouchDirection(pos);
            }
        }
        private void OnAttack()
        {
            if (!_ballController.IsPushed)
            {
                _ballController.Push(Vector2.up, _startImpulseStrenth);
            }
        }

        private async UniTask MoveTaskAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (Application.isMobilePlatform)
                    {
                        await UniTask.WaitUntil(() => Mathf.Abs(_touchPosition - _paddleTransform.position.x) > float.Epsilon, cancellationToken: cancellationToken);
                        if (!_ballController.IsPushed)
                        {
                            var direction = new Vector2(_touchDirection, 1).normalized;
                            _ballController.Push(direction, _startImpulseStrenth);
                        }
                    }
                    else
                    {
                        await UniTask.Yield();
                    }
                    var moveDeltaX = Application.isMobilePlatform ? _touchDirection : _playerActionsProvider.LastMoveDelta.x;
                    var moveDelta = moveDeltaX * _moveSpeed * Time.deltaTime;
                    var desiredPositionX = moveDelta + _paddleTransform.position.x;
                    if (desiredPositionX > _minX && desiredPositionX < _maxX)
                    {
                        var newPosition = _paddleTransform.position + moveDelta * Vector3.right;
                        if (Application.isMobilePlatform)
                        {
                            bool isLeftDirection = IsTouchToTheLeftFromPaddleCenter(newPosition);
                            if (_isLeftTouchDirection != isLeftDirection)
                            {
                                newPosition = new Vector3(_touchPosition, newPosition.y, newPosition.z);
                                _touchDirection = 0;
                            }
                        }
                        _paddleTransform.position = newPosition;
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(nameof(PaddleMovement), ex);
            }
        }
    }
}
