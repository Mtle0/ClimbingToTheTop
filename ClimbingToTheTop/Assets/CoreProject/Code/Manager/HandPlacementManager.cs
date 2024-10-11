using UnityEngine;

public class HandPlacementManager : MonoBehaviour
{
    public void StartPlaceHands(IClimbable climbable)
    {
        if (climbable != null)
        {
            Debug.Log("Placing hands on climbable: " + climbable);
        }
    }

    public void StopPlaceHands(IClimbable climbable)
    {
        if (climbable != null)
        {
            Debug.Log("stop placing hand on: " + climbable);
        }
    }
}