using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
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
    public bool defending;
    public bool healing;
    public bool hurting;
    public float maxDistance = 5f;
    public float waitTimeAttack;
    public float waitTimeToDefend;
    public float waitTimeToHeal;
    public float waitTimeToHurt;


    [Header("Health Settings")]
    public Slider healthBar;
    public float healthUpdateDuration = 1f; // duração da animação da barra
    public int potionHealAmount = 20;       // quanto a poção cura


    Animator anim;
    [Header("Enemy")]
    public Transform enemyPosition;
    public EnemyController enemyController;

    [Header("UI")]
    public GameObject combatPanel;
    public GameObject healtBar;
    public Button healButton;
    public TextMeshProUGUI potionCountText;


    [Header("TurnManager")]
    public TurnManager turnManager;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player_startPosition = transform.position;
        potionCountText.text = numberOfPotions.ToString();

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Attack()
    {
        if (turnManager.gameEnded) return;

        attacking = true;
        transform.position = Vector3.MoveTowards(transform.position, enemyPosition.position, maxDistance);
        anim.SetBool("Attack", attacking);
        StartCoroutine("WaitAttack");
        
    }

    IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(waitTimeAttack);
        attacking = false;
        anim.SetBool("Attack", attacking);
        transform.position = player_startPosition;
        turnManager.EndTurn();
    }


    public void Defend()
    {
        if (turnManager.gameEnded) return;

        defending = true;
        anim.SetBool("Defend", defending);
        StartCoroutine("Defending");
    }

    IEnumerator Defending()
    {
        yield return new WaitForSeconds(waitTimeToDefend);
        defending = false;
        anim.SetBool("Defend", defending);
        combatPanel.SetActive(true);
        turnManager.EndTurn();
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
            StartCoroutine("Healing");
        }
     
    }

    IEnumerator Healing()
    {
        int previousHealth = current_health;
        current_health = Mathf.Min(current_health + potionHealAmount, maxHealth);

        healing = true;
        anim.SetBool("Healing", healing);

        // Inicia a animação da barra de vida junto com a animação de cura
        StartCoroutine(AnimateHealthBar(previousHealth, current_health, waitTimeToHeal));

        yield return new WaitForSeconds(waitTimeToHeal);

        healing = false;
        anim.SetBool("Healing", healing);


        if (numberOfPotions == 0)
        {
            healButton.image.color = Color.black;
            healButton.interactable = false;
        }

        turnManager.EndTurn();
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

        hurting = true;
        anim.SetBool("Hurt", hurting);
        int previousHealth = current_health;
        current_health = Mathf.Max(current_health - amount, 0);
        StartCoroutine(AnimateHealthBar(previousHealth, current_health, healthUpdateDuration));

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
        anim.SetBool("Hurt", hurting);
        //combatPanel.SetActive(true);
        
    }
    IEnumerator Die()
    {
        anim.SetTrigger("Die");
        turnManager.gameEnded = true;
        combatPanel.SetActive(false);
        healtBar.SetActive(false);
        turnManager.ShowWinner("Enemy");
        yield return null;
    }



}
