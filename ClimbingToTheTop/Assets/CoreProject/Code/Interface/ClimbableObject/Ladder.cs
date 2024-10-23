using StarterAssets;
using System.Collections;
using UnityEngine;

public class Ladder : MonoBehaviour, IClimbable
{
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
        StartCoroutine(GoToTopLadderCoroutine());
    }

    private IEnumerator GoToTopLadderCoroutine()
    {
        float forwardOffset = 0.5f;

        while (climbingManager.FootPosition.position.y < topOFLadder.position.y || climbingManager.FootPosition.position.z < topOFLadder.position.z + forwardOffset)
        {
            if (climbingManager.FootPosition.position.y < topOFLadder.position.y)
            {
                climbingManager.playerMovement.Move(Vector3.up * 1.5f);
            }
            
            climbingManager.playerMovement.Move(Vector3.forward * 1.5f);
            yield return null;
        }

        climbingManager.IsFinishClimbing = false;
        climbingManager.enableBasicMovement = true;
    }
}