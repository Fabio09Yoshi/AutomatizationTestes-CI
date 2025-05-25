using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Controllers")]
    public PlayerController player;
    public TurnManager turnManager;
    public Transform playerPosition;

    [Header("Enemy Stats")]
    public int enemyDamage;
    public int enemy_current_health = 100;
    public int enemy_max_health = 100;
    public bool enemy_hurting;
    public bool enemy_attacking;
    private Animator anim;
    public float enemy_waitTimeAttacking;
    public float enemy_waitTimeToHurt;
    public float enemy_waitTimeToRecover;

    private Vector3 enemy_startPosition;

    [Header("Health Settings")]
    public Slider enemy_healthBar;
    public Button enemy_recoverButton;
    public float enemy_healthUpdateDuration = 1f;

    void Start()
    {
        anim = GetComponent<Animator>();
        enemy_startPosition = transform.position;
    }

    public void PerformAction()
    {
        if (enemy_current_health <= 0 || turnManager.gameEnded) return;
        EnemyAttack();
    }

    public void EnemyAttack()
    {
        if (!enemy_attacking)
        {
            StartCoroutine(EnemyAttackRoutine());
        }
    }

    IEnumerator EnemyAttackRoutine()
    {
        enemy_attacking = true;
        anim.SetBool("Attack", true);

        Vector3 targetPosition = playerPosition.position;
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, targetPosition, elapsed / duration);
            yield return null;
        }
        transform.position = targetPosition;

        yield return new WaitForSeconds(0.25f);

        player.TriggerDefendImpactAnimation();
        player.ReceiveDamage(enemyDamage);
        //Delay
        player.ExitDefense();

        yield return new WaitForSeconds(enemy_waitTimeAttacking);




        elapsed = 0f;
        start = transform.position;
        Vector3 returnTarget = enemy_startPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, returnTarget, elapsed / duration);
            yield return null;
        }
        transform.position = returnTarget;

        anim.SetBool("Attack", false);
        enemy_attacking = false;

        turnManager.StartTurn(Turn.Player);
    }


    public void EnemyReceiveDamage(int amount)
    {
        if (enemy_current_health <= 0) return;

        enemy_hurting = true;
        anim.SetBool("Hurt", enemy_hurting);

        int previousHealth = enemy_current_health;
        enemy_current_health = Mathf.Max(enemy_current_health - amount, 0);
        StartCoroutine(AnimateHealthBar(previousHealth, enemy_current_health, enemy_healthUpdateDuration));

        if (enemy_current_health <= 0)
        {
            StartCoroutine(EnemyDie());
            Debug.Log("Morreu");
        }
        else
        {
            StartCoroutine(EnemyHurtAnimation());
        }
    }

    IEnumerator EnemyHurtAnimation()
    {
        yield return new WaitForSeconds(enemy_waitTimeToHurt);
        enemy_hurting = false;
        anim.SetBool("Hurt", enemy_hurting);
    }

    IEnumerator AnimateHealthBar(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            enemy_healthBar.value = Mathf.Lerp(from, to, t);
            yield return null;
        }
        enemy_healthBar.value = to;
    }

    IEnumerator EnemyDie()
    {
        anim.SetTrigger("Die");
        turnManager.gameEnded = true;
        turnManager.ShowWinner("Player");
        enemy_recoverButton.interactable = true;
        enemy_recoverButton.gameObject.SetActive(true);
        yield return null;
    }

    IEnumerator EnemyRecoverAnimation()
    {
        enemy_hurting = false;
        anim.SetBool("Hurt", enemy_hurting);
        anim.ResetTrigger("Die");
        anim.SetTrigger("Recover");

        enemy_recoverButton.interactable = false;
        enemy_recoverButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(enemy_waitTimeToRecover);

        int previousHealth = enemy_current_health;
        enemy_current_health = enemy_max_health;
        StartCoroutine(AnimateHealthBar(previousHealth, enemy_current_health, enemy_healthUpdateDuration));

        enemy_healthBar.gameObject.SetActive(true);
        turnManager.gameEnded = false;
        turnManager.StartTurn(Turn.Player);
    }

    public void EnemyRecoverStart()
    {
        if (!enemy_recoverButton.interactable) return;
        StartCoroutine(EnemyRecoverAnimation());
    }

    public void HealthBarRecover()
    {
        int previousHealth = (int)enemy_healthBar.value;
        StartCoroutine(AnimateHealthBar(previousHealth, enemy_max_health, enemy_healthUpdateDuration));
    }

    //public void SetupEnemyControllerForTesting()
    //{
    //    // Referenciamos todos os objetos que precisam de referencia
    //    // Isso evita Erros de NullReference em testes
    //    anim = gameObject.AddComponent<Animator>();
    //    //Referencias de UI
    //    enemy_healthBar = new GameObject("HealthBar").AddComponent<Slider>();
    //    enemy_recoverButton = new GameObject("Enemy_RecoverButton").AddComponent<Button>();
    //    //Referencia do Turn Manager
    //    turnManager = new GameObject("TurnManager").AddComponent<TurnManager>();
    //    turnManager.turnText = new GameObject("TurnText").AddComponent<TextMeshProUGUI>();
    //    turnManager.enemy = this;
    //    //Referencia do Player
    //    playerPosition = new GameObject("Player").transform;
    //    player = new GameObject("PlayerController").AddComponent<PlayerController>();
    //}
}
