using UnityEngine;

[CreateAssetMenu(menuName = "Sistema de Hechizos/Hechizo de Buff")]
public class HechizoBuff : Hechizo
{
    [Header("Configuración del Buff")]
    public float cantidadEscudo;
    // Podrías añadir duración, VFX, etc.

    public override void Lanzar(LanzadorDeHechizos lanzador)
    {
        // --- ¡¡INICIO DE LA MODIFICACIÓN!! ---

        // 1. Comprobamos si el jugador tiene suficiente maná Y lo gastamos.
        //    (Asumimos que 'costoDeMana' existe en tu clase base 'Hechizo.cs')
        if (lanzador.playerStats.GastarMana(costoDeMana))
        {
            // --- ¡ÉXITO! EL MANÁ SE GASTÓ. EJECUTAMOS EL HECHIZO. ---

            Debug.Log($"Lanzando Buff: {nombreHechizo}");

            // ¡¡CORRECCIÓN IMPORTANTE!!
            // La lógica de 'AplicarEscudo' ahora está en el script 'PlayerStats'.
            lanzador.playerStats.AplicarEscudo(cantidadEscudo);

            // --- Audio ---
            if (sonidoLanzamiento != null)
            {
                lanzador.audioSource.PlayOneShot(sonidoLanzamiento);
            }
        }
        else
        {
            // --- ¡FALLO! NO HAY SUFICIENTE MANÁ ---
            // 'PlayerStats.GastarMana()' ya se habrá quejado en la consola.
        }
        // --- ¡¡FIN DE LA MODIFICACIÓN!! ---
    }
}