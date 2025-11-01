using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// Opcional: Si usas NavMeshAgent para moverlo
// using UnityEngine.AI; 

public class Enemigo : MonoBehaviour
{
    [Header("Estadísticas")]
    public float salud = 100f;
    private float saludMaxima; // --- NUEVO: Para calcular el %
    public float velocidad = 5f;

    private float velocidadOriginal;
    private Coroutine corrutinaRalentizacion;

    // Opcional: Referencia al agente de movimiento
    // private NavMeshAgent agent; 

    [Header("Feedback de Daño")]
    public Material materialFlash; // Un material rojo brillante
    public Image barraDeVida; // --- NUEVO: Arrastra tu imagen "RellenoBarra" aquí ---

    private Material materialOriginal;
    private Renderer rend;
    private Coroutine flashCoroutine; // Para controlar el flash
    private Camera camaraPrincipal; // --- NUEVO: Para el billboard ---

    void Start()
    {
        // --- NUEVO: Guarda la salud máxima al inicio ---
        saludMaxima = salud;

        // --- NUEVO: Busca la cámara ---
        camaraPrincipal = Camera.main;

        velocidadOriginal = velocidad;
        // agent = GetComponent<NavMeshAgent>();
        // if(agent != null) agent.speed = velocidad;

        // Obtenemos el Renderer (puede ser de este objeto o de un hijo)
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            materialOriginal = rend.material; // Guardamos el material normal
        }
    }

    // --- NUEVO: LateUpdate se ejecuta después de Update ---
    // Lo usamos para que la UI siempre mire a la cámara
    void LateUpdate()
    {
        if (barraDeVida != null)
        {
            // Apunta el transform "padre" (el Canvas) hacia la cámara
            barraDeVida.transform.parent.LookAt(transform.position + camaraPrincipal.transform.forward);
        }
    }

    // Función pública llamada por el proyectil
    public void RecibirDaño(float cantidad)
    {
        // Resta la salud ANTES de actualizar la UI
        salud -= cantidad;
        salud = Mathf.Clamp(salud, 0, saludMaxima); // Evita que la salud sea negativa

        // --- ESTA ES LA LÓGICA QUE FALTABA ---
        if (barraDeVida != null)
        {
            // fillAmount es un valor de 0.0 a 1.0
            // Calculamos el porcentaje de salud restante
            barraDeVida.fillAmount = salud / saludMaxima;
        }
        // --- FIN DE LA LÓGICA ---
        Debug.Log($"Enemigo golpeado. Salud restante: {salud}");

        // Iniciar el flash. Si ya estaba en flash, lo reinicia.
        if (materialFlash != null && rend != null)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashDeDaño());
        }

        if (salud <= 0)
        {
            Morir();
        }
    }

    private IEnumerator FlashDeDaño()
    {
        rend.material = materialFlash; // Cambiar al material rojo
        yield return new WaitForSeconds(0.1f); // Duración del flash
        rend.material = materialOriginal; // Volver al material original
        flashCoroutine = null; // Limpiar la corutina
    }

    // Función pública llamada por el proyectil
    public void AplicarRalentizacion(float multiplicador, float duracion)
    {
        // Si ya estábamos ralentizados, detenemos la corrutina anterior
        // para reemplazarla con la nueva.
        if (corrutinaRalentizacion != null)
        {
            StopCoroutine(corrutinaRalentizacion);
        }

        corrutinaRalentizacion = StartCoroutine(EfectoRalentizar(multiplicador, duracion));
    }

    private IEnumerator EfectoRalentizar(float multiplicador, float duracion)
    {
        Debug.Log("Enemigo RALENTIZADO");
        velocidad = velocidadOriginal * multiplicador;
        // if(agent != null) agent.speed = velocidad;

        // Esperar el tiempo de duración
        yield return new WaitForSeconds(duracion);

        // Devolver a la normalidad
        Debug.Log("Ralentización TERMINADA");
        velocidad = velocidadOriginal;
        // if(agent != null) agent.speed = velocidad;

        corrutinaRalentizacion = null; // Limpiar la referencia
    }

    private void Morir()
    {
        Debug.Log("Enemigo ha muerto.");
        // Aquí podrías instanciar efectos de muerte, loot, etc.
        Destroy(gameObject);
    }
}