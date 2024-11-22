using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator _animator;
    public Animator Animator => _animator;
    private PlayerMovement _movement;
    public bool HasAnimator { get; private set; }

    public float AnimationBlend { get; set; }

    //parameter
    [HideInInspector]public int animIDSpeed, animIDGrounded, animIDJump, animIDFreeFall, animIDMotionSpeed;
    [HideInInspector] public int animeIDClimbingLadder, animeIDClimbingToTopLadder;
    [HideInInspector] public int animeIDIdleOnLadder;
    [HideInInspector] public int animeIDClimbingToBottomLadder;
    [HideInInspector] public int animeIDClimbingGroundToEdge, animeIDClimbingEdgeAxisX, animeIDEdgeToTop, animeIDEdgeToGround;

    private void Start()
    {
        HasAnimator = TryGetComponent(out _animator);
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animeIDClimbingLadder = Animator.StringToHash("AttachLadder");
        animeIDClimbingToTopLadder = Animator.StringToHash("GoTopOfLadder");
        animeIDClimbingToBottomLadder = Animator.StringToHash("GoToBottomLadder");
        animeIDIdleOnLadder = Animator.StringToHash("IsMovingOnLadder");
        animeIDClimbingGroundToEdge = Animator.StringToHash("AttachToEdge");
        animeIDClimbingEdgeAxisX = Animator.StringToHash("EdgeMovingAxisX");
        animeIDEdgeToTop = Animator.StringToHash("EdgeToTop");
        animeIDEdgeToGround = Animator.StringToHash("EdgeToGround");
    }

    public void Update()
    {
        HasAnimator = TryGetComponent(out _animator);
    }
}