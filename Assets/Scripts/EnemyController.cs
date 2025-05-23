using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Controllers")]
    public PlayerController player;
    public TurnManager turnManager;

    [Header("Enemy Stats")]
    public int enemyDamage;
    public int enemy_current_health = 100;
    public int enemy_max_health = 100;
    public bool enemy_hurting;
    private Animator anim;
    public float enemy_waitTimeToHurt;
    public float enemy_waitTimeToRecover;

    [Header("Health Settings")]
    public Slider enemy_healthBar;
    public Button enemy_recoverButton;
    public float enemy_healthUpdateDuration = 1f;


     void Start()
    {
        anim = GetComponent<Animator>();   
    }
    public void PerformAction()
    {
        // Simula um ataque ao player
        Debug.Log("Enemy attacks!");
        player.ReceiveDamage(enemyDamage);
        // Aqui pode adicionar animação e barra de vida também
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
            Debug.Log("Morreu)");
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
        enemy_recoverButton.gameObject.SetActive(true);
        yield return null;
    }

    IEnumerator EnemyRecoverAnimation()
    {
        enemy_hurting = false;
        anim.SetBool("Hurt", enemy_hurting);
        anim.ResetTrigger("Die");
        anim.SetTrigger("Recover");
        enemy_recoverButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(enemy_waitTimeToRecover);
        enemy_current_health = enemy_max_health;
        HealthBarRecover();
        enemy_healthBar.gameObject.SetActive(true);
        turnManager.gameEnded = false;
        turnManager.StartTurn(Turn.Player);
    }

    public void EnemyRecoverStart()
    {
        StartCoroutine(EnemyRecoverAnimation());
        //anim.ResetTrigger("Recover");
    }

    public void HealthBarRecover()
    {

        int previousHealth = enemy_current_health;
        enemy_current_health = enemy_max_health;
        StartCoroutine(AnimateHealthBar(previousHealth, enemy_current_health, enemy_healthUpdateDuration));

    }

}
