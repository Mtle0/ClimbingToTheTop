using StarterAssets;
using UnityEngine;

public class ClimbingDetection : MonoBehaviour
{
    [SerializeField][Range(.5f, 2)] private float gripDistance = 1.0f;
    private float detectionAngle = 90f;

    ClimbingManager climbingManager;
    private void Awake()
    {
        climbingManager = GetComponent<ClimbingManager>();
    }

    void Update()
    {
        if (!climbingManager.IsClimbing)
        {
            DetectClimbableInFront();
        }
    }

    void DetectClimbableInFront()
    {
        Transform centerOfPlayer = climbingManager.centerOfPlayer;
        Collider[] hitColliders = Physics.OverlapSphere(centerOfPlayer.position, gripDistance);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Climbable"))
            {
                Vector3 directionToTarget = (hitCollider.transform.position - centerOfPlayer.position).normalized;
                directionToTarget = Vector3.ProjectOnPlane(directionToTarget, Vector3.up);

                float dotProduct = Vector3.Dot(centerOfPlayer.forward, directionToTarget);

                if (dotProduct > Mathf.Cos(detectionAngle * Mathf.Deg2Rad))
                {
                    IClimbable climbable = hitCollider.GetComponent<IClimbable>();
                    if (climbable != null)
                    {
                        climbingManager.StartClimbingTest(climbable);
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
            Transform centerOfPlayer = climbingManager.centerOfPlayer;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(centerOfPlayer.position, gripDistance);
        }
    }
#endif
}
