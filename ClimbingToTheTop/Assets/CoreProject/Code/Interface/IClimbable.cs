using StarterAssets;
using UnityEngine;

public interface IClimbable
{
    bool AvailableToAttach { get; set;} // variable use for not go on the climbable we left

    void StartClimbingCondition();
    void OnClimb(Vector2 inputDirection);
    bool StopClimbingCondition(StarterAssetsInputs input);
    void EndClimb();
}