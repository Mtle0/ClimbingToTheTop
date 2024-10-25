using StarterAssets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Edge : MonoBehaviour, IClimbable
{
    private float moveSpeed = 2f;
    float offsetplayerCornerDist = 2f;
    private ClimbingManager climbingManager;

    private Vector3 leftPoint;
    private Vector3 rightPoint;

    void Start()
    {
        CalculateBounds();
        climbingManager = GameManager.Instance.ClimbingManager;
    }

    private void CalculateBounds()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;

            rightPoint = bounds.center - transform.right * bounds.extents.x;
            leftPoint = bounds.center + transform.right * bounds.extents.x;
        }
        else
        {
            Debug.LogError("Renderer non trouvé sur l'objet " + gameObject.name);
        }
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
        float offsetY = 0.21f;
        float offsetZ = 0.1f;
        Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y + offsetY, transform.position.z + offsetZ) + transform.forward * 0.4f;
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
        climbingManager.playerAnimationController.Animator.SetInteger(climbingManager.playerAnimationController.animeIDClimbingEdgeAxisX, (int)_inputDirection.x);
        if (_inputDirection.x > 0) { MoveRight(); }
        else if (_inputDirection.x < 0) { MoveLeft(); }
    }

    private void MoveLeft()
    {
        if (climbingManager.transform.position.x > leftPoint.x)
        {
            climbingManager.playerMovement.Move(transform.right * moveSpeed);
        }
    }

    private void MoveRight()
    {
        if (climbingManager.transform.position.x < rightPoint.x)
        {
            climbingManager.playerMovement.Move(-transform.right * moveSpeed);
        }
    }

    public bool StopClimbingCondition(StarterAssetsInputs _input)
    {
        return false;
    }

    public void EndClimb()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leftPoint, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(rightPoint, 0.1f);
    }
}
