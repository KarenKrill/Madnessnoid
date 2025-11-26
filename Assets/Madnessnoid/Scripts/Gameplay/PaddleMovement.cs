using System.Threading;

using UnityEngine;
using UnityEngine.InputSystem;

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
        private BallController _ballController;
        [SerializeField, Min(0)]
        private float _moveSpeed = 5;
        [SerializeField, Min(0)]
        private float _startImpulseStrenth = 5;
        [SerializeField]
        private float _minX = -5;
        [SerializeField]
        private float _maxX = 5;

        private ILogger _logger;
        private IPlayerActionsProvider _playerActionsProvider;
        private readonly InputAction _touch0PressAction = new(type: InputActionType.Button, binding: "<Touchscreen>/touch0/press");
        private readonly InputAction _touch0PosAction = new(type: InputActionType.Value, binding: "<Touchscreen>/touch0/position");
        private CancellationTokenSource _moveCts;
        private float _touchDirection = 0;


        private void OnEnable()
        {
            if (Application.isMobilePlatform)
            {
                _touch0PressAction.started += OnTouch0PressStarted;
                _touch0PressAction.canceled += OnTouch0PressCanceled;
                _touch0PosAction.performed += OnTouch0PosActionPerformed;
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
                _touch0PressAction.started -= OnTouch0PressStarted;
                _touch0PressAction.canceled -= OnTouch0PressCanceled;
                _touch0PosAction.performed -= OnTouch0PosActionPerformed;
            }
            else
            {
                _playerActionsProvider.MoveStarted -= OnMoveStarted;
                _playerActionsProvider.MoveCancel -= OnMoveCancel;
                _playerActionsProvider.Attack -= OnAttack;
            }
        }

        private void UpdateTouchDirection(Vector2 pos)
        {
            bool isLeft = pos.x < Screen.width * 0.5f;
            _touchDirection = isLeft ? -1 : 1;
        }
        private void OnMoveStarted()
        {
            if (!_ballController.IsPushed)
            {
                var moveDeltaX = Application.isMobilePlatform ? _touchDirection : _playerActionsProvider.LastMoveDelta.x;
                var direction = new Vector2(moveDeltaX, 1).normalized;
                _ballController.Push(direction, _startImpulseStrenth);
            }
            _moveCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, Application.exitCancellationToken);
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
        private void OnMoveCancel()
        {
            _moveCts?.Cancel();
        }
        private void OnTouch0PressStarted(InputAction.CallbackContext ctx)
        {
            UpdateTouchDirection(ctx.ReadValue<Vector2>());
            OnMoveStarted();
        }
        private void OnTouch0PosActionPerformed(InputAction.CallbackContext ctx)
        {
            UpdateTouchDirection(ctx.ReadValue<Vector2>());
        }
        private void OnTouch0PressCanceled(InputAction.CallbackContext ctx)
        {
            _touchDirection = 0;
            _moveCts?.Cancel();
        }
        private void OnAttack()
        {
            _logger.Log("Attack works");
            if (!_ballController.IsPushed)
            {
                _ballController.Push(Vector2.up, _startImpulseStrenth);
            }
        }

        private async UniTask MoveTaskAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield();
                var moveDeltaX = Application.isMobilePlatform ? _touchDirection : _playerActionsProvider.LastMoveDelta.x;
                var moveDelta = moveDeltaX * _moveSpeed * Time.deltaTime;
                var desiredPositionX = moveDelta + _paddleTransform.position.x;
                if (desiredPositionX > _minX && desiredPositionX < _maxX)
                {
                    _paddleTransform.position += moveDelta * Vector3.right;
                }
            }
        }
    }
}
