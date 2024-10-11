using UnityEngine;
using UnityEngine.Windows;

public class Ladder : MonoBehaviour, IClimbable
{
    private float climbDistance = 0.55f;

    public void ClimbingCondition()
    {
        Transform centerOfPlayer = ClimbingManager.Instance.GetThirdPersonController().CenterOfPlayer;

        RaycastHit hit;
        Vector3 direction = centerOfPlayer.forward;

        if (Physics.Raycast(centerOfPlayer.position, direction, out hit, climbDistance))
        {
            IClimbable climbable = hit.collider.GetComponent<IClimbable>();
            if (climbable != null)
            {
                ClimbingManager.Instance.GetThirdPersonController().IsClimbing = true;
                ClimbingManager.Instance.GetThirdPersonController().CurrentClimbable = this;
            }
        }

        Debug.DrawRay(centerOfPlayer.position, direction * climbDistance, Color.red);
    }

    public void OnClimb(Vector2 _input)
    {
        float verticalSpeed = _input.y * MoveSpeed;
        Vector3 moveDirection = new Vector3(0.0f, verticalSpeed, 0.0f);

        ClimbingManager.Instance.GetThirdPersonController().MoveCharacter(moveDirection);
    }

    public void EndClimb()
    {

    }
}
