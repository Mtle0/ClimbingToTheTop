using UnityEngine;

public class GameManager : MonoBehaviour
{
    static private GameManager instance;
    static public GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(nameof(GameManager));
                obj.AddComponent<GameManager>();
            }

            return instance;
        }
    }

    public ClimbingManager ClimbingManager { get; private set; }

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

        ClimbingManager = FindAnyObjectByType<ClimbingManager>();
    }
}
