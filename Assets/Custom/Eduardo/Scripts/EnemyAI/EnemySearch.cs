using UnityEngine;
using UnityEngine.AI;

public class EnemySearch : MonoBehaviour
{
    [Header("Configuración de Detección")]
    public float rangoVision = 10f; // Rango de detección del jugador
	[TagSelector] public string tagJugador = "Player"; // Tag del jugador [tagselector] hace que se vea como un dropdownMenu
    
    [Header("Configuración de Ataque")]
    public float rangoAtaque = 2f; // Rango mínimo para atacar

	[Header("Configuración de Merodeo (Búsqueda del jugador)")]
	public bool merodeoActivo = true; // Si es true, el enemigo merodea cuando no ve al jugador
	public float radioMerodeo = 5f; // Radio del área pequeña de búsqueda
	public float maxPasoMerodeo = 2f; // Distancia máxima por salto de merodeo
	public float pausaEntrePuntos = 1.0f; // Pausa al alcanzar un punto
	public float velocidadMerodeo = 2.5f; // Velocidad del agente al merodear
	public float intervaloCambioDestino = 2.5f; // Cada cuánto elige un nuevo punto si no llega
	public float distanciaParaNuevoDestino = 0.5f; // Distancia para considerar alcanzado el punto
	public Transform centroMerodeo; // Si es null, usa la posición del enemigo como centro
    
    private NavMeshAgent navMeshAgent;
    private Transform jugador;
    private bool jugadorDetectado = false;
    private EnemyAttack enemyAttack;
	private float velocidadBaseAgente = 3.5f;

	// Estado interno de merodeo
	private Vector3 destinoMerodeoActual;
	private bool tieneDestinoMerodeo = false;
	private float tiempoProximoCambio = 0f;
    
    void Start()
    {
        // Obtener el componente NavMeshAgent
        navMeshAgent = GetComponent<NavMeshAgent>();
		if (navMeshAgent != null)
		{
			velocidadBaseAgente = navMeshAgent.speed;
		}
        
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
			// Asegurar velocidad base al perseguir
			navMeshAgent.speed = velocidadBaseAgente;
			// Al seguir al jugador, invalidar el destino de merodeo
			tieneDestinoMerodeo = false;
        }
        else
        {
            if (jugadorDetectado)
            {
                Debug.Log("No veo a nadie");
                jugadorDetectado = false;
            }

			// Si no ve al jugador, activar merodeo aleatorio si está habilitado
			if (merodeoActivo)
			{
				ActualizarMerodeo();
			}
        }
    }

	private Vector3 ObtenerCentroMerodeo()
	{
		return centroMerodeo != null ? centroMerodeo.position : transform.position;
	}

	private bool ElegirNuevoDestinoMerodeo(out Vector3 destino)
	{
		Vector3 centro = ObtenerCentroMerodeo();
		// Generar paso corto desde la posición actual
		float distanciaPaso = Mathf.Clamp(Random.Range(maxPasoMerodeo * 0.5f, maxPasoMerodeo), 0.1f, maxPasoMerodeo);
		Vector2 dir2D = Random.insideUnitCircle.normalized;
		Vector3 candidato = transform.position + new Vector3(dir2D.x, 0f, dir2D.y) * distanciaPaso;
		// Mantener dentro del radio de merodeo
		Vector3 desdeCentro = candidato - centro;
		float mag = desdeCentro.magnitude;
		if (mag > radioMerodeo)
		{
			candidato = centro + desdeCentro.normalized * (radioMerodeo - 0.05f);
		}
		NavMeshHit hit;
		if (NavMesh.SamplePosition(candidato, out hit, maxPasoMerodeo, NavMesh.AllAreas))
		{
			destino = hit.position;
			return true;
		}
		destino = centro;
		return false;
	}

	private void ActualizarMerodeo()
	{
		// Si no hay destino, o es tiempo de cambiar, elegir uno nuevo
		bool debeCambiar = !tieneDestinoMerodeo || Time.time >= tiempoProximoCambio;
		if (debeCambiar)
		{
			Vector3 nuevoDestino;
			if (ElegirNuevoDestinoMerodeo(out nuevoDestino))
			{
				destinoMerodeoActual = nuevoDestino;
				tieneDestinoMerodeo = true;
				tiempoProximoCambio = Time.time + intervaloCambioDestino;
				// Velocidad de merodeo
				navMeshAgent.speed = velocidadMerodeo;
				navMeshAgent.SetDestination(destinoMerodeoActual);
			}
		}

		// Si ya tenemos un destino, comprobar si llegamos y, de ser así, forzar cambio pronto
		if (tieneDestinoMerodeo)
		{
			float distanciaRestante = Vector3.Distance(transform.position, destinoMerodeoActual);
			if (distanciaRestante <= distanciaParaNuevoDestino || (navMeshAgent.remainingDistance <= distanciaParaNuevoDestino && !navMeshAgent.pathPending))
			{
				// Pausar antes del siguiente destino
				tiempoProximoCambio = Time.time + pausaEntrePuntos;
				tieneDestinoMerodeo = false;
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
		
		// Área de merodeo en cian
		if (merodeoActivo)
		{
			Gizmos.color = Color.cyan;
			Vector3 centro = centroMerodeo != null ? centroMerodeo.position : transform.position;
			Gizmos.DrawWireSphere(centro, radioMerodeo);
		}
    }
}
