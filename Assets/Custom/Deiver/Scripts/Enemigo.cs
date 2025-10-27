using System.Collections;
using UnityEngine;
// Opcional: Si usas NavMeshAgent para moverlo
// using UnityEngine.AI; 

public class Enemigo : MonoBehaviour
{
    [Header("Estadísticas")]
    public float salud = 100f;
    public float velocidad = 5f;

    private float velocidadOriginal;
    private Coroutine corrutinaRalentizacion;

    // Opcional: Referencia al agente de movimiento
    // private NavMeshAgent agent; 

    [Header("Feedback de Daño")]
    public Material materialFlash; // Un material rojo brillante

    private Material materialOriginal;
    private Renderer rend;
    private Coroutine flashCoroutine; // Para controlar el flash

    void Start()
    {
        velocidadOriginal = velocidad;
        // agent = GetComponent<NavMeshAgent>();
        // if(agent != null) agent.speed = velocidad;

        // Obtenemos el Renderer (puede ser de este objeto o de un hijo)
        rend = GetComponentInChildren<Renderer>(); 
        if(rend != null)
        {
            materialOriginal = rend.material; // Guardamos el material normal
        }
    }

    // Función pública llamada por el proyectil
    public void RecibirDaño(float cantidad)
    {
        salud -= cantidad;
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