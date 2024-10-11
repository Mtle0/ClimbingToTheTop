using UnityEngine;

public class ProceduralHandPlacement : MonoBehaviour
{
    public Transform leftHand, rightHand;
    private Transform[] handPositions;

    public void PlaceHandsOnClimbable(IClimbable climbable)
    {
        // Logique pour placer les mains en fonction des positions spécifiques sur l'objet escaladable
        // Cela peut être des raycasts ou des points prédéfinis sur le modèle 3D
    }

}
