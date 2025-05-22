using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    Vector3 player_startPosition;
    public int current_health = 100;
    public int maxHealth = 100;
    public int damage = 10;
    public bool attacking;
    public float maxDistance = 5f;
    public float waitTimeAttack;

    Animator anim;
    [Header("Enemy")]
    public Transform enemyPosition;

    [Header("UI")]
    public GameObject combatPanel;
    public GameObject healtBar;


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

}
