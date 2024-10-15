using StarterAssets;
using System.Collections;
using UnityEngine;

public class Ladder : MonoBehaviour, IClimbable
{
    private float climbDistance = 0.55f;
    private float moveSpeed = 2f;

    public void StartClimbingCondition()
    {
        ClimbingManager climbingManager = GameManager.Instance.ClimbingManager;
        Transform centerOfPlayer = climbingManager.centerOfPlayer;

        RaycastHit hit;
        Vector3 direction = centerOfPlayer.forward;

        Debug.DrawRay(centerOfPlayer.position, direction * climbDistance, Color.red);
        if (Physics.Raycast(centerOfPlayer.position, direction, out hit, climbDistance))
        {
            IClimbable climbable = hit.collider.GetComponent<IClimbable>();
            if (climbable != null)
            {
                climbingManager.currentClimbable = this;
                climbingManager.IsClimbing = true;
            }
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
        Transform NeckOfPlayer = climbingManager.neckPosition;

        RaycastHit hit;
        Vector3 direction = NeckOfPlayer.forward;

        Debug.DrawRay(NeckOfPlayer.position, direction * climbDistance * 2, Color.yellow);
        if (Physics.Raycast(NeckOfPlayer.position, direction, out hit, climbDistance * 2))
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
        StartCoroutine(GoToTopLadderCoroutine(2f));
    }

    private IEnumerator GoToTopLadderCoroutine(float duration)
    {
        float elapsedTime = 0f;
        ClimbingManager climbingManager = GameManager.Instance.ClimbingManager;
        Vector3 startPosition = climbingManager.transform.position;


        while (elapsedTime < duration)
        {
            if (elapsedTime < duration/2)
            {
                climbingManager.playerMovement.Move(Vector3.up * 2);
            }
            else
            {
                climbingManager.playerMovement.Move(Vector3.forward * 1.5f);
            }

            elapsedTime += Time.deltaTime;

            yield return null; 
        }

        GameManager.Instance.ClimbingManager.IsFinishClimbing = false;
    }
}
