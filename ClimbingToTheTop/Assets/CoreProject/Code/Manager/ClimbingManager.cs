using UnityEngine;
using UnityEngine.Serialization;

public class ClimbingManager : MonoBehaviour
{
    #region variable
    private bool _isClimbing = false;
    public bool IsClimbing
    {
        get => _isClimbing;
        set
        {
            _isClimbing = value;
            if (value)
            {
                handPlacementManager.StartPlaceHands(CurrentClimbable);
            }
        }
    }

    public enum PlayerForwardOnClimbable
    {
        Forward,
        Left,
        Right,
        Back
    }

    public IClimbable CurrentClimbable;
    public IClimbable LastClimbable;
    public Transform centerOfPlayer;
    public Transform neckPosition;
    [FormerlySerializedAs("FootPosition")] public Transform footPosition;
    public CollideTestWithEdge playerCollideEdge;
    [HideInInspector] public bool stopClimbingCondition = false;
    [HideInInspector] public bool enableBasicMovement = true;
    [HideInInspector] public HandPlacementManager handPlacementManager;
    [HideInInspector] public PlayerCameraController playerCameraController;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerAnimationController playerAnimationController;
    [HideInInspector] public PlayerForwardOnClimbable playerForwardOnClimbable;
    #endregion

    private void Awake()
    {
        handPlacementManager = GetComponent<HandPlacementManager>();
        playerCameraController = GetComponent<PlayerCameraController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimationController = GetComponent<PlayerAnimationController>();

        if (handPlacementManager == null || playerCameraController == null 
            || playerMovement == null || playerAnimationController == null)
        {
            Debug.LogError("ClimbingManager: Missing dependencies!");
        }
    }

    private void ResetAnimatorVariable()
    {
        playerAnimationController.Animator.SetBool(playerAnimationController.animIDFreeFall, false);
        playerAnimationController.Animator.SetBool(playerAnimationController.animIDJump, false);
    }

    public void StartClimbingTest(IClimbable climbable)
    {
        if (!climbable.AvailableToAttach) return;
        ResetAnimatorVariable();
        climbable.StartClimbingCondition();
        if (LastClimbable != null) // if != null startClimbing condition tell that we can climb
        {
            LastClimbable.AvailableToAttach = true;
        }
    }

    public void ClimbingMovement()
    {
        if (stopClimbingCondition) return;
        CurrentClimbable.OnClimb(playerMovement.Input.move);
        stopClimbingCondition = CurrentClimbable.StopClimbingCondition(playerMovement.Input);
    }
    
    public void StopClimbing()
    {
        CurrentClimbable.EndClimb();
        handPlacementManager.StopPlaceHands(CurrentClimbable);
        _isClimbing = false;
        stopClimbingCondition = false;
        LastClimbable = CurrentClimbable;
        CurrentClimbable = null;
    }

    public void SetVariableWhenAttachOnClimbable(bool switchSide)
    {
        if (switchSide)
        {
            _isClimbing = true;
        }
        else
        {
            IsClimbing = true;
        }
        
    }
}