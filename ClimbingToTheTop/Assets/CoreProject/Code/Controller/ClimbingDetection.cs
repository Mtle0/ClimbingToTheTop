using UnityEngine;

public class ClimbingDetection : MonoBehaviour
{
    [SerializeField][Range(.5f, 2)] private float gripDistance = 1.0f;
    private const float DetectionAngle = 90f;

    private ClimbingManager _climbingManager;
    private void Awake()
    {
        _climbingManager = GetComponent<ClimbingManager>();
    }

    private void Update()
    {
        if (!_climbingManager.IsClimbing && _climbingManager.enableBasicMovement)
        {
            DetectClimbableInFront();
        }
    }

    private void DetectClimbableInFront()
    {
        Transform centerOfPlayer = _climbingManager.centerOfPlayer;
        Collider[] hitColliders = Physics.OverlapSphere(centerOfPlayer.position, gripDistance);

        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.CompareTag("Climbable")) continue;
            Vector3 directionToTarget = (hitCollider.transform.position - centerOfPlayer.position).normalized;
            directionToTarget = Vector3.ProjectOnPlane(directionToTarget, Vector3.up);

            float dotProduct = Vector3.Dot(centerOfPlayer.forward, directionToTarget);

            if (!(dotProduct > Mathf.Cos(DetectionAngle * Mathf.Deg2Rad))) continue;
            Ray ray = new Ray(centerOfPlayer.position, (hitCollider.transform.position - centerOfPlayer.position).normalized);
            if (!Physics.Raycast(ray, out RaycastHit hit, gripDistance)) continue;
            if (hit.collider != hitCollider) continue;
            IClimbable climbable = hitCollider.GetComponent<IClimbable>();
            if (climbable != null)
            {
                _climbingManager.StartClimbingTest(climbable);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Transform centerOfPlayer = _climbingManager.centerOfPlayer;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerOfPlayer.position, gripDistance);
    }
#endif
}
