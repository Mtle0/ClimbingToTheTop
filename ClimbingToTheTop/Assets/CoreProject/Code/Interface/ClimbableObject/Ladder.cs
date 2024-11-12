using StarterAssets;
using System;
using System.Collections;
using UnityEngine;

public class Ladder : MonoBehaviour, IClimbable
{
    public bool availableToAttatch { get; set; } = true;

    private float climbDistance = 0.55f;
    private float moveSpeed = 2f;
    public Transform topOFLadder;
    private ClimbingManager climbingManager;

    public void Start()
    {
        climbingManager = GameManager.Instance.ClimbingManager;
    }

    public void StartClimbingCondition()
    {
        if (climbingManager.playerMovement.Grounded)
        {
            Transform centerOfPlayer = climbingManager.centerOfPlayer;
            RaycastHit hit;
            Vector3 direction = centerOfPlayer.forward;

            Debug.DrawRay(centerOfPlayer.position, direction * climbDistance, Color.red);
            if (Physics.Raycast(centerOfPlayer.position, direction, out hit, climbDistance))
            {
                if (hit.collider == GetComponent<Collider>())
                {
                    climbingManager.currentClimbable = this;
                    climbingManager.enableBasicMovement = false;
                    StartCoroutine(GoFrontToLadderCoroutine());
                    StartCoroutine(LookAtLadderCoroutine());
                }
            }
        }
    }

    private IEnumerator GoFrontToLadderCoroutine()
    {
        Transform playerTransform = climbingManager.centerOfPlayer;
        Vector3 targetPosition = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z) + transform.forward * 0.5f;
        float replaceSpeed = 0.5f;

        bool isFrontOfLadder = false;

        while (!isFrontOfLadder)
        {
            ReplacePlayer(climbingManager, playerTransform, targetPosition, replaceSpeed);

            if (Vector2.Distance(new Vector2(playerTransform.position.x, playerTransform.position.z), new Vector2(targetPosition.x, targetPosition.z)) < 0.1f)
            {
                isFrontOfLadder = true;
            }

            yield return null;
        }
        climbingManager.playerAnimationController.Animator.SetTrigger(climbingManager.playerAnimationController.animeIDClimbingLadder);

        climbingManager.SetVariableWhenAttatchOnClimbable();
    }

    private IEnumerator LookAtLadderCoroutine()
    {
        Transform playerTransform = climbingManager.transform;
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

    private void ReplacePlayer(ClimbingManager _climbingManager, Transform _playerTransform, Vector3 _targetPosition, float _replaceSpeed)
    {
        if (_playerTransform.position.x < _targetPosition.x)
        {
            _climbingManager.playerMovement.Move(new Vector3(_replaceSpeed, 0, 0));
        }
        else
        {
            _climbingManager.playerMovement.Move(new Vector3(-_replaceSpeed, 0, 0));
        }

        if (_playerTransform.position.z < _targetPosition.z)
        {
            _climbingManager.playerMovement.Move(new Vector3(0, 0, _replaceSpeed));
        }
        else
        {
            _climbingManager.playerMovement.Move(new Vector3(0, 0, -_replaceSpeed));
        }
    }

    public void OnClimb(Vector2 _inputDirection)
    {
        float verticalSpeed = _inputDirection.y * moveSpeed;
        Vector3 moveDirection = new Vector3(0.0f, verticalSpeed, 0.0f);
        climbingManager.playerMovement.Move(moveDirection);

        climbingManager.playerAnimationController.Animator.SetBool(climbingManager.playerAnimationController.AnimeIDIdleOnLadder, verticalSpeed == 0 ? false : true);
    }

    public bool StopClimbingCondition(StarterAssetsInputs _input)
    {
        Transform neckOfPlayer = climbingManager.neckPosition;

        if (neckOfPlayer.position.y >= topOFLadder.position.y)
        {
            return true;
        }
        return false;
    }


    public void EndClimb()
    {
        climbingManager.playerAnimationController.Animator.SetTrigger(climbingManager.playerAnimationController.animeIDClimbingToTopLadder);
        availableToAttatch = false;
        StartCoroutine(GoToTopLadderCoroutine());
    }

    private IEnumerator GoToTopLadderCoroutine()
    {
        float forwardOffset = 0.5f;

        while (climbingManager.FootPosition.position.y < topOFLadder.position.y)
        {
            if (climbingManager.FootPosition.position.y < topOFLadder.position.y)
            {
                climbingManager.playerMovement.Move(Vector3.up * 1.5f);
            }
            
            climbingManager.playerMovement.Move(climbingManager.transform.forward * 1.5f);
            yield return null;
        }
        climbingManager.enableBasicMovement = true;
    }
}