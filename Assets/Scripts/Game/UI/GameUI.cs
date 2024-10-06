
using Germinator;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private RectTransform hpBar;
    [SerializeField] private TMP_Text textKills;
    [SerializeField] private TMP_Text textScore;
    [SerializeField] private TMP_Text textCombo;
    [SerializeField] private string[] comboLevelTexts;
    [SerializeField] private TMP_Text textSpeed;
    [SerializeField] private TMP_Text textAtkDamage;
    [SerializeField] private TMP_Text textAtkSpeed;
    private Animator animator;

    private string ComboLevelText(int level)
    {
        if (comboLevelTexts.Length == 0 || level < 0)
        {
            return "GOGOGO!";
        }
        if (level >= comboLevelTexts.Length)
        {
            return comboLevelTexts[comboLevelTexts.Length - 1];
        }

        return comboLevelTexts[level];
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdatePlayer(PlayerEntity player)
    {
        float hpPerc = 1 - player.data.health / player.data.maxHealth;

        hpBar.offsetMax = new Vector2(-hpPerc * 350f, 0f);
        animator.SetTrigger("Hit");
        textSpeed.text = player.data.moveSpeed.ToString();
        textAtkDamage.text = player.data.attack.damage.ToString();
        textAtkSpeed.text = player.data.attack.speed.ToString();
    }

    public void UpdateScore(int kills, int score)
    {
        textKills.text = kills.ToString();
        textScore.text = score.ToString();
        animator.SetTrigger("Kills");
        animator.SetTrigger("Score");
    }

    public void UpdateCombo(int level)
    {
        textCombo.text = ComboLevelText(level);
        animator.SetInteger("ComboLevel", level);
    }
}
