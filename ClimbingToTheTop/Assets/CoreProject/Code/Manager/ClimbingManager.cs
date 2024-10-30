using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
    #region variable
    private bool isClimbing = false;
    public bool IsClimbing
    {
        get { return isClimbing; }
        set
        {
            isClimbing = value;
            if (value)
            {
                handPlacementManager.StartPlaceHands(currentClimbable);
            }
        }
    }

    public enum PlayerForwardOnClimbable
    {
        FORWARD,
        LEFT,
        RIGHT,
        BACK
    }

    public IClimbable currentClimbable;
    public IClimbable lastClimbable;
    public Transform centerOfPlayer;
    public Transform neckPosition;
    public Transform FootPosition;
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

        if (handPlacementManager == null || playerCameraController == null || playerMovement == null || playerAnimationController == null)
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
        if (climbable.availableToAttatch)
        {
            ResetAnimatorVariable();
            climbable.StartClimbingCondition();
            if (lastClimbable != null)
            {
                lastClimbable.availableToAttatch = true;
            }
        }
    }

    public void ClimbingMovement()
    {
        if (!stopClimbingCondition)
        {
            currentClimbable.OnClimb(playerMovement.Input.move);
            stopClimbingCondition = currentClimbable.StopClimbingCondition(playerMovement.Input);
        }
    }

    public void StopClimbing()
    {
        currentClimbable.EndClimb();
        handPlacementManager.StopPlaceHands(currentClimbable);
        isClimbing = false;
        stopClimbingCondition = false;
        lastClimbable = currentClimbable;
        currentClimbable = null;
    }

    public void SetVariableWhenAttatchOnClimbable()
    {
        isClimbing = true;
        SetPlayerForwardEnum();
    }

    public void SetPlayerForwardEnum()
    {
        if (transform.forward == new Vector3(1,0,0))
        {
            playerForwardOnClimbable = PlayerForwardOnClimbable.RIGHT;
        }
        if (transform.forward == new Vector3(-1, 0, 0))
        {
            playerForwardOnClimbable = PlayerForwardOnClimbable.LEFT;
        }
        if (transform.forward == new Vector3(0, 0, 1))
        {
            playerForwardOnClimbable = PlayerForwardOnClimbable.FORWARD;
        }
        if (transform.forward == new Vector3(0, 0, -1))
        {
            playerForwardOnClimbable = PlayerForwardOnClimbable.BACK;
        }
    }
}