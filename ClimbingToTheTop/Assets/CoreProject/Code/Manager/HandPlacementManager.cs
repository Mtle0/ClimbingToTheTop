using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandPlacementManager : MonoBehaviour
{
    public TwoBoneIKConstraint leftHandIK;
    public TwoBoneIKConstraint rightHandIK;
    public TwoBoneIKConstraint rightFootIK;
    public TwoBoneIKConstraint leftFootIK;
    
    private Vector3 _leftHandPosLock;
    private Vector3 _rightHandPosLock;

    private void Start()
    {
        leftHandIK.weight = 0f;
        rightHandIK.weight = 0f;
        rightFootIK.weight = 0f;
        leftFootIK.weight = 0f;
    }

    public void StartPlaceHands(IClimbable climbable)
    {
        leftHandIK.data.target.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
        rightHandIK.data.target.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
        switch (climbable)
        {
            case null:
                return;
            case Edge edge:
            {
                leftHandIK.weight = 0.5f;
                rightHandIK.weight = 0.5f;

                Vector3 targetPosition = edge.transform.position;
                leftHandIK.data.target.position = new Vector3(leftHandIK.data.target.position.x, targetPosition.y,
                    leftHandIK.data.target.position.z);
                rightHandIK.data.target.position = new Vector3(rightHandIK.data.target.position.x, targetPosition.y,
                    rightHandIK.data.target.position.z);

                MoveHands(0f, 0.42f, 0.3f);

                Quaternion currentGlobalRotation = leftHandIK.data.target.rotation;
                Quaternion additionalRotation = Quaternion.Euler(20f, 100f, 5f);
                leftHandIK.data.target.rotation = currentGlobalRotation * additionalRotation;

                currentGlobalRotation = rightHandIK.data.target.rotation;
                additionalRotation = Quaternion.Euler(0f, 150f, 170f);
                rightHandIK.data.target.rotation = currentGlobalRotation * additionalRotation;

                break;
            }
        }
    }

    private void MoveHands(float upOffset, float rightOffset, float forwardOffset)
    {
        leftHandIK.data.target.position += transform.forward * forwardOffset;
        rightHandIK.data.target.position += transform.forward * forwardOffset;
        
        leftHandIK.data.target.position -= transform.right * rightOffset;
        rightHandIK.data.target.position += transform.right * rightOffset;
        
        leftHandIK.data.target.position += transform.up * upOffset;
        rightHandIK.data.target.position += transform.up * upOffset;
    }
    
    public void StopPlaceHands(IClimbable climbable)
    {
        switch (climbable)
        {
            case null:
                return;
            case Edge:
            {
                leftHandIK.weight = 0f;
                rightHandIK.weight = 0f;
                break;
            }
        }
    }

    public void LockHands()
    {
        leftHandIK.data.target.position = _leftHandPosLock;
        rightHandIK.data.target.position = _rightHandPosLock;
    }

    public void SetLastPosHand()
    {
        _leftHandPosLock = leftHandIK.data.target.position;
        _rightHandPosLock = rightHandIK.data.target.position;
    }

    public void DecreaseWeightWithLerp(float targetWeight)
    {
        leftHandIK.weight = Mathf.Lerp(targetWeight, leftHandIK.weight, Time.deltaTime * 10f);
        rightHandIK.weight = Mathf.Lerp(targetWeight, rightHandIK.weight, Time.deltaTime * 10f);
    }
}