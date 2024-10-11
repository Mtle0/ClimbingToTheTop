using StarterAssets;
using UnityEngine;

public class ClimbingDetection : MonoBehaviour
{
    [SerializeField][Range(.5f, 2)] private float gripDistance;
    private float detectionAngle = 90;

    void Update()
    {
        if(!ClimbingManager.Instance.GetThirdPersonController().IsClimbing)
        {
            FrontClimbingTest();
        }
        
    }

    //ClimbingTest for climbable in front of player (think I will add more fonction for jump climbing and jump behind)
    void FrontClimbingTest()
    {
        Transform centerOfPlayer = ClimbingManager.Instance.GetThirdPersonController().CenterOfPlayer;
        Collider[] hitColliders = Physics.OverlapSphere(centerOfPlayer.position, gripDistance);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Climbable"))
            {
                // Project direction vector on Y plan for ignor the height difference
                Vector3 directionToTarget = (hitCollider.transform.position - centerOfPlayer.position).normalized;
                directionToTarget = Vector3.ProjectOnPlane(directionToTarget, Vector3.up);
                float dotProduct = Vector3.Dot(centerOfPlayer.forward, directionToTarget);

                if (dotProduct > Mathf.Cos(detectionAngle * Mathf.Deg2Rad))
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
            }
        }
    }
}
#endif
