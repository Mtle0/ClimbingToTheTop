using StarterAssets;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
    static private ClimbingManager instance;
    static public ClimbingManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(nameof(ClimbingManager));
                obj.AddComponent<ClimbingManager>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            Destroy(instance);
            return;
        }
    }

    public ThirdPersonController GetThirdPersonController()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            ThirdPersonController controller = player.GetComponent<ThirdPersonController>();

            if (controller != null)
            {
                return controller;
            }
            else
            {
                Debug.LogWarning("ThirdPersonController non trouvé sur l'objet avec le tag 'Player' !");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Objet avec le tag 'Player' non trouvé !");
            return null;
        }
    }
}
