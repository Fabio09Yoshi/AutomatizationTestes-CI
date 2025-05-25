using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public enum Turn
{
    Player,
    Enemy
}

public class TurnManager : MonoBehaviour
{
    public Turn currentTurn;
    public TextMeshProUGUI turnText;
    public bool gameEnded = false;

    public PlayerController player;
    public EnemyController enemy;

    void Start()
    {
        StartTurn(currentTurn);
    }

    public void StartTurn(Turn turn)
    {

        if (gameEnded) return;

        currentTurn = turn;

        SetTurnText($"{turn.ToString()}'s Turn");

        if (turn == Turn.Player)
            player.combatPanel.SetActive(turn == Turn.Player);



        if (turn == Turn.Enemy)
        {
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(0.5f); 
        enemy.PerformAction();
    }

    public void EndTurn()
    {
        if (gameEnded) return;

        if (currentTurn == Turn.Player)
        {
            StartTurn(Turn.Enemy);
        }
        else
        {
            StartTurn(Turn.Player);
        }
    }

    void EnemyAction()
    {
        if (gameEnded) return;
        enemy.PerformAction();
        Invoke(nameof(EndTurn), 2f);
    }

    public void SetTurnText(string text)
    {
        if (gameEnded) return;

        //Tratando um caso de Teste - Evitando erro de que o teste execute antes dos objetos e referências
        if (turnText != null)
            turnText.text = text;
    }

    public void ShowWinner(string winner)
    {
        gameEnded = true; 
        turnText.text = $"{winner} Wins!";
        player.combatPanel.SetActive(false);
    }

    //public void SetupTurnManagerForTesting()
    //{        
    //    // Referenciamos todos os objetos que precisam de referencia
    //    // Isso evita Erros de NullReference em testes
    //    turnText = new GameObject("TurnText").AddComponent<TextMeshProUGUI>();

    //    player = new GameObject("Player").AddComponent<PlayerController>();
    //    player.combatPanel = new GameObject("CombatPanel");

    //    enemy = new GameObject("Enemy").AddComponent<EnemyController>();
    //}


}
