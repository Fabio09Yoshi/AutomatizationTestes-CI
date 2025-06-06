using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    Vector3 player_startPosition;
    public int current_health = 100;
    public int maxHealth = 100;
    public int damage = 10;
    public int numberOfPotions = 3;
    public bool attacking;
    public bool playerIsDead;

    public bool healing;
    public bool hurting;

    public float maxDistance = 5f;
    public float waitTimeAttack;
    public float waitTimeToHeal;
    public float waitTimeToHurt;

    [Header("Health Settings")]
    public Slider healthBar;
    public float healthUpdateDuration = 1f;
    public int potionHealAmount = 20;

    [Header("Defense Settings")]
    public bool defending;
    public bool isBlocking;
    [Range(0f, 1f)] public float defenseBlock;

    public Animator anim;

    [Header("Enemy")]
    public Transform enemyPosition;
    public EnemyController enemyController;

    [Header("UI")]
    public GameObject combatPanel;
    public Button healButton;
    public TextMeshProUGUI potionCountText;
    public Button restartGame;

    [Header("TurnManager")]
    public TurnManager turnManager;

    void Start()
    {
        playerIsDead = false;
        anim = GetComponent<Animator>();
        player_startPosition = transform.position;

        //Tratando um caso de Teste - Evitando erro de que o teste execute antes dos objetos e referÍncias
        if (potionCountText != null)
        {
            potionCountText.text = numberOfPotions.ToString();
        }

    }

    public void Attack()
    {
        if (turnManager.gameEnded) return;

        attacking = true;
        transform.position = Vector3.MoveTowards(transform.position, enemyPosition.position, maxDistance);
        anim.SetBool("Attack", attacking);
        StartCoroutine(WaitAttack());
    }

    IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(waitTimeAttack);
        attacking = false;
        anim.SetBool("Attack", attacking);
        transform.position = player_startPosition;
        enemyController.EnemyReceiveDamage(damage);
        turnManager.EndTurn();
    }

    public void Defend()
    {
        if (turnManager.gameEnded) return;

        defending = true;
        isBlocking = true;
        anim.SetBool("Defend", true);

        turnManager.EndTurn(); // Permite o inimigo atacar agora
    }

    public void ExitDefense()
    {
        defending = false;
        isBlocking = false;
        anim.SetBool("Defend", false);
        anim.ResetTrigger("BlockImpact");
    }

    public void TriggerDefendImpactAnimation()
    {
        if (isBlocking)
        {
            anim.ResetTrigger("BlockImpact");
            anim.SetTrigger("BlockImpact");
            anim.Update(0f);
        }
    }

    public void Heal()
    {
        if (turnManager.gameEnded) return;

        if (numberOfPotions > 0)
        {
            healing = true;
            numberOfPotions--;
            potionCountText.text = numberOfPotions.ToString();
            anim.SetBool("Healing", healing);
            StartCoroutine(Healing());
        }
    }

    IEnumerator Healing()
    {
        int previousHealth = current_health;
        current_health = Mathf.Min(current_health + potionHealAmount, maxHealth);

        StartCoroutine(AnimateHealthBar(previousHealth, current_health, waitTimeToHeal));

        yield return new WaitForSeconds(waitTimeToHeal);

        healing = false;

        if (anim != null && anim.runtimeAnimatorController != null)
        {
            anim.SetBool("Healing", healing);
        }

        if (numberOfPotions == 0 && healButton != null && healButton.image != null)
        {
            healButton.image.color = Color.black;
            healButton.interactable = false;
        }


        if (turnManager != null)
        {
            turnManager.EndTurn();
        }

    }

    IEnumerator AnimateHealthBar(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            healthBar.value = Mathf.Lerp(from, to, t);
            yield return null;
        }
        healthBar.value = to;
    }

    public void ReceiveDamage(int amount)
    {
        if (current_health <= 0) return;

        if (isBlocking)
        {
            TriggerDefendImpactAnimation();

            amount = Mathf.FloorToInt(amount * defenseBlock);

            if (amount <= 0) return;

            int previousHealth = current_health;
            current_health = Mathf.Max(current_health - amount, 0);
            StartCoroutine(AnimateHealthBar(previousHealth, current_health, healthUpdateDuration));

            if (current_health <= 0)
            {
                StartCoroutine(Die());
            }

            return;
        }

        hurting = true;
        anim.SetBool("Hurt", true);

        int prevHealth = current_health;
        current_health = Mathf.Max(current_health - amount, 0);
        StartCoroutine(AnimateHealthBar(prevHealth, current_health, healthUpdateDuration));

        if (current_health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(HurtAnimation());
        }
    }



    IEnumerator HurtAnimation()
    {
        yield return new WaitForSeconds(waitTimeToHurt);
        hurting = false;
        anim.SetBool("Hurt", false);
    }

    IEnumerator Die()
    {
        playerIsDead = true;
        anim.SetTrigger("Die");
        turnManager.gameEnded = true;
        combatPanel.SetActive(false);
        healthBar.gameObject.SetActive(false);
        restartGame.gameObject.SetActive(true);
        turnManager.ShowWinner("Enemy");
        yield return null;
    }

    //public void SetupPlayerControllerForTesting()
    //{
    //    // Referenciamos todos os objetos que precisam de referencia
    //    // Isso evita Erros de NullReference em testes
    //    anim = gameObject.AddComponent<Animator>();
    //    //Referencias de UI
    //    healthBar = new GameObject("HealthBar").AddComponent<Slider>();
    //    healButton = new GameObject("HealButton").AddComponent<Button>();
    //    potionCountText = new GameObject("PotionText").AddComponent<TextMeshProUGUI>();
    //    combatPanel = new GameObject("CombatPanel");
    //    restartGame = new GameObject("RestartGame").AddComponent<Button>();
    //    //Referencia do Turn Manager
    //    turnManager = new GameObject("TurnManager").AddComponent<TurnManager>();
    //    turnManager.turnText = new GameObject("TurnText").AddComponent<TextMeshProUGUI>();
    //    turnManager.player = this;
    //    //Referencia do Inimigo
    //    enemyPosition = new GameObject("Enemy").transform;
    //    enemyController = new GameObject("EnemyController").AddComponent<EnemyController>();
    //}
}
