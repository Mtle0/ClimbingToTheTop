using StarterAssets;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private StarterAssetsInputs input;
    private ClimbingManager climbingManager;
    private float speed;
    private float rotationVelocity;
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    public StarterAssetsInputs Input { get { return input; } }

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
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    public void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();
        climbingManager = GetComponent<ClimbingManager>();
    }

    public void Start()
    {
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;
    }

    public void Update()
    {
        if (climbingManager.IsClimbing)
        {
            climbingManager.ClimbingMovement();
            if (climbingManager.stopClimbingCondition)
            {
                climbingManager.StopClimbing();
            }
        }
        else if (climbingManager.enableBasicMovement)
        {
            JumpAndGravity();
            GroundedCheck();
            RegularMove();
        }
    }

    private void RegularMove()
    {
        PlayerAnimationController animationController = climbingManager.playerAnimationController;
        PlayerCameraController cameraController = climbingManager.playerCameraController;


        float targetSpeed = input.sprint ? sprintSpeed : moveSpeed;
        if (input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        animationController.AnimationBlend = Mathf.Lerp(animationController.AnimationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (animationController.AnimationBlend < 0.01f) animationController.AnimationBlend = 0f;

        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

        if (input.move != Vector2.zero)
        {
            cameraController.TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraController.MainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, cameraController.TargetRotation, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, cameraController.TargetRotation, 0.0f) * Vector3.forward;
        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

        if (animationController.HasAnimator)
        {
            animationController.Animator.SetFloat(animationController.animIDSpeed, animationController.AnimationBlend);
            animationController.Animator.SetFloat(animationController.animIDMotionSpeed, inputMagnitude);
        }
    }

    private void GroundedCheck()
    {
        PlayerAnimationController animationController = climbingManager.playerAnimationController;
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (animationController.HasAnimator)
        {
            animationController.Animator.SetBool(animationController.animIDGrounded, Grounded);
        }
    }

    private void JumpAndGravity()
    {
        PlayerAnimationController animationController = climbingManager.playerAnimationController;
        if (Grounded)
        {
            if (climbingManager.lastClimbable != null)
            {
                climbingManager.lastClimbable.availableToAttatch = true;
            }

            fallTimeoutDelta = fallTimeout;
            if (animationController.HasAnimator)
            {
                animationController.Animator.SetBool(animationController.animIDJump, false);
                animationController.Animator.SetBool(animationController.animIDFreeFall, false);
            }

            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            if (input.jump && jumpTimeoutDelta <= 0.0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (animationController.HasAnimator)
                {
                    animationController.Animator.SetBool(animationController.animIDJump, true);
                }
            }

            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else if (animationController.HasAnimator)
            {
                animationController.Animator.SetBool(animationController.animIDFreeFall, true);
            }

            input.jump = false;
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    public void Move(Vector3 _moveDirection)
    {
        controller.Move(_moveDirection * Time.deltaTime);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {

    }

    private void OnLand(AnimationEvent animationEvent)
    {

    }
}
