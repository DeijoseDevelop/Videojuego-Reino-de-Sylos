using UnityEngine;

[CreateAssetMenu(menuName = "Sistema de Hechizos/Hechizo de Buff")]
public class HechizoBuff : Hechizo
{
    [Header("Configuración del Buff")]
    public float cantidadEscudo;
    // Podrías añadir duración, VFX, etc.

    public override void Lanzar(LanzadorDeHechizos lanzador)
    {
        Debug.Log($"Lanzando Buff: {nombreHechizo}");

        // El hechizo le "pide" al lanzador que se aplique el efecto a sí mismo.
        lanzador.AplicarEscudo(cantidadEscudo);

        // --- Audio ---
        if (sonidoLanzamiento != null)
        {
            lanzador.audioSource.PlayOneShot(sonidoLanzamiento);
        }
    }
}