using UnityEngine;

public class CollideTestWithEdge : MonoBehaviour
{
    [HideInInspector] public bool isOnEdge;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isOnEdge = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            isOnEdge = false;
        }
    }
}
