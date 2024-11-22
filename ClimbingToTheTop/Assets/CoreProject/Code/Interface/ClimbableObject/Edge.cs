using StarterAssets;
using System.Collections;
using UnityEngine;

public class Edge : MonoBehaviour, IClimbable
{
    public bool AvailableToAttach { get; set; } = true;

    private const float MoveSpeed = 2f;
    private const float OffsetPlayerCornerDist = 0.5f;
    private ClimbingManager _climbingManager;
    private bool _endOnGoToGround = false;
    public Transform leftPoint;
    public Transform rightPoint;


    private void Start()
    {
        _climbingManager = GameManager.Instance.ClimbingManager;
    }

    public void StartClimbingCondition()
    {
        if (_climbingManager.playerMovement.grounded || !AvailableToAttach) return;
        _climbingManager.CurrentClimbable = this;
        _climbingManager.enableBasicMovement = false;
        _climbingManager.playerAnimationController.Animator.SetTrigger(_climbingManager.playerAnimationController.animeIDClimbingGroundToEdge);
        StartCoroutine(AdjustToEdgeCoroutine());
        StartCoroutine(LookAtEdgeCoroutine());
    }

    private IEnumerator LookAtEdgeCoroutine()
    {
        Transform playerTransform = _climbingManager.transform;
        Vector3 playerTransformForwardStart = playerTransform.forward;
        Vector3 targetRotation = -transform.forward;
        float alpha = 0f;
        bool isLookAtEdge = false;

        while (!isLookAtEdge)
        {
            playerTransform.forward = Vector3.Lerp(playerTransformForwardStart, targetRotation, alpha);
            alpha += Time.deltaTime * 2;
            if (alpha >= 1)
            {
                isLookAtEdge = true;
            }

            yield return null;
        }
    }

    private IEnumerator AdjustToEdgeCoroutine()
    {
        Transform playerTransform = _climbingManager.neckPosition;
        const float replaceSpeed = 3f;
        const float yDistanceThreshold = 0.1f;
        float targetY = transform.position.y + 0.175f;
        bool isAdjustToEdge = false;

        while (!isAdjustToEdge)
        {
            ReplacePlayer(_climbingManager, playerTransform, targetY, replaceSpeed);

            if (_climbingManager.playerCollideEdge.isOnEdge && Mathf.Abs(playerTransform.position.y - targetY) <= yDistanceThreshold)
            {
                isAdjustToEdge = true;
            }

            yield return null;
        }

        _climbingManager.SetVariableWhenAttachOnClimbable();
    }

    

    private void ReplacePlayer(ClimbingManager climbingManager, Transform playerTransform, float targetY, float replaceSpeed)
    {
        climbingManager.playerMovement.Move(playerTransform.position.y < targetY
            ? new Vector3(0, replaceSpeed, 0)
            : new Vector3(0, -replaceSpeed, 0));

        if (this._climbingManager.playerCollideEdge.isOnEdge) return;
        Vector3 directionToEdge = (this._climbingManager.playerCollideEdge.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToEdge);

        if (dotProduct > 0) 
        {
            climbingManager.playerMovement.Move(playerTransform.forward * replaceSpeed);
        }
        else
        {
            climbingManager.playerMovement.Move(-playerTransform.forward * replaceSpeed);
        }
    }

    public void OnClimb(Vector2 inputDirection)
    {
        _climbingManager.playerAnimationController.Animator.SetInteger(_climbingManager.playerAnimationController.animeIDClimbingEdgeAxisX, (int)inputDirection.x);
        switch (inputDirection.x)
        {
            case > 0:
                MoveRight();
                break;
            case < 0:
                MoveLeft();
                break;
        }
    }

    private void MoveLeft()
    {
        Vector2 playerPos2D = new Vector2(_climbingManager.transform.position.x, _climbingManager.transform.position.z);
        Vector2 leftPoint2D = new Vector2(leftPoint.position.x,leftPoint.position.z);

        if (Vector2.Distance(playerPos2D, leftPoint2D) > OffsetPlayerCornerDist)
        {
            _climbingManager.playerMovement.Move(transform.right * MoveSpeed);
        }
    }

    private void MoveRight()
    {
        Vector2 playerPos2D = new Vector2(_climbingManager.transform.position.x, _climbingManager.transform.position.z);
        Vector2 rightPoint2D = new Vector2(rightPoint.position.x, rightPoint.position.z);

        if (Vector2.Distance(playerPos2D, rightPoint2D) > OffsetPlayerCornerDist)
        {
            _climbingManager.playerMovement.Move(-transform.right * MoveSpeed);
        }
    }

    public bool StopClimbingCondition(StarterAssetsInputs input)
    {

        if (Mathf.Approximately(input.move.y, 1) && Input.GetKeyDown(KeyCode.Space))
        {
            _endOnGoToGround = false;
            _climbingManager.playerAnimationController.Animator.SetTrigger(_climbingManager.playerAnimationController.animeIDEdgeToTop);
            return true;
        }

        if (Mathf.Approximately(input.move.y, -1) && Input.GetKeyDown(KeyCode.Space))
        {
            _endOnGoToGround = true;
            _climbingManager.playerAnimationController.Animator.SetTrigger(_climbingManager.playerAnimationController.animeIDEdgeToGround);
            return true;
        }

        if (!Input.GetKeyDown(KeyCode.Space)) return false;
        _endOnGoToGround = false;
        _climbingManager.playerAnimationController.Animator.SetTrigger(_climbingManager.playerAnimationController.animeIDEdgeToTop);
        return true;
    }

    public void EndClimb()
    {
        AvailableToAttach = false;
        StartCoroutine(_endOnGoToGround ? GoToGroundCoroutine() : GoToTopCoroutine());
    }

    private IEnumerator GoToGroundCoroutine()
    {
        yield return null;
        _climbingManager.enableBasicMovement = true;
    }

    private IEnumerator GoToTopCoroutine()
    {
        while (_climbingManager.footPosition.position.y  < transform.position.y)
        {
            _climbingManager.playerMovement.Move(Vector3.up * 1.5f);
            yield return null;
        }
        _climbingManager.playerMovement.Move(Vector3.up * 1.5f);
        _climbingManager.playerMovement.Move(_climbingManager.transform.forward * 4);
        _climbingManager.enableBasicMovement = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leftPoint.position, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(rightPoint.position, 0.1f);
    }
}
