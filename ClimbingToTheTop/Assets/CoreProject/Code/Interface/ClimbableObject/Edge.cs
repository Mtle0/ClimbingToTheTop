using StarterAssets;
using System;
using System.Collections;
using UnityEngine;

public class Edge : MonoBehaviour, IClimbable
{
    public bool availableToAttatch { get; set; } = true;

    private float moveSpeed = 2f;
    private float offsetplayerCornerDist = 0.5f;
    private ClimbingManager climbingManager;
    private bool endOnGoToGround = false;
    public Transform leftPoint;
    public Transform rightPoint;


    void Start()
    {
        climbingManager = GameManager.Instance.ClimbingManager;
    }

    public void StartClimbingCondition()
    {
        if (!climbingManager.playerMovement.Grounded && availableToAttatch)
        {
            climbingManager.currentClimbable = this;
            climbingManager.enableBasicMovement = false;
            climbingManager.playerAnimationController.Animator.SetTrigger(climbingManager.playerAnimationController.animeIDClimbingGroundToEdge);
            StartCoroutine(AdjusteToEdgeCoroutine());
            StartCoroutine(LookAtEdgeCoroutine());
        }
    }

    private IEnumerator LookAtEdgeCoroutine()
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

    private IEnumerator AdjusteToEdgeCoroutine()
    {
        Transform playerTransform = climbingManager.neckPosition;
        float replaceSpeed = 3f;
        float yDistanceThreshold = 0.1f;
        float targetY = transform.position.y + 0.175f;
        bool isAdjusteToEdge = false;

        while (!isAdjusteToEdge)
        {
            ReplacePlayer(climbingManager, playerTransform, targetY, replaceSpeed);

            if (climbingManager.playerCollideEdge.isOnEdge && Mathf.Abs(playerTransform.position.y - targetY) <= yDistanceThreshold)
            {
                isAdjusteToEdge = true;
            }

            yield return null;
        }

        climbingManager.SetVariableWhenAttatchOnClimbable();
    }

    

    private void ReplacePlayer(ClimbingManager _climbingManager, Transform _playerTransform, float _targetY, float _replaceSpeed)
    {
        if (_playerTransform.position.y < _targetY)
        {
            _climbingManager.playerMovement.Move(new Vector3(0, _replaceSpeed, 0));
        }
        else
        {
            _climbingManager.playerMovement.Move(new Vector3(0, -_replaceSpeed, 0));
        }
        _climbingManager.playerMovement.Move(_playerTransform.forward * _replaceSpeed);
    }

    public void OnClimb(Vector2 _inputDirection)
    {
        climbingManager.playerAnimationController.Animator.SetInteger(climbingManager.playerAnimationController.animeIDClimbingEdgeAxisX, (int)_inputDirection.x);
        if (_inputDirection.x > 0)
        {
            MoveRight();
        }
        else if (_inputDirection.x < 0)
        {
            MoveLeft();
        }
    }

    private void MoveLeft()
    {
        Vector2 playerPos2D = new Vector2(climbingManager.transform.position.x, climbingManager.transform.position.z);
        Vector2 leftPoint2D = new Vector2(leftPoint.position.x,leftPoint.position.z);

        if (Vector2.Distance(playerPos2D, leftPoint2D) > offsetplayerCornerDist)
        {
            climbingManager.playerMovement.Move(transform.right * moveSpeed);
        }
    }

    private void MoveRight()
    {
        Vector2 playerPos2D = new Vector2(climbingManager.transform.position.x, climbingManager.transform.position.z);
        Vector2 RightPoint2D = new Vector2(rightPoint.position.x, rightPoint.position.z);

        if (Vector2.Distance(playerPos2D, RightPoint2D) > offsetplayerCornerDist)
        {
            climbingManager.playerMovement.Move(-transform.right * moveSpeed);
        }
    }

    public bool StopClimbingCondition(StarterAssetsInputs _input)
    {

        if (_input.move.y == 1 && Input.GetKeyDown(KeyCode.Space))
        {
            endOnGoToGround = false;
            climbingManager.playerAnimationController.Animator.SetTrigger(climbingManager.playerAnimationController.animeIDEgdeToTop);
            return true;
        }

        else if (_input.move.y == -1 && Input.GetKeyDown(KeyCode.Space))
        {
            endOnGoToGround = true;
            climbingManager.playerAnimationController.Animator.SetTrigger(climbingManager.playerAnimationController.animeIDEdgeToGround);
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            endOnGoToGround = false;
            climbingManager.playerAnimationController.Animator.SetTrigger(climbingManager.playerAnimationController.animeIDEgdeToTop);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EndClimb()
    {
        availableToAttatch = false;
        if (endOnGoToGround) { StartCoroutine(GoToGroundCoroutine()); } else { StartCoroutine(GoToTopCoroutine()); }
    }

    private IEnumerator GoToGroundCoroutine()
    {
        yield return null;
        climbingManager.enableBasicMovement = true;
    }

    private IEnumerator GoToTopCoroutine()
    {
        while (climbingManager.FootPosition.position.y  < transform.position.y)
        {
            climbingManager.playerMovement.Move(Vector3.up * 1.5f);
            climbingManager.playerMovement.Move(climbingManager.transform.forward * 0.8f);
            yield return null;
        }
        climbingManager.playerMovement.Move(Vector3.up);
        climbingManager.playerMovement.Move(climbingManager.transform.forward * 4);
        climbingManager.enableBasicMovement = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leftPoint.position, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(rightPoint.position, 0.1f);
    }
}
