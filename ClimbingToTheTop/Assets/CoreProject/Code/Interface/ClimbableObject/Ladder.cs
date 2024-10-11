using UnityEngine;

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
                climbable.OnClimb();
            }
        }

        Debug.DrawRay(centerOfPlayer.position, direction * climbDistance, Color.red);
    }

    public void OnClimb()
    {
        ClimbingManager.Instance.GetThirdPersonController().IsClimbing = true;
        ClimbingManager.Instance.GetThirdPersonController().CurrentClimbable = this;
    }

    public void EndClimb()
    {

    }
}
