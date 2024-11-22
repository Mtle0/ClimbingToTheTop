using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
            GameObject obj = new GameObject(nameof(GameManager));
            obj.AddComponent<GameManager>();

            return _instance;
        }
    }

    public ClimbingManager ClimbingManager { get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            transform.parent = null;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else
        {
            Destroy(_instance);
            return;
        }

        ClimbingManager = FindAnyObjectByType<ClimbingManager>();
    }
}
