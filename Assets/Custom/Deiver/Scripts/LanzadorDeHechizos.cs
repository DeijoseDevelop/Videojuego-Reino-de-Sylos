using UnityEngine;

[RequireComponent(typeof(AudioSource))] // Buena práctica
public class LanzadorDeHechizos : MonoBehaviour
{
    [Header("Estadísticas del Jugador")]
    public float saludActual = 100f;
    public float escudoActual = 0f;

    // --- REFACTORIZADO ---
    // ¡Ya no necesitamos referencias a prefabs o sonidos aquí!
    // Solo tenemos una "Barra de Acción" de hechizos.
    [Header("Barra de Hechizos")]
    public Hechizo[] hechizosEnBarra = new Hechizo[3];

    [Header("Componentes del Lanzador")]
    // Estos componentes deben ser públicos para que los
    // Hechizos (ScriptableObjects) puedan usarlos.
    public Transform puntoDeLanzamiento;
    public Camera camaraDelJugador;
    public AudioSource audioSource;

    [Header("Efectos Internos")]
    public GameObject vfxEscudo; // Se queda aquí, es un efecto del *jugador*
    // --- FIN REFACTORIZADO ---

    void Start()
    {
        if (vfxEscudo != null)
        {
            vfxEscudo.SetActive(false);
        }

        if (camaraDelJugador == null)
        {
            camaraDelJugador = Camera.main;
        }

        // Asignamos la referencia pública
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // --- REFACTORIZADO ---
        // El Update es ahora limpio, genérico y escalable.

        // Lanzar Hechizo 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Si hay un hechizo en la ranura 0, intenta lanzarlo.
            hechizosEnBarra[0]?.Lanzar(this);
            // El '?' (operador null-conditional) evita errores
            // si la ranura está vacía.
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
        // --- FIN REFACTORIZADO ---
    }

    // --- LÓGICA DE BUFF (Ahora es una API pública) ---
    // Esta función es llamada por el 'HechizoBuff'
    public void AplicarEscudo(float cantidad)
    {
        escudoActual = cantidad;
        if (vfxEscudo != null)
        {
            vfxEscudo.SetActive(true);
        }
        // Aquí podrías iniciar una corrutina para que el escudo dure X segundos
    }


    // --- LÓGICA DE DAÑO (Sin cambios) ---
    public void RecibirDaño(float cantidad)
    {
        Debug.Log($"Jugador recibe {cantidad} de daño.");

        if (escudoActual > 0)
        {
            float dañoAbsorbido = Mathf.Min(escudoActual, cantidad);
            escudoActual -= dañoAbsorbido;
            cantidad -= dañoAbsorbido;

            Debug.Log($"Escudo absorbió {dañoAbsorbido}. Escudo restante: {escudoActual}");

            if (escudoActual <= 0)
            {
                if (vfxEscudo != null) vfxEscudo.SetActive(false);
                Debug.Log("¡Escudo roto!");
            }
        }

        if (cantidad > 0)
        {
            saludActual -= cantidad;
            Debug.Log($"Salud golpeada. Salud restante: {saludActual}");
        }

        if (saludActual <= 0)
        {
            Debug.Log("¡El jugador ha muerto!");
        }
    }

    // --- ¡LAS VIEJAS FUNCIONES 'LanzarProyectil' y 'LanzarEscudoArcano' SE ELIMINARON! ---
    // Su lógica ahora vive dentro de los ScriptableObjects 'HechizoProyectil' y 'HechizoBuff'.
}