using UnityEngine;
using UnityEngine.Windows;

public class Ladder : MonoBehaviour, IClimbable
{
    private float climbDistance = 0.55f;
    private float moveSpeed = 2f;

    public void ClimbingCondition()
    {
        ClimbingManager climbingManager = GameManager.Instance.ClimbingManager;
        Transform centerOfPlayer = climbingManager.centerOfPlayer;

        RaycastHit hit;
        Vector3 direction = centerOfPlayer.forward;

        if (Physics.Raycast(centerOfPlayer.position, direction, out hit, climbDistance))
        {
            IClimbable climbable = hit.collider.GetComponent<IClimbable>();
            if (climbable != null)
            {
                climbingManager.IsClimbing = true;
                climbingManager.currentClimbable = this;
            }
        }

        Debug.DrawRay(centerOfPlayer.position, direction * climbDistance, Color.red);
    }

    public void OnClimb(Vector2 _input)
    {
        float verticalSpeed = _input.y * moveSpeed;
        Vector3 moveDirection = new Vector3(0.0f, verticalSpeed, 0.0f);

        GameManager.Instance.ClimbingManager.playerMovement.Move(moveDirection);
    }

    public void EndClimb()
    {

    }
}
