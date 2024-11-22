using UnityEngine;
using UnityEngine.Serialization;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [FormerlySerializedAs("MoveSpeed")] [Header("Player Settings")]
        public float moveSpeed = 2.0f;
        [FormerlySerializedAs("SprintSpeed")] public float sprintSpeed = 5.335f;
        [FormerlySerializedAs("RotationSmoothTime")] public float rotationSmoothTime = 0.12f;
        [FormerlySerializedAs("SpeedChangeRate")] public float speedChangeRate = 10.0f;
        [FormerlySerializedAs("JumpHeight")] public float jumpHeight = 1.2f;
        [FormerlySerializedAs("Gravity")] public float gravity = -15.0f;
        [FormerlySerializedAs("JumpTimeout")] public float jumpTimeout = 0.50f;
        [FormerlySerializedAs("FallTimeout")] public float fallTimeout = 0.15f;

        [FormerlySerializedAs("Grounded")] [Header("Grounded Settings")]
        public bool grounded = true;
        [FormerlySerializedAs("GroundedOffset")] public float groundedOffset = -0.14f;
        [FormerlySerializedAs("GroundedRadius")] public float groundedRadius = 0.28f;
        [FormerlySerializedAs("GroundLayers")] public LayerMask groundLayers;

        [FormerlySerializedAs("CinemachineCameraTarget")] [Header("Cinemachine Settings")]
        public GameObject cinemachineCameraTarget;
        [FormerlySerializedAs("TopClamp")] public float topClamp = 70.0f;
        [FormerlySerializedAs("BottomClamp")] public float bottomClamp = -30.0f;
        [FormerlySerializedAs("CameraAngleOverride")] public float cameraAngleOverride = 0.0f;
        [FormerlySerializedAs("LockCameraPosition")] public bool lockCameraPosition = false;

        // Private variables
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private int _animIDSpeed, _animIDGrounded, _animIDJump, _animIDFreeFall, _animIDMotionSpeed;
        private int _animeIDClimbingLadder, _animeIDClimbingEdge;

        [FormerlySerializedAs("_centerOfPlayer")] [SerializeField] private GameObject centerOfPlayer;
        public Transform CenterOfPlayer => centerOfPlayer.transform;

        private bool _isClimbing;
        public bool IsClimbing
        {
            get => _isClimbing;
            set
            {
                _isClimbing = value;

                if (_isClimbing)
                {
                    _verticalVelocity = 0f;
                }
                else
                {
                    _verticalVelocity = -2f;
                }
            }
        }

        public IClimbable CurrentClimbable { get; set; }

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private bool _hasAnimator;
        private const float Threshold = 0.01f;

        private bool IsCurrentDeviceMouse =>
#if ENABLE_INPUT_SYSTEM
            _playerInput.currentControlScheme == "KeyboardMouse";
#else
            false;
#endif

        private void Awake()
        {
            _mainCamera = _mainCamera ?? GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Start()
        {
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Missing dependencies. Please reinstall.");
#endif
            AssignAnimationIDs();
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            if (!IsClimbing)
            {
                JumpAndGravity();
                GroundedCheck();
            }
            Move();
        }

        private void LateUpdate() => CameraRotation();

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animeIDClimbingLadder = Animator.StringToHash("Speed");
            _animeIDClimbingEdge = Animator.StringToHash("Speed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
            grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, grounded);
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            if (IsClimbing)
            {
                ClimbingMovement();
            }
            else
            {
                RegularMove();
            }
        }

        private void ClimbLadder()
        {
            float verticalSpeed = _input.move.y * moveSpeed; 
            Vector3 moveDirection = new Vector3(0.0f, verticalSpeed, 0.0f);

            _controller.Move(moveDirection * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animeIDClimbingLadder, Mathf.Abs(verticalSpeed)); 
            }
        }

        private void MoveOnEdge()
        {
            float horizontalSpeed = _input.move.x * moveSpeed; 
            Vector3 moveDirection = new Vector3(horizontalSpeed, 0.0f, 0.0f);

            _controller.Move(moveDirection * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animeIDClimbingEdge, Mathf.Abs(horizontalSpeed));
            }
        }

        private void RegularMove()
        {
            float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (grounded)
            {
                _fallTimeoutDelta = fallTimeout;
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = jumpTimeout;
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private void ClimbingMovement()
        {
            if (CurrentClimbable != null)
            {

                if (CurrentClimbable is Ladder)
                {
                    ClimbLadder();
                }
                else if (CurrentClimbable is Edge)
                {
                    MoveOnEdge();
                }
            }
            else
            {
                Debug.LogWarning("No climbable object detected.");
            }
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = grounded ? new Color(0.0f, 1.0f, 0.0f, 0.35f) : new Color(1.0f, 0.0f, 0.0f, 0.35f);
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {

        }

        private void OnLand(AnimationEvent animationEvent)
        {

        }
    }
}
