using System.Collections;
using StarterAssets;
using UnityEngine;

public class Dot : MonoBehaviour, IClimbable
{
    public bool AvailableToAttach { get; set; }

    private ClimbingManager _climbingManager;
    private bool _goToGround;
    private const float MoveSpeed = 2f;

    private void Start()
    {
        _climbingManager = GameManager.Instance.ClimbingManager;
    }

    public void StartClimbingCondition()
    {
        if (_climbingManager.playerMovement.grounded || !AvailableToAttach) return;
        _climbingManager.CurrentClimbable = this;
        _climbingManager.enableBasicMovement = false;
        _climbingManager.playerAnimationController.Animator.SetTrigger(_climbingManager.playerAnimationController
            .animeIDClimbingGroundToEdge);
        StartCoroutine(AdjustToClimbable());
        StartCoroutine(LookAtClimbable());
    }

    public IEnumerator LookAtClimbable()
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

    public IEnumerator AdjustToClimbable()
    {
        Transform playerTransform = _climbingManager.neckPosition;
        const float replaceSpeed = 3f;
        const float yDistanceThreshold = 0.1f;
        float targetY = transform.position.y + 0.175f;
        bool isAdjustToEdge = false;
        while (!isAdjustToEdge)
        {
            ReplacePlayer(_climbingManager, playerTransform, targetY, replaceSpeed);
            
            if (_climbingManager.playerCollideEdge.isOnEdge &&
                Mathf.Abs(playerTransform.position.y - targetY) <= yDistanceThreshold)
            {
                isAdjustToEdge = true;
            }
            
            yield return null;
        }

        _climbingManager.SetVariableWhenAttachOnClimbable(false);
    }


    private void ReplacePlayer(ClimbingManager climbingManager, Transform playerTransform, float targetY,
        float replaceSpeed)
    {
        climbingManager.playerMovement.Move(playerTransform.position.y < targetY
            ? new Vector3(0, replaceSpeed, 0)
            : new Vector3(0, -replaceSpeed, 0));

        if (_climbingManager.playerCollideEdge.isOnEdge) return;
        Vector3 directionToDot = (_climbingManager.playerCollideEdge.transform.position - transform.position)
            .normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToDot);

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
        throw new System.NotImplementedException();
    }

    public bool StopClimbingCondition(StarterAssetsInputs input)
    {
        if (!Mathf.Approximately(input.move.y, -1) || !Input.GetKeyDown(KeyCode.Space)) return false;
        _goToGround = true;
        _climbingManager.playerAnimationController.Animator.SetTrigger(_climbingManager.playerAnimationController
            .animeIDEdgeToGround);
        return true;
    }

    public void EndClimb()
    {
        AvailableToAttach = false;
        StartCoroutine(_goToGround ? GoToGroundCoroutine() : JumpToNextDotCoroutine());
    }

    private IEnumerator GoToGroundCoroutine()
    {
        yield return null;
        _climbingManager.enableBasicMovement = true;
    }

    private IEnumerator JumpToNextDotCoroutine()
    {
        _climbingManager.handPlacementManager.SetLastPosHand();
        while (_climbingManager.footPosition.position.y < transform.position.y)
        {
            _climbingManager.playerMovement.Move(Vector3.up * 1.5f);
            _climbingManager.handPlacementManager.LockHands();
            yield return null;
        }

        float deltaTime = 0f;
        while (deltaTime < .3f)
        {
            _climbingManager.handPlacementManager.DecreaseWeightWithLerp(0f);
            deltaTime += Time.deltaTime;
            yield return null;
            _climbingManager.playerMovement.Move(_climbingManager.transform.forward * MoveSpeed);
        }

        _climbingManager.enableBasicMovement = true;
    }
}