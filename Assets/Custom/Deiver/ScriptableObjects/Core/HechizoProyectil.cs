using UnityEngine;

// [CreateAssetMenu] es la magia. Esto añade la opción al menú de clic derecho.
[CreateAssetMenu(menuName = "Sistema de Hechizos/Hechizo de Proyectil")]
public class HechizoProyectil : Hechizo
{
    [Header("Configuración del Proyectil")]
    public GameObject prefabProyectil;

    public override void Lanzar(LanzadorDeHechizos lanzador)
    {
        Debug.Log($"Lanzando Proyectil: {nombreHechizo}");

        // --- 1. Lógica de Raycast (movida desde el Lanzador) ---
        Vector3 puntoDeMira;
        RaycastHit hit;
        Ray rayo = lanzador.camaraDelJugador.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(rayo, out hit))
        {
            puntoDeMira = hit.point;
        }
        else
        {
            puntoDeMira = rayo.GetPoint(100);
        }

        Vector3 direccion = (puntoDeMira - lanzador.puntoDeLanzamiento.position).normalized;

        // --- 2. Instanciar ---
        GameObject proyectilGO = Instantiate(
            prefabProyectil,
            lanzador.puntoDeLanzamiento.position,
            Quaternion.LookRotation(direccion)
        );

        // --- 3. Inicializar ---
        ProyectilDeHechizo scriptProyectil = proyectilGO.GetComponent<ProyectilDeHechizo>();
        if (scriptProyectil != null)
        {
            scriptProyectil.InicializarLanzamiento(direccion);
        }

        // --- 4. Audio ---
        if (sonidoLanzamiento != null)
        {
            lanzador.audioSource.PlayOneShot(sonidoLanzamiento);
        }
    }
}