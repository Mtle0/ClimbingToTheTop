using StarterAssets;
using UnityEngine;

public interface IClimbable
{
    void StartClimbingCondition();
    void OnClimb(Vector2 _inputDirection);
    bool StopClimbingCondition(StarterAssetsInputs _input);
    void EndClimb();
}