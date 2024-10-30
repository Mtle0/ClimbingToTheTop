using StarterAssets;
using UnityEngine;

public interface IClimbable
{
    bool availableToAttatch { get; set;} // variable use for not go on the climbable we left

    void StartClimbingCondition();
    void OnClimb(Vector2 _inputDirection);
    bool StopClimbingCondition(StarterAssetsInputs _input);
    void EndClimb();
}