using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandPlacementManager : MonoBehaviour
{
    public TwoBoneIKConstraint leftHandIK;
    public TwoBoneIKConstraint rightHandIK;
    public TwoBoneIKConstraint rightFootIK;
    public TwoBoneIKConstraint leftFootIK;

    private void Start()
    {
        leftHandIK.weight = 0f;
        rightHandIK.weight = 0f;
        rightFootIK.weight = 0f;
        leftFootIK.weight = 0f;
    }

    public void StartPlaceHands(IClimbable climbable)
    {
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

                leftHandIK.data.target.position += transform.forward * 0.22f;
                rightHandIK.data.target.position += transform.forward * 0.22f;

                Quaternion currentGlobalRotation = leftHandIK.data.target.rotation;
                
                Quaternion additionalRotation = Quaternion.Euler(150, 130f, 20f);
                leftHandIK.data.target.rotation = currentGlobalRotation * additionalRotation;

                currentGlobalRotation = rightHandIK.data.target.rotation;
                rightHandIK.data.target.rotation = currentGlobalRotation * additionalRotation;

                break;
            }
        }
    }

    public void StopPlaceHands(IClimbable climbable)
    {
        switch (climbable)
        {
            case null:
                return;
            case Edge edge:
            {
                leftHandIK.weight = 0f;
                rightHandIK.weight = 0f;
                break;
            }
        }
    }
}