using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    public Animator Animator {  get { return animator; } }
    private PlayerMovement _movement;
    private bool hasAnimator;
    public bool HasAnimator { get { return hasAnimator; } }
    private float animationBlend;
    public float AnimationBlend { get { return animationBlend; } set { animationBlend = value; } }
    //parametter
    [HideInInspector]public int animIDSpeed, animIDGrounded, animIDJump, animIDFreeFall, animIDMotionSpeed;
    [HideInInspector] public int animeIDClimbingLadder, animeIDClimbingToTopLadder, AnimeIDIdleOnLadder, AnimeIDClimbingToBottomLadder;
    [HideInInspector] public int animeIDClimbingGroundToEdge, animeIDClimbingEdgeAxisX, animeIDEgdeToTop, animeIDEdgeToGround;

    void Start()
    {
        hasAnimator = TryGetComponent(out animator);
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animeIDClimbingLadder = Animator.StringToHash("AttatchLadder");
        animeIDClimbingToTopLadder = Animator.StringToHash("GoTopOfLadder");
        AnimeIDClimbingToBottomLadder = Animator.StringToHash("GoToBottomLadder");
        AnimeIDIdleOnLadder = Animator.StringToHash("IsMovingOnLadder");
        animeIDClimbingGroundToEdge = Animator.StringToHash("AttachToEdge");
        animeIDClimbingEdgeAxisX = Animator.StringToHash("EdgeMovingAxisX");
        animeIDEgdeToTop = Animator.StringToHash("EdgeToTop");
        animeIDEdgeToGround = Animator.StringToHash("EdgeToGround");
    }

    public void Update()
    {
        hasAnimator = TryGetComponent(out animator);
    }
}
