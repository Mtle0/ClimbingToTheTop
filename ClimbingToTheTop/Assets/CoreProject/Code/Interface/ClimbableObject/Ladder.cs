using UnityEngine;

public class Ladder : MonoBehaviour, IClimbable
{
    public void OnClimb()
    {
        Debug.Log("Grip on ladder");
    }
}