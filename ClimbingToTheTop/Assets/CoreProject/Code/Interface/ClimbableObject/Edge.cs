using StarterAssets;
using System;
using System.Collections;
using UnityEngine;

public class Edge : MonoBehaviour, IClimbable
{
    private float moveSpeed = 2f;

    private ClimbingManager climbingManager;

    public void Start()
    {
        climbingManager = GameManager.Instance.ClimbingManager;
    }

    public void StartClimbingCondition()
    {
        if (!climbingManager.playerMovement.Grounded)
        {
            climbingManager.enableBasicMovement = false;
            climbingManager.playerAnimationController.Animator.SetTrigger(climbingManager.playerAnimationController.animeIDClimbingGroundToEdge);
            StartCoroutine(AdjusteToEdgeCoroutine());
            StartCoroutine(LookAtEdgeCoroutine());
        }
    }

    private IEnumerator AdjusteToEdgeCoroutine()
    {
        Transform playerTransform = climbingManager.neckPosition;
        Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z) + transform.forward * 0.4f;
        float replaceSpeed = 5f;

        bool isAdjusteToEdge = false;

        while (!isAdjusteToEdge)
        {
            ReplacePlayer(climbingManager, playerTransform, targetPosition, replaceSpeed);

            if (Math.Abs(playerTransform.position.y - targetPosition.y) <= 0.1f && Math.Abs(playerTransform.position.z - targetPosition.z) <= 0.1f)
            {
                isAdjusteToEdge = true;
            }

            yield return null;
        }

        climbingManager.IsClimbing = true;
        //climbingManager.playerAnimationController.Animator.SetTrigger(climbingManager.playerAnimationController.animeIDClimbingLadder);
    }

    private IEnumerator LookAtEdgeCoroutine()
    {
        Transform playerTransform = climbingManager.transform;
        float targetRotationY = transform.rotation.y - 180;
        float replaceSpeed = 100f;

        bool isLookAtEdge = false;

        while (!isLookAtEdge)
        {
            Quaternion currentRotation = playerTransform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, targetRotationY, 0);
            playerTransform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, replaceSpeed * Time.deltaTime);

            if (playerTransform.rotation.y > targetRotation.y - 0.01f && playerTransform.rotation.y < targetRotation.y + 0.01f)
            {

                isLookAtEdge = true;
            }

            yield return null;
        }
    }

    private void ReplacePlayer(ClimbingManager _climbingManager, Transform _playerTransform, Vector3 _targetPosition, float _replaceSpeed)
    {
        if (_playerTransform.position.y < _targetPosition.y)
        {
            _climbingManager.playerMovement.Move(new Vector3(0, _replaceSpeed, 0));
        }
        else
        {
            _climbingManager.playerMovement.Move(new Vector3(0, -_replaceSpeed, 0));
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
        float horizontalSpeed = _inputDirection.x * moveSpeed;
        Vector3 moveDirection = new Vector3(horizontalSpeed, 0.0f, 0.0f);

        climbingManager.playerMovement.Move(moveDirection);
    }

    public bool StopClimbingCondition(StarterAssetsInputs _input)
    {
        return false;
    }

    public void EndClimb()
    {

    }
}
