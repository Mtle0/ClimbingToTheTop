using UnityEngine;

public class HandPlacementManager : MonoBehaviour
{
    public static void StartPlaceHands(IClimbable climbable)
    {
        if (climbable != null)
        {
            Debug.Log("Placing hands on climbable: " + climbable);
        }
    }

    public static void StopPlaceHands(IClimbable climbable)
    {
        if (climbable != null)
        {
            Debug.Log("stop placing hand on: " + climbable);
        }
    }
}