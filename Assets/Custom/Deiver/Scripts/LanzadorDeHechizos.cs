using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerStats))] // ¡Asegura que el script de Stats exista!
public class LanzadorDeHechizos : MonoBehaviour
{
    // --- NUEVA REFERENCIA DE STATS ---
    [Header("Componentes Centrales")]
    // Esta es la conexión clave. La llenaremos en Start().
    public PlayerStats playerStats;

    [Header("Barra de Hechizos")]
    public Hechizo[] hechizosEnBarra = new Hechizo[3];

    [Header("Componentes del Lanzador")]
    // Estos componentes deben ser públicos para que los
    // Hechizos (ScriptableObjects) puedan usarlos.
    public Transform puntoDeLanzamiento;
    public Camera camaraDelJugador;
    public AudioSource audioSource;

    // --- TODA LA LÓGICA DE SALUD Y ESCUDO SE HA MOVIDO A 'PlayerStats' ---

    void Start()
    {
        // Obtenemos la referencia al script de Stats que está en este mismo objeto
        playerStats = GetComponent<PlayerStats>();

        if (camaraDelJugador == null)
        {
            camaraDelJugador = Camera.main;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // El Update sigue siendo limpio y perfecto.

        // Lanzar Hechizo 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hechizosEnBarra[0]?.Lanzar(this);
        }

        // Lanzar Hechizo 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hechizosEnBarra[1]?.Lanzar(this);
        }

        // Lanzar Hechizo 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            hechizosEnBarra[2]?.Lanzar(this);
        }
    }

    // --- ¡LAS FUNCIONES 'RecibirDaño' Y 'AplicarEscudo' SE ELIMINARON! ---
    // Ahora viven en 'PlayerStats.cs' y son llamadas desde allí.
}