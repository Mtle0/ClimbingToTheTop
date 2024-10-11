using UnityEngine;

public interface IClimbable
{
    void ClimbingCondition();
    void OnClimb(Vector2 _input);
    void EndClimb();
}