using StarterAssets;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
    public bool IsClimbing { get; set; }
    public IClimbable currentClimbable;
    public HandPlacementManager handPlacementManager;
    public PlayerCameraController playerCameraController;
    public PlayerMovement playerMovement;
    public Transform CenterOfPlayer { get;}

    private void Awake()
    {
        handPlacementManager = GetComponent<HandPlacementManager>();
        playerCameraController = GetComponent<PlayerCameraController>();
        playerMovement = GetComponent<PlayerMovement>();

        if (handPlacementManager == null || playerCameraController == null || playerMovement == null)
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