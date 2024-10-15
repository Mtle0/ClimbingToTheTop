using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class Edge : MonoBehaviour, IClimbable
{
    private float moveSpeed = 2f;

    public void StartClimbingCondition()
    {
        ClimbingManager climbingManager = GameManager.Instance.ClimbingManager;
        if (!climbingManager.playerMovement.Grounded)
        {
            climbingManager.currentClimbable = this;
            climbingManager.IsClimbing = true;
        }
    }

    public void OnClimb(Vector2 _inputDirection)
    {
        float horizontalSpeed = _inputDirection.x * moveSpeed;
        Vector3 moveDirection = new Vector3(horizontalSpeed, 0.0f, 0.0f);

        GameManager.Instance.ClimbingManager.playerMovement.Move(moveDirection);
    }

    public bool StopClimbingCondition(StarterAssetsInputs _input)
    {
        return false;
    }

    public void EndClimb()
    {

    }
}
