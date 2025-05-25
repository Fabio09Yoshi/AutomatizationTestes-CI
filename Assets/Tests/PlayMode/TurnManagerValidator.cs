using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TurnManagerValidator
{
    [UnityTest]
    //Validando se o turno do Player está iniciando com tudo que é necessário ativado
    //Esperado: UI de turno deverá estar chamando o turno do player (Player's Turn)
    //Esperado: Painel de combate estar ativo com todos os seus 3 respectivos botões ativos também (attack, defend, heal)
    public IEnumerator PlayerTurn_ActivatesCombatPanel()
    {
        //Setup Inicial
        //Criando a variavel para armazenar o Script TurnManager
        var go = new GameObject("TurnManager");
        //Referenciando o Script na nova variavel turnManager
        var turnManager = go.AddComponent<TurnManager>();
        //Iniciando todo o Setup Inicial de test do TurnManager
        turnManager.SetupTurnManagerForTesting();

        yield return null;
        //
        Assert.AreEqual("Player's Turn", turnManager.turnText.text);
        Debug.Log("Turn Status: " + turnManager.turnText.text);

        Assert.IsTrue(turnManager.player.combatPanel.gameObject.activeSelf);
        Debug.Log("Combat Panel Status: " + turnManager.player.combatPanel.gameObject.activeSelf);
    }
}
