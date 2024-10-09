using UnityEngine;

public class ClimbingDetection : MonoBehaviour
{
    [SerializeField][Range(.5f, 2)] private float gripDistance;
    [SerializeField] GameObject centerOfPlayer;

    void Update()
    {
        ClimbingOverlapSphereTest();
    }

    void ClimbingOverlapSphereTest()
    {
        Collider[] hitColliders = Physics.OverlapSphere(centerOfPlayer.transform.position, gripDistance);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Climbable"))
            {
                IClimbable climbable = hitCollider.GetComponent<IClimbable>();
                if (climbable != null)
                {
                    climbable.OnClimb();
                }
            }
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerOfPlayer.transform.position, gripDistance);
    }
}
