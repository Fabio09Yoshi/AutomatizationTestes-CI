using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public PlayerController player;
    public int enemyDamage;

    public void PerformAction()
    {
        // Simula um ataque ao player
        Debug.Log("Enemy attacks!");
        player.ReceiveDamage(enemyDamage);
        // Aqui pode adicionar animação e barra de vida também
    }
}
