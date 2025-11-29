using System;

using UnityEngine;
using UnityEngine.InputSystem;

using KarenKrill.UniCore.Input.Abstractions;

namespace Madnessnoid.Input
{
    using Abstractions;

    public class InputActionService : IBasicActionsProvider,
        IUIActionsProvider,
        IPlayerActionsProvider,
        InputActionCollection.IPlayerActions,
        InputActionCollection.IUIActions
    {
        #region Player Actions Info & Events

        public Vector2 LastMoveDelta { get; private set; }
        public bool IsSprintActive { get; private set; }
        public bool IsAttackActive { get; private set; }

        public event Action MoveStarted;
        public event MoveDelegate Move;
        public event Action MoveCancel;
        public event Action Sprint;
        public event Action SprintCancel;
        public event Action Attack;
        public event Action AttackCancel;
        public event Action Pause;

        #endregion

        #region UI Actions Info & Events

        public Vector2 LastNavigateValue { get; private set; }
        public Vector2 LastPointValue { get; private set; }
        public Vector2 LastScrollWheelValue { get; private set; }

        public event NavigateDelegate Navigate;
        public event Action NavigateCancel;
        public event PointDelegate Point;
        public event Action PointCancel;
        public event ScrollWheelDelegate ScrollWheel;
        public event Action ScrollWheelCancel;
        public event Action Submit;
        public event Action Cancel;
        public event Action Click;
        public event Action RightClick;
        public event Action MiddleClick;

        #endregion

        public event Action<ActionMap> ActionMapChanged;

        public InputActionService(ILogger logger)
        {
            _logger = logger;
            if (_playerControls == null)
            {
                _playerControls = new();
                _playerControls.Player.SetCallbacks(this);
                _playerControls.UI.SetCallbacks(this);
            }
        }

        public void SetActionMap(ActionMap actionMap)
        {
            switch (actionMap)
            {
                case ActionMap.Player:
                    _playerControls.UI.Disable();
                    _playerControls.Player.Enable();
                    break;
                case ActionMap.UI:
                    _playerControls.Player.Disable();
                    _playerControls.UI.Enable();
                    break;
                default:
                    throw new NotImplementedException($"\"{actionMap}\" {nameof(ActionMap)} setting isn't implemented");
            }
            ActionMapChanged?.Invoke(actionMap);
            _logger.Log($"{actionMap} {nameof(ActionMap)} enabled");
        }

        public void Disable()
        {
            _playerControls.Player.Disable();
            _playerControls.UI.Disable();
            _logger.Log($"{nameof(ActionMap)}s disabled");
        }

        #region Player Actions

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                MoveStarted?.Invoke();
            }
            else if (context.performed)
            {
                var moveDelta = context.ReadValue<Vector2>();
                LastMoveDelta = moveDelta;
                Move?.Invoke(moveDelta);
            }
            else if (context.canceled)
            {
                LastMoveDelta = Vector2.zero;
                MoveCancel?.Invoke();
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                IsSprintActive = true;
                _logger.Log($"{nameof(OnSprint)} performed");
                Sprint?.Invoke();
            }
            else if (context.canceled)
            {
                IsSprintActive = false;
                _logger.Log($"{nameof(OnSprint)} canceled");
                SprintCancel?.Invoke();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                IsAttackActive = true;
                Attack?.Invoke();
            }
            else if (context.canceled)
            {
                IsAttackActive = false;
                AttackCancel?.Invoke();
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _logger.Log($"{nameof(OnPause)} performed");
                Pause?.Invoke();
            }
        }

        #endregion

        #region UI Actions

        public void OnNavigate(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var value = context.ReadValue<Vector2>();
                LastNavigateValue = value;
                Navigate?.Invoke(value);
            }
            else if (context.canceled)
            {
                LastNavigateValue = Vector2.zero;
                NavigateCancel?.Invoke();
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var value = context.ReadValue<Vector2>();
                LastPointValue = value;
                Point?.Invoke(value);
            }
            else if (context.canceled)
            {
                LastPointValue = Vector2.zero;
                PointCancel?.Invoke();
            }
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var value = context.ReadValue<Vector2>();
                LastScrollWheelValue = value;
                ScrollWheel?.Invoke(value);
            }
            else if (context.canceled)
            {
                LastScrollWheelValue = Vector2.zero;
                ScrollWheelCancel?.Invoke();
            }
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _logger.Log($"{nameof(OnSubmit)} performed");
                Submit?.Invoke();
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Cancel?.Invoke();
            }
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Click?.Invoke();
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RightClick?.Invoke();
            }
            else if (context.canceled)
            {
            }
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                MiddleClick?.Invoke();
            }
        }

        #endregion

        private readonly ILogger _logger;
        private readonly InputActionCollection _playerControls;
    }
}
