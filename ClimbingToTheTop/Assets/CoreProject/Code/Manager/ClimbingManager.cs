using StarterAssets;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
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
    private bool isFinishClimbing = false;
    public bool IsFinishClimbing
    {
        get { return isFinishClimbing; }
        set
        {
            isFinishClimbing = value;
        }
    }


    public IClimbable currentClimbable;
    public Transform centerOfPlayer;
    public Transform neckPosition;
    public Transform FootPosition;
    [HideInInspector] public bool stopClimbingCondition = false;
    [HideInInspector] public bool enableBasicMovement = true;
    [HideInInspector] public HandPlacementManager handPlacementManager;
    [HideInInspector] public PlayerCameraController playerCameraController;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerAnimationController playerAnimationController;
    

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

    public void StartClimbingTest(IClimbable climbable)
    {
        if (!isFinishClimbing)
        {
            currentClimbable = climbable;
            currentClimbable.StartClimbingCondition();
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
        isFinishClimbing = true;
        currentClimbable = null;
    }
}