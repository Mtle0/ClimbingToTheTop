using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator _animator;
    private PlayerMovement _movement;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
    }

    public void UpdateAnimations()
    {
        // Logique pour mettre à jour les animations en fonction des mouvements du joueur
    }
}
