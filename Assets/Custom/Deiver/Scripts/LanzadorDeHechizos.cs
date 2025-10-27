using UnityEngine;

public class LanzadorDeHechizos : MonoBehaviour
{
    [Header("Estadísticas del Jugador")]
    public float saludActual = 100f;
    public float escudoActual = 0f;

    [Header("Configuración de Hechizos")]
    public GameObject prefabBolaDeFuego;
    public GameObject prefabLanzaDeHielo;
    public Transform puntoDeLanzamiento; // Un 'Empty' hijo de la cámara

    [Header("Buff: Escudo Arcano")]
    public float cantidadEscudo = 50f;
    public GameObject vfxEscudo; // La esfera semitransparente

    // ¡NUEVO! Necesitamos una referencia a la cámara
    public Camera camaraDelJugador;

    // --- NUEVO: SECCIÓN DE SONIDO ---
    [Header("Audio Feedback (Lanzamiento)")]
    public AudioClip sonidoLanzarFuego;
    public AudioClip sonidoLanzarHielo;
    public AudioClip sonidoLanzarEscudo;
    private AudioSource audioSource;
    // --- FIN NUEVO ---

    void Start()
    {
        if (vfxEscudo != null)
        {
            vfxEscudo.SetActive(false); // Asegurarse de que el escudo esté apagado al iniciar
        }

        if (camaraDelJugador == null)
        {
            camaraDelJugador = Camera.main; // Intenta encontrarla automáticamente
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // --- BUCLUS DE JUEGO PRINCIPAL: DETECTAR INPUT ---

        // 1. Lanzar Bola de Fuego
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LanzarProyectil(prefabBolaDeFuego);
        }

        // 2. Lanzar Lanza de Hielo
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LanzarProyectil(prefabLanzaDeHielo);
        }

        // 3. Lanzar Escudo Arcano
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LanzarEscudoArcano();
        }
    }

    // --- LÓGICA DE LANZAMIENTO ---

    void LanzarProyectil(GameObject prefabHechizo)
    {
        if (prefabHechizo == null || puntoDeLanzamiento == null)
        {
            Debug.LogError("¡Falta el Prefab del hechizo o el Punto de Lanzamiento!");
            return;
        }

        Debug.Log($"Lanzando {prefabHechizo.name}");

        // 1. Calcular el punto de mira (target)
        Vector3 puntoDeMira;
        RaycastHit hit;

        // Lanzamos un rayo desde el centro de la cámara
        Ray rayo = camaraDelJugador.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(rayo, out hit))
        {
            // Si golpeamos algo, ese es nuestro objetivo
            puntoDeMira = hit.point;
        }
        else
        {
            // Si no golpeamos nada, el objetivo es un punto lejano
            puntoDeMira = rayo.GetPoint(100); // 100 metros
        }

        // 2. Calcular la dirección desde el punto de lanzamiento hacia el punto de mira
        Vector3 direccion = (puntoDeMira - puntoDeLanzamiento.position).normalized;

        // 3. Instanciar el proyectil
        // Usamos Quaternion.LookRotation para que el prefab "mire" instantáneamente hacia el objetivo
        GameObject proyectilInstanciado = Instantiate(prefabHechizo, puntoDeLanzamiento.position, Quaternion.LookRotation(direccion));

        // 4. Inicializar el proyectil
        // (Esto solo funciona si el prefab tiene el script ProyectilDeHechizo)
        ProyectilDeHechizo scriptProyectil = proyectilInstanciado.GetComponent<ProyectilDeHechizo>();
        if (scriptProyectil != null)
        {
            scriptProyectil.InicializarLanzamiento(direccion);
        }
        else
        {
            // Si el script no está, usamos la física por defecto (menos preciso)
            Rigidbody rb = proyectilInstanciado.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = direccion * 20f; // Usar una velocidad genérica
        }

        if (prefabHechizo == prefabBolaDeFuego && sonidoLanzarFuego != null)
        {
            audioSource.PlayOneShot(sonidoLanzarFuego);
        }
        else if (prefabHechizo == prefabLanzaDeHielo && sonidoLanzarHielo != null)
        {
            audioSource.PlayOneShot(sonidoLanzarHielo);
        }
    }

    void LanzarEscudoArcano()
    {
        Debug.Log("¡Escudo Arcano lanzado!");
        escudoActual = cantidadEscudo;

        if (vfxEscudo != null)
        {
            vfxEscudo.SetActive(true);
        }

        // Opcional: Podrías usar una corrutina aquí para que el escudo dure X segundos
        // StartCoroutine(DuracionEscudo(10f));

        if (sonidoLanzarEscudo != null)
        {
            audioSource.PlayOneShot(sonidoLanzarEscudo);
        }
    }

    // --- LÓGICA DE RECEPCIÓN DE DAÑO (para el Jugador) ---

    public void RecibirDaño(float cantidad)
    {
        Debug.Log($"Jugador recibe {cantidad} de daño.");

        // 1. El daño golpea el escudo primero
        if (escudoActual > 0)
        {
            float dañoAbsorbido = Mathf.Min(escudoActual, cantidad);
            escudoActual -= dañoAbsorbido;
            cantidad -= dañoAbsorbido; // Reducimos el daño restante

            Debug.Log($"Escudo absorbió {dañoAbsorbido}. Escudo restante: {escudoActual}");

            if (escudoActual <= 0)
            {
                if (vfxEscudo != null) vfxEscudo.SetActive(false);
                Debug.Log("¡Escudo roto!");
            }
        }

        // 2. Si queda daño, golpea la salud
        if (cantidad > 0)
        {
            saludActual -= cantidad;
            Debug.Log($"Salud golpeada. Salud restante: {saludActual}");
        }

        if (saludActual <= 0)
        {
            Debug.Log("¡El jugador ha muerto!");
            // Lógica de muerte del jugador
        }
    }
}