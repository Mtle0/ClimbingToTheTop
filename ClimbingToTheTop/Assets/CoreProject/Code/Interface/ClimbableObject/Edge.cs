using UnityEngine;

public class Edge : MonoBehaviour, IClimbable
{

    public void ClimbingCondition()
    {
        if (!ClimbingManager.Instance.GetThirdPersonController().Grounded)
        {
            OnClimb();
        }
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
