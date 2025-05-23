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
        // Impede início de turno após o jogo acabar
        if (gameEnded) return;

        currentTurn = turn;

        // Atualiza UI
        SetTurnText($"{turn.ToString()}'s Turn");

        // Ativa/Desativa painel de ações
        player.combatPanel.SetActive(turn == Turn.Player);

        // Enemy age automaticamente no turno dele
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
        if (gameEnded) return; // Impede troca de turno após fim do jogo

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
        if (gameEnded) return; // Evita ação inimiga após fim do jogo
        enemy.PerformAction();
        Invoke(nameof(EndTurn), 2f);
    }

    public void SetTurnText(string text)
    {
        if (gameEnded) return; // Impede sobreescrever após vitória
        turnText.text = text;
    }

    public void ShowWinner(string winner)
    {
        gameEnded = true; // IMPORTANTE: sinaliza fim do jogo
        turnText.text = $"{winner} Wins!";
        player.combatPanel.SetActive(false); // Oculta ações
    }
}
