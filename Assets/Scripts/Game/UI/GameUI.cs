
using Germinator;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private RectTransform hpBar;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdatePlayer(PlayerEntity player)
    {
        float hpPerc = 1 - player.data.health / player.data.maxHealth;

        hpBar.offsetMax = new Vector2(-hpPerc * 350f, 0f);
        animator.SetTrigger("Hit");
    }
}
