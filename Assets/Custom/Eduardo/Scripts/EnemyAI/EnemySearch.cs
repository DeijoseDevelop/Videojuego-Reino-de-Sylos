using UnityEngine;
using UnityEngine.AI;

public class EnemySearch : MonoBehaviour
{
    [Header("Configuración de Detección")]
    public float rangoVision = 10f; // Rango de detección del jugador
    public string tagJugador = "Player"; // Tag del jugador
    
    [Header("Configuración de Ataque")]
    public float rangoAtaque = 2f; // Rango mínimo para atacar
    
    private NavMeshAgent navMeshAgent;
    private Transform jugador;
    private bool jugadorDetectado = false;
    private EnemyAttack enemyAttack;
    
    void Start()
    {
        // Obtener el componente NavMeshAgent
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        // Obtener el componente EnemyAttack
        enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack == null)
        {
            Debug.LogWarning("EnemyAttack no encontrado en el objeto");
        }
        
        // Buscar el jugador por tag
        GameObject jugadorObj = GameObject.FindGameObjectWithTag(tagJugador);
        if (jugadorObj != null)
        {
            jugador = jugadorObj.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el tag: " + tagJugador);
        }
    }

    void Update()
    {
        if (jugador == null) return;
        
        // Calcular la distancia al jugador
        float distancia = Vector3.Distance(transform.position, jugador.position);
        
        // Verificar si el jugador está en rango de ataque
        if (distancia <= rangoAtaque && enemyAttack != null)
        {
            // Detener el movimiento cuando está en rango de ataque
            navMeshAgent.SetDestination(transform.position);
            
            // Intentar atacar
            if (enemyAttack.CanAttack())
            {
                enemyAttack.AttackPlayer();
                Debug.Log("¡Atacando al jugador!");
            }
            
            return;
        }
        
        // Verificar si el jugador está en rango de visión
        if (distancia <= rangoVision)
        {
            if (!jugadorDetectado)
            {
                Debug.Log("Te vi");
                jugadorDetectado = true;
            }
            
            // Mover hacia el jugador
            navMeshAgent.SetDestination(jugador.position);
        }
        else
        {
            if (jugadorDetectado)
            {
                Debug.Log("No veo a nadie");
                jugadorDetectado = false;
            }
        }
    }
    
    // Dibujar el rango de visión y ataque en el editor
    void OnDrawGizmosSelected()
    {
        // Rango de visión en amarillo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVision);
        
        // Rango de ataque en rojo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}
