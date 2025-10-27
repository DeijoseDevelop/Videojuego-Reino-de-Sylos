using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProyectilDeHechizo : MonoBehaviour
{
    [Header("Configuración del Proyectil")]
    public float velocidad = 20f;
    public float tiempoDeVida = 5f;
    public GameObject prefabImpacto; // El prefab de "Explosión" o "ImpactoHielo"

    [Header("Efectos del Hechizo")]
    public float daño = 10f;

    [Header("Efectos de Estado (Opcional)")]
    public bool aplicarRalentización = false;
    public float multiplicadorRalentizacion = 0.5f; // 50% de la velocidad normal
    public float duracionRalentizacion = 3f;

    private Rigidbody rb;

    [Header("Audio Feedback (Impacto)")]
    public AudioClip sonidoImpacto;

    // void Start()
    // {
    //     rb = GetComponent<Rigidbody>();

    //     // Lanzamos el proyectil hacia adelante (basado en su propia rotación)
    //     rb.linearVelocity = transform.forward * velocidad;

    //     // Autodestrucción si no golpea nada
    //     Destroy(gameObject, tiempoDeVida);
    // }

    public void InicializarLanzamiento(Vector3 direccion)
    {
        rb = GetComponent<Rigidbody>();

        // Usamos la dirección calculada por el Raycast
        rb.linearVelocity = direccion * velocidad;

        // Hacemos que el proyectil mire en la dirección que se mueve
        // (Esto arregla la rotación automáticamente si usaste la Solución 2)
        if (rb.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }

        // Autodestrucción
        Destroy(gameObject, tiempoDeVida);
    }

    // Usamos OnCollisionEnter porque nuestros colliders NO son Triggers.
    void OnCollisionEnter(Collision collision)
    {
        // 1. Instanciar el efecto de impacto (explosión, etc.)
        if (prefabImpacto != null)
        {
            Instantiate(prefabImpacto, transform.position, Quaternion.identity);
        }

        if (sonidoImpacto != null)
        {
            AudioSource.PlayClipAtPoint(sonidoImpacto, transform.position);
        }

        // 2. Intentar aplicar efectos al objeto golpeado
        // Buscamos un script "Enemigo" en el objeto que golpeamos
        Enemigo enemigo = collision.gameObject.GetComponent<Enemigo>();
        if (enemigo != null)
        {
            // Aplicar daño
            enemigo.RecibirDaño(daño);

            // Aplicar efectos de estado si están marcados
            if (aplicarRalentización)
            {
                enemigo.AplicarRalentizacion(multiplicadorRalentizacion, duracionRalentizacion);
            }
        }

        // 3. Destruir el proyectil
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
