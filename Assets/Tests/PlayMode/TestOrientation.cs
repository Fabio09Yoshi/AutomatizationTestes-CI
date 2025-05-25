using System.Collections;
using NUnit.Framework;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.TestTools;

public class TestOrientation
{
    private GameObject enemyGO;
    private EnemyController enemy;
    private GameObject playerGO;
    private PlayerController player;
    private TurnManager turnManager;

    //Outra forma de fazer o Setup de testes ao invés de setar em cada código. Essa forma é mais profissional pois não criamos a dependencia de que todo código esteja setado corretamente na hora de instanciar
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Criar Enemy GameObject
        enemyGO = new GameObject("Enemy");
        enemy = enemyGO.AddComponent<EnemyController>();

        // Criar e adicionar o Animator
        enemy.anim = enemyGO.AddComponent<Animator>();

        // HealthBar (Slider)
        enemy.enemy_healthBar = new GameObject("HealthBar").AddComponent<Slider>();
        enemy.enemy_healthBar.gameObject.AddComponent<Canvas>(); // Canvas necessário para renderização
        enemy.enemy_healthBar.maxValue = 100;
        enemy.enemy_healthBar.value = 100;

        // Recover Button
        enemy.enemy_recoverButton = new GameObject("RecoverButton").AddComponent<Button>();

        // Criar Player e conectar no EnemyController
        playerGO = new GameObject("Player");
        player = playerGO.AddComponent<PlayerController>();
        enemy.player = player;

        // Referência da posição do jogador
        enemy.playerPosition = playerGO.transform;

        // Criar TurnManager e conectar nos dois
        GameObject turnGO = new GameObject("TurnManager");
        turnManager = turnGO.AddComponent<TurnManager>();

        turnManager.enemy = enemy;
        turnManager.player = player;

        // Criar TMP Text para evitar erro de NullReference
        turnManager.turnText = new GameObject("TurnText").AddComponent<TextMeshProUGUI>();

        enemy.turnManager = turnManager;
        player.turnManager = turnManager;

        // Configurar valores básicos para evitar null em testes
        enemy.enemy_current_health = 100;
        enemy.enemy_max_health = 100;
        enemy.enemyDamage = 25;
        enemy.enemy_healthUpdateDuration = 1f;
        enemy.enemy_waitTimeAttacking = 0.4f;
        enemy.enemy_waitTimeToHurt = 0.5f;
        enemy.enemy_waitTimeToRecover = 1.2f;

        // Setup da UI do jogador
        player.enemyController = enemy;
        player.healthBar = new GameObject("HealthSlider").AddComponent<Slider>();
        player.healthBar.maxValue = 100;
        player.healthBar.value = 100;
        player.anim = playerGO.AddComponent<Animator>();
        player.combatPanel = new GameObject("CombatPanel");
        player.restartGame = new GameObject("Restart").AddComponent<Button>();
        player.potionCountText = new GameObject("PotionText").AddComponent<TextMeshProUGUI>();
        player.healButton = new GameObject("HealButton").AddComponent<Button>();

        // Posicionar o Player e o Inimigo
        enemyGO.transform.position = Vector3.zero;
        playerGO.transform.position = Vector3.right * 2;

        // Setup valores padrão do player
        player.current_health = 100;
        player.maxHealth = 100;
        player.defenseBlock = 0.5f;
        player.waitTimeAttack = 0.7f;
        player.waitTimeToHeal = 0.7f;
        player.waitTimeToHurt = 0.6f;
        player.damage = 25;
        player.playerIsDead = false;

        yield return null;
    }

    [UnityTest] //Usar quando o teste precisar validar tempo, frame updates, corrotinas, comportamento assíncrono da Unity.
    public IEnumerator PlayerTurn_ActivatesCombatPanel()
    {
        turnManager.currentTurn = Turn.Player; 
        turnManager.StartTurn(Turn.Player);
        yield return null;
        Assert.IsTrue(player.combatPanel.activeSelf);
    }

    [UnityTest]
    public IEnumerator EnemyTurn_AttacksPlayer()
    {
        player.current_health = 10;
        enemy.enemyDamage = 4;
        turnManager.currentTurn = Turn.Enemy;
        turnManager.StartTurn(Turn.Enemy);
        yield return new WaitForSeconds(1.1f);
        Assert.AreEqual(6, player.current_health);
    }

    [Test] //Usado para testes de lógica, sem precisar aguardar frames ou corrotinas. Bom para testar cálculos, mudanças de estado, etc.
    public void EndTurn_AlternatesTurn()
    {
        turnManager.currentTurn = Turn.Player;
        turnManager.EndTurn();
        Assert.AreEqual(Turn.Enemy, turnManager.currentTurn);
    }

    [Test]
    public void ShowWinner_EndsGame()
    {
        turnManager.ShowWinner("Player");
        Assert.AreEqual("Player Wins!", turnManager.turnText.text);
        Assert.IsTrue(turnManager.gameEnded);
        Assert.IsFalse(player.combatPanel.activeSelf);
    }

    [Test]
    public void Heal_UsesPotionAndRestoresHealth()
    {
        player.current_health = 5;
        player.numberOfPotions = 1;
        player.Heal();
        Assert.AreEqual(25, player.current_health);
        Assert.AreEqual(0, player.numberOfPotions);
    }

    [Test]
    public void ReceiveDamage_WhenBlocking_ReducesDamage()
    {
        player.current_health = 10;
        player.isBlocking = true;
        player.ReceiveDamage(4);
        Assert.AreEqual(8, player.current_health);
    }

    [Test]
    public void ReceiveDamage_KillsPlayerAndEndsGame()
    {
        player.current_health = 2;
        player.ReceiveDamage(5);
        Assert.AreEqual(0, player.current_health);
        Assert.IsTrue(player.playerIsDead);
    }

    [Test]
    public void EnemyReceivesFatalDamage_TriggersDeath()
    {
        enemy.enemy_current_health = 3;
        enemy.EnemyReceiveDamage(3);
        Assert.AreEqual(0, enemy.enemy_current_health);
        Assert.IsTrue(enemy.isDead);
        Assert.IsTrue(enemy.enemy_recoverButton.interactable);
    }

    [UnityTest]
    public IEnumerator EnemyAttack_MovesBackToOriginalPosition()
    {
        Vector3 start = enemyGO.transform.position;
        enemy.enemyDamage = 2;
        player.current_health = 5;
        yield return enemy.StartCoroutine("EnemyAttackRoutine");
        Assert.AreEqual(3, player.current_health);
        Assert.Less(Vector3.Distance(start, enemyGO.transform.position), 0.01f);
    }

    [UnityTest]
    public IEnumerator RecoverEnemy_ResetsGame()
    {
        enemy.enemy_current_health = 0;
        turnManager.gameEnded = true;
        turnManager.currentTurn = Turn.Enemy;
        enemy.EnemyRecoverStart();
        yield return new WaitForSeconds(1.2f); 
        Assert.AreEqual(enemy.enemy_max_health, enemy.enemy_current_health);
        Assert.IsFalse(turnManager.gameEnded);
        Assert.AreEqual(Turn.Player, turnManager.currentTurn);
    }
}

