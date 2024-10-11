using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class Edge : MonoBehaviour, IClimbable
{
    private float moveSpeed = 2f;

    public void ClimbingCondition()
    {
        if (!ClimbingManager.Instance.GetThirdPersonController().Grounded)
        {
            ClimbingManager.Instance.GetThirdPersonController().IsClimbing = true;
            ClimbingManager.Instance.GetThirdPersonController().CurrentClimbable = this;
        }
    }

    public void OnClimb(Vector2 _input)
    {
        float horizontalSpeed = _input.x * moveSpeed;
        Vector3 moveDirection = new Vector3(horizontalSpeed, 0.0f, 0.0f);

        GameManager.Instance.ClimbingManager.thirdPersonController.MoveCharacter(moveDirection);
    }

    public void EndClimb()
    {

    }
}
