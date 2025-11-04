using UnityEngine;

// [CreateAssetMenu] es la magia. Esto añade la opción al menú de clic derecho.
[CreateAssetMenu(menuName = "Sistema de Hechizos/Hechizo de Proyectil")]
public class HechizoProyectil : Hechizo
{
    [Header("Configuración del Proyectil")]
    public GameObject prefabProyectil;

    public override void Lanzar(LanzadorDeHechizos lanzador)
    {
        // --- ¡¡INICIO DE LA MODIFICACIÓN!! ---
        
        // 1. Comprobamos si el jugador tiene suficiente maná Y lo gastamos.
        //    (Asumimos que 'costoDeMana' existe en tu clase base 'Hechizo.cs')
        if (lanzador.playerStats.GastarMana(costoDeMana))
        {
            // --- ¡ÉXITO! EL MANÁ SE GASTÓ. EJECUTAMOS EL HECHIZO. ---
            
            Debug.Log($"Lanzando Proyectil: {nombreHechizo}");

            // --- 2. Lógica de Raycast (tu código original) ---
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

            // --- 3. Instanciar (tu código original) ---
            GameObject proyectilGO = Instantiate(
                prefabProyectil,
                lanzador.puntoDeLanzamiento.position,
                Quaternion.LookRotation(direccion)
            );

            // --- 4. Inicializar (tu código original) ---
            // (Asumiendo que tu prefab tiene un script llamado 'ProyectilDeHechizo' como en tu script anterior)
            ProyectilDeHechizo scriptProyectil = proyectilGO.GetComponent<ProyectilDeHechizo>();
            if (scriptProyectil != null)
            {
                scriptProyectil.InicializarLanzamiento(direccion);
            }

            // --- 5. Audio (tu código original) ---
            if (sonidoLanzamiento != null)
            {
                lanzador.audioSource.PlayOneShot(sonidoLanzamiento);
            }
        }
        else
        {
            // --- ¡FALLO! NO HAY SUFICIENTE MANÁ ---
            // 'PlayerStats.GastarMana()' ya se habrá quejado en la consola.
            // Opcional: Reproducir un sonido de "fallo" aquí.
        }
        // --- ¡¡FIN DE LA MODIFICACIÓN!! ---
    }
}