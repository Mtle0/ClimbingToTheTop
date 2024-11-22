using StarterAssets;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;
    private ClimbingManager _climbingManager;
    private float _speed;
    private float _rotationVelocity;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _verticalVelocity;
    private const float TerminalVelocity = 53.0f;

    public StarterAssetsInputs Input { get; private set; }

    [Header("Player Settings")]
    public float moveSpeed = 2.0f;
    public float sprintSpeed = 5.335f;
    public float rotationSmoothTime = 0.12f;
    public float speedChangeRate = 10.0f;
    public float jumpHeight = 1.2f;
    public float gravity = -15.0f;
    public float jumpTimeout = 0.50f;
    public float fallTimeout = 0.15f;

    [Header("Grounded Settings")]
    public bool grounded = true;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundLayers;

    public void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Input = GetComponent<StarterAssetsInputs>();
        _climbingManager = GetComponent<ClimbingManager>();
    }

    public void Start()
    {
        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
    }

    public void Update()
    {
        if (_climbingManager.IsClimbing)
        {
            _climbingManager.ClimbingMovement();
            if (_climbingManager.stopClimbingCondition)
            {
                _climbingManager.StopClimbing();
            }
        }
        else if (_climbingManager.enableBasicMovement)
        {
            JumpAndGravity();
            GroundedCheck();
            RegularMove();
        }
    }

    private void RegularMove()
    {
        PlayerAnimationController animationController = _climbingManager.playerAnimationController;
        PlayerCameraController cameraController = _climbingManager.playerCameraController;


        float targetSpeed = Input.sprint ? sprintSpeed : moveSpeed;
        if (Input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        const float speedOffset = 0.1f;
        float inputMagnitude = Input.analogMovement ? Input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        animationController.AnimationBlend = Mathf.Lerp(animationController.AnimationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (animationController.AnimationBlend < 0.01f) animationController.AnimationBlend = 0f;

        Vector3 inputDirection = new Vector3(Input.move.x, 0.0f, Input.move.y).normalized;

        if (Input.move != Vector2.zero)
        {
            cameraController.TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraController.MainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, cameraController.TargetRotation, ref _rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, cameraController.TargetRotation, 0.0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        if (!animationController.HasAnimator) return;
        animationController.Animator.SetFloat(animationController.animIDSpeed, animationController.AnimationBlend);
        animationController.Animator.SetFloat(animationController.animIDMotionSpeed, inputMagnitude);
    }

    private void GroundedCheck()
    {
        PlayerAnimationController animationController = _climbingManager.playerAnimationController;
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        if (animationController.HasAnimator)
        {
            animationController.Animator.SetBool(animationController.animIDGrounded, grounded);
        }
    }

    private void JumpAndGravity()
    {
        PlayerAnimationController animationController = _climbingManager.playerAnimationController;
        if (grounded)
        {
            if (_climbingManager.LastClimbable != null)
            {
                _climbingManager.LastClimbable.AvailableToAttach = true;
            }

            _fallTimeoutDelta = fallTimeout;
            if (animationController.HasAnimator)
            {
                animationController.Animator.SetBool(animationController.animIDJump, false);
                animationController.Animator.SetBool(animationController.animIDFreeFall, false);
            }

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (Input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (animationController.HasAnimator)
                {
                    animationController.Animator.SetBool(animationController.animIDJump, true);
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
            else if (animationController.HasAnimator)
            {
                animationController.Animator.SetBool(animationController.animIDFreeFall, true);
            }

            Input.jump = false;
        }

        if (_verticalVelocity < TerminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    public void Move(Vector3 moveDirection)
    {
        _controller.Move(moveDirection * Time.deltaTime);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {

    }

    private void OnLand(AnimationEvent animationEvent)
    {

    }
}
