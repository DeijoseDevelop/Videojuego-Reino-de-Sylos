using UnityEngine;
using UnityEngine.AI;

public class EnemySearch : MonoBehaviour
{
    [Header("Configuración de Detección")]
    public float rangoVision = 10f; // Rango de detección del jugador
    public string tagJugador = "Player"; // Tag del jugador
    
    private NavMeshAgent navMeshAgent;
    private Transform jugador;
    private bool jugadorDetectado = false;
    
    void Start()
    {
        // Obtener el componente NavMeshAgent
        navMeshAgent = GetComponent<NavMeshAgent>();
        
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
        
        // Verificar si el jugador está en rango
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
    
    // Dibujar el rango de visión en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVision);
    }
}
