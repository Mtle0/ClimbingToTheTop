using StarterAssets;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
    public bool IsClimbing { get; set; }
    public IClimbable currentClimbable;
    public Transform centerOfPlayer;
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

    public void StartClimbing(IClimbable climbable)
    {
        currentClimbable = climbable;
        IsClimbing = true;
        handPlacementManager.StartPlaceHands(climbable);
    }

    public void ClimbingMovement()
    {
        currentClimbable.OnClimb(playerMovement.Input.move);
    }

    public void StopClimbing()
    {
        currentClimbable.EndClimb();
        handPlacementManager.StopPlaceHands(currentClimbable);
        IsClimbing = false;
        currentClimbable = null;
        
    }
}