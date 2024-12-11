using StarterAssets;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Ladder : MonoBehaviour, IClimbable
{
    public bool AvailableToAttach { get; set; } = true;

    private const float ClimbDistance = 0.55f;
    private const float MoveSpeed = 2f;
    public Transform topOfLadder;
    public Transform bottomOfLadder;
    private ClimbingManager _climbingManager;
    private bool _endClimbOnTop = false;

    public void Start()
    {
        _climbingManager = GameManager.Instance.ClimbingManager;
    }

    public void StartClimbingCondition()
    {
        if (!_climbingManager.playerMovement.grounded || !AvailableToAttach) return;
        Transform centerOfPlayer = _climbingManager.centerOfPlayer;
        Vector3 direction = centerOfPlayer.forward;

        Debug.DrawRay(centerOfPlayer.position, direction * ClimbDistance, Color.red);
        if (!Physics.Raycast(centerOfPlayer.position, direction, out var hit, ClimbDistance)) return;
        if (hit.collider != GetComponent<Collider>()) return;
        _climbingManager.CurrentClimbable = this;
        _climbingManager.enableBasicMovement = false;
        StartCoroutine(AdjustToClimbable());
        StartCoroutine(LookAtClimbable());
    }

    public IEnumerator AdjustToClimbable()
    {
        Transform playerTransform = _climbingManager.centerOfPlayer;
        Vector3 targetPosition = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z) + transform.forward * 0.5f;
        const float replaceSpeed = 0.5f;

        bool isFrontOfLadder = false;

        while (!isFrontOfLadder)
        {
            ReplacePlayer(_climbingManager, playerTransform, targetPosition, replaceSpeed);

            if (Vector2.Distance(new Vector2(playerTransform.position.x, playerTransform.position.z), new Vector2(targetPosition.x, targetPosition.z)) < 0.1f)
            {
                isFrontOfLadder = true;
            }

            yield return null;
        }
        _climbingManager.playerAnimationController.Animator.SetTrigger(_climbingManager.playerAnimationController.animeIDClimbingLadder);

        _climbingManager.SetVariableWhenAttachOnClimbable(false);
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

    private static void ReplacePlayer(ClimbingManager climbingManager, Transform playerTransform, Vector3 targetPosition, float replaceSpeed)
    {
        climbingManager.playerMovement.Move(playerTransform.position.x < targetPosition.x
            ? new Vector3(replaceSpeed, 0, 0)
            : new Vector3(-replaceSpeed, 0, 0));

        climbingManager.playerMovement.Move(playerTransform.position.z < targetPosition.z
            ? new Vector3(0, 0, replaceSpeed)
            : new Vector3(0, 0, -replaceSpeed));
    }

    public void OnClimb(Vector2 inputDirection)
    {
        float verticalSpeed = inputDirection.y * MoveSpeed;
        Vector3 moveDirection = new Vector3(0.0f, verticalSpeed, 0.0f);
        _climbingManager.playerMovement.Move(moveDirection);

        _climbingManager.playerAnimationController.Animator.SetBool(_climbingManager.playerAnimationController.animeIDIdleOnLadder, verticalSpeed != 0);
    }

    public bool StopClimbingCondition(StarterAssetsInputs input)
    {
        Transform neckOfPlayer = _climbingManager.neckPosition;
        Transform footOfPlayer = _climbingManager.footPosition;

        if (neckOfPlayer.position.y >= topOfLadder.position.y)
        {
            _endClimbOnTop = true;
            return true;
        }

        if (!Mathf.Approximately(input.move.y, -1) ||
            !(math.abs(footOfPlayer.position.y - bottomOfLadder.position.y) <= 0.2f)) return false;
        _endClimbOnTop = false;
        return true;
    }


    public void EndClimb()
    {
        AvailableToAttach = false;
        if (_endClimbOnTop)
        {
            _climbingManager.playerAnimationController.Animator.SetTrigger(_climbingManager.playerAnimationController.animeIDClimbingToTopLadder);
            StartCoroutine(GoToTopLadderCoroutine());
        }
        else
        {
            _climbingManager.playerAnimationController.Animator.SetTrigger(_climbingManager.playerAnimationController.animeIDClimbingToBottomLadder);
            StartCoroutine(GoToBottomOfLadderCoroutine());
        }
    }

    private IEnumerator GoToTopLadderCoroutine()
    {
        while (_climbingManager.footPosition.position.y < topOfLadder.position.y)
        {
            _climbingManager.playerMovement.Move(Vector3.up * 1.5f);
            _climbingManager.playerMovement.Move(_climbingManager.transform.forward * 1.5f);
            yield return null;
        }
        float deltaTime = 0f;
        while (deltaTime < .3f)
        {
            deltaTime += Time.deltaTime;
            yield return null;
            _climbingManager.playerMovement.Move(_climbingManager.transform.forward * MoveSpeed);
        }
        _climbingManager.enableBasicMovement = true;
    }

    private IEnumerator GoToBottomOfLadderCoroutine()
    {
        Transform playerTransform = _climbingManager.transform;
        Quaternion startRotation = playerTransform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        for (float alpha = 0f; alpha < 1f; alpha += Time.deltaTime * 2)
        {
            playerTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, alpha);
            yield return null;
        }
        _climbingManager.playerMovement.Move(_climbingManager.transform.forward * 4);
        playerTransform.rotation = targetRotation;
        _climbingManager.enableBasicMovement = true;
    }
}