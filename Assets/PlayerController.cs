using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

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
    public bool hurt;
    public float maxDistance = 5f;
    public float waitTimeAttack;
    public float waitTimeToDefend;
    public float waitTimeToHeal;

    Animator anim;
    [Header("Enemy")]
    public Transform enemyPosition;

    [Header("UI")]
    public GameObject combatPanel;
    public GameObject healtBar;
    public Button healButton;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player_startPosition = transform.position;


    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Attack()
    {

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
        combatPanel.SetActive(true);
        StopCoroutine("WaitAttack");
    }


    public void Defend()
    { 
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
        StopCoroutine("waitTimeToDefend");
    }

    public void Heal()
    {
        if (numberOfPotions > 0)
        {
            healing = true;
            numberOfPotions--;
            anim.SetBool("Healing", healing);
            StartCoroutine("Healing");
        }
     
    }

    IEnumerator Healing()
    {       
        yield return new WaitForSeconds(waitTimeToHeal);
        healing = false;
        anim.SetBool("Healing", healing);
        combatPanel.SetActive(true);
        if (numberOfPotions == 0)
        {
            healing = false;
            healButton.image.color = Color.black;
            healButton.interactable = false;
        }
        StopCoroutine("Healing");
    }
            

}
