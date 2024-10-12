using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class Edge : MonoBehaviour, IClimbable
{
    private float moveSpeed = 2f;

    public void ClimbingCondition()
    {
        ClimbingManager climbingManager = GameManager.Instance.ClimbingManager;
        if (!climbingManager.playerMovement.Grounded)
        {
            climbingManager.IsClimbing = true;
            climbingManager.currentClimbable = this;
        }
    }

    public void OnClimb(Vector2 _input)
    {
        float horizontalSpeed = _input.x * moveSpeed;
        Vector3 moveDirection = new Vector3(horizontalSpeed, 0.0f, 0.0f);

        GameManager.Instance.ClimbingManager.playerMovement.Move(moveDirection);
    }

    public void EndClimb()
    {

    }
}
