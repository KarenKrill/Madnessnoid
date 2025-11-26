using Cysharp.Threading.Tasks;
using DG.Tweening;
using Madnessnoid;
using Madnessnoid.Abstractions;
using Madnessnoid.Input.Abstractions;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

using Zenject;

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
    private CancellationTokenSource _moveCts;


    private void OnEnable()
    {
        _playerActionsProvider.MoveStarted += OnMoveStarted;
        _playerActionsProvider.MoveCancel += OnMoveCancel;
        _playerActionsProvider.Attack += OnAttack;
    }
    private void OnDisable()
    {
        _playerActionsProvider.MoveStarted -= OnMoveStarted;
        _playerActionsProvider.MoveCancel -= OnMoveCancel;
        _playerActionsProvider.Attack -= OnAttack;
    }

    private void OnMove(Vector2 moveDelta)
    {
        _logger.Log(nameof(OnMove), moveDelta);
    }
    private void OnMoveStarted()
    {
        if (!_ballController.IsPushed)
        {
            var direction = new Vector2(_playerActionsProvider.LastMoveDelta.x, 1).normalized;
            _ballController.Push(direction, _startImpulseStrenth);
        }
        _moveCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, Application.exitCancellationToken);
        var localMoveCts = _moveCts;
        MoveTaskAsync(_moveCts.Token).ContinueWith(()=>
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
            var moveDelta = _playerActionsProvider.LastMoveDelta.x * _moveSpeed * Time.deltaTime;
            var desiredPositionX = moveDelta + _paddleTransform.position.x;
            if (desiredPositionX > _minX && desiredPositionX < _maxX)
            {
                _paddleTransform.position += moveDelta * Vector3.right;
            }
        }
    }
}
