using StarterAssets;
using System.Collections;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class Ladder : MonoBehaviour, IClimbable
{
    private float climbDistance = 0.55f;
    private float moveSpeed = 2f;

    public void StartClimbingCondition()
    {
        ClimbingManager climbingManager = GameManager.Instance.ClimbingManager;
        if (climbingManager.playerMovement.Grounded && climbingManager.playerAnimationController)
        {
            Transform centerOfPlayer = climbingManager.centerOfPlayer;

            RaycastHit hit;
            Vector3 direction = centerOfPlayer.forward;

            Debug.DrawRay(centerOfPlayer.position, direction * climbDistance, Color.red);
            if (Physics.Raycast(centerOfPlayer.position, direction, out hit, climbDistance))
            {
                IClimbable climbable = hit.collider.GetComponent<IClimbable>();
                if (climbable != null)
                {
                    climbingManager.enableBasicMovement = false;
                    StartCoroutine(GoFrontToLadderCoroutine());
                    StartCoroutine(LookAtLadderCoroutine());
                }
            }
        }
    }

    private IEnumerator GoFrontToLadderCoroutine()
    {
        ClimbingManager climbingManager = GameManager.Instance.ClimbingManager;
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

        climbingManager.IsClimbing = true;
        climbingManager.playerAnimationController.Animator.SetTrigger(climbingManager.playerAnimationController.animeIDClimbingLadder);
    }

    private IEnumerator LookAtLadderCoroutine()
    {
        ClimbingManager climbingManager = GameManager.Instance.ClimbingManager;
        Transform playerTransform = climbingManager.transform;
        float targetRotationY = transform.rotation.y;
        float replaceSpeed = 100f;

        bool isLookAtLadder = false;

        while (!isLookAtLadder)
        {
            Quaternion currentRotation = playerTransform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, targetRotationY, 0);
            playerTransform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, replaceSpeed * Time.deltaTime);

            if (playerTransform.rotation.y > targetRotation.y - 0.01f && playerTransform.rotation.y < targetRotation.y + 0.01f)
            {

                isLookAtLadder = true;
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

        GameManager.Instance.ClimbingManager.playerMovement.Move(moveDirection);
    }

    public bool StopClimbingCondition(StarterAssetsInputs _input)
    {
        ClimbingManager climbingManager = GameManager.Instance.ClimbingManager;
        Transform neckOfPlayer = climbingManager.neckPosition;
        float offsetY = 0.1f;

        RaycastHit hit;
        Vector3 direction = neckOfPlayer.forward;
        Vector3 startPosition = neckOfPlayer.position + new Vector3(0, offsetY, 0); 

        Debug.DrawRay(startPosition, direction * climbDistance * 2, Color.yellow);
        if (Physics.Raycast(startPosition, direction, out hit, climbDistance * 2))
        {
            IClimbable climbable = hit.collider.GetComponent<IClimbable>();
            if (climbable != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }


    public void EndClimb()
    {   

        StartCoroutine(GoToTopLadderCoroutine(1.4f));
    }

    private IEnumerator GoToTopLadderCoroutine(float duration)
    {
        float elapsedTime = 0f;
        ClimbingManager climbingManager = GameManager.Instance.ClimbingManager;
        climbingManager.playerAnimationController.Animator.SetTrigger(climbingManager.playerAnimationController.animeIDClimbingToTopLadder);

        while (elapsedTime < duration)
        {
            if (elapsedTime < 1f)
            {
                climbingManager.playerMovement.Move(Vector3.up * 1f);
            }
            climbingManager.playerMovement.Move(Vector3.forward * 2f);
            

            elapsedTime += Time.deltaTime;

            yield return null; 
        }

        climbingManager.IsFinishClimbing = false;
        climbingManager.enableBasicMovement = true;
    }
}