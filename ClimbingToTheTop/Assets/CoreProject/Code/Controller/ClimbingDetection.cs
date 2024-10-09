using StarterAssets;
using UnityEngine;

public class ClimbingDetection : MonoBehaviour
{
    [SerializeField][Range(.5f, 2)] private float gripDistance;
    [SerializeField][Range(0, 90)] private float detectionAngle;

    void Update()
    {
        if(!ClimbingManager.Instance.GetThirdPersonController().IsClimbing)
        {
            StartClimbingTest();
        }
        
    }

    //ClimbingTest for climbable in front of player (think I will add more fonction for jump climbing and jump behind)
    void StartClimbingTest()
    {
        Transform centerOfPlayer = ClimbingManager.Instance.GetThirdPersonController().CenterOfPlayer;

        Collider[] hitColliders = Physics.OverlapSphere(centerOfPlayer.position, gripDistance);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Climbable"))
            {
                Vector3 directionToTarget = new Vector3(hitCollider.transform.position.x - centerOfPlayer.position.x,0,hitCollider.transform.position.z - centerOfPlayer.position.z).normalized;
                float angle = Vector3.Angle(centerOfPlayer.forward, directionToTarget);
                if (angle < detectionAngle)
                {
                    IClimbable climbable = hitCollider.GetComponent<IClimbable>();
                    if (climbable != null)
                    {
                        climbable.ClimbingCondition();
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            ThirdPersonController controller = ClimbingManager.Instance.GetThirdPersonController();

            if (controller != null)
            {
                Transform centerOfPlayer = controller.CenterOfPlayer;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(centerOfPlayer.position, gripDistance);

                DrawDetectionCone(centerOfPlayer);
            }
        }
    }

    private void DrawDetectionCone(Transform _centerOfPlayer)
    {
        Vector3 forward = _centerOfPlayer.forward * gripDistance;

        Vector3 rightLimit = Quaternion.Euler(0, detectionAngle, 0) * forward;
        Vector3 leftLimit = Quaternion.Euler(0, -detectionAngle, 0) * forward;

        Vector3 origin = _centerOfPlayer.position;



        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + rightLimit);
        Gizmos.DrawLine(origin, origin + leftLimit);
    }
}
#endif
