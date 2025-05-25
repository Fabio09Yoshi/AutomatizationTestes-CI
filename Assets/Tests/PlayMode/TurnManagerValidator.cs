using System.Collections;
using NUnit.Framework;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.TestTools;

public class TurnManagerValidator
{
    GameObject playerGO, enemyGO, canvasGO;
    PlayerController player;
    EnemyController enemy;
    TurnManager turnManager;

    //Outra forma de fazer o Setup de testes
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        canvasGO = new GameObject("Canvas", typeof(Canvas));

        playerGO = new GameObject("Player", typeof(PlayerController), typeof(Animator));
        enemyGO = new GameObject("Enemy", typeof(EnemyController), typeof(Animator));

        player = playerGO.GetComponent<PlayerController>();
        enemy = enemyGO.GetComponent<EnemyController>();

        var combatPanel = new GameObject("CombatPanel");
        combatPanel.transform.SetParent(playerGO.transform);
        combatPanel.AddComponent<CanvasRenderer>();
        combatPanel.SetActive(false);
        player.combatPanel = combatPanel;

        var enemyRecoverButton = new GameObject("EnemyRecoverButton", typeof(Button));
        enemy.enemy_recoverButton = enemyRecoverButton.GetComponent<Button>();
        enemy.enemy_recoverButton.interactable = false;

        var turnTextGO = new GameObject("TurnText", typeof(TextMeshProUGUI));
        turnTextGO.transform.SetParent(canvasGO.transform);
        var turnText = turnTextGO.GetComponent<TextMeshProUGUI>();


        var managerGO = new GameObject("TurnManager", typeof(TurnManager));
        turnManager = managerGO.GetComponent<TurnManager>();
        turnManager.player = player;
        turnManager.enemy = enemy;
        turnManager.turnText = turnText;
        turnManager.gameEnded = false;

        yield return null;
    }

}
