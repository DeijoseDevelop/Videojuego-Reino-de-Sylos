using UnityEngine;

// Esta es la clase base para TODOS los hechizos
public abstract class Hechizo : ScriptableObject
{
    [Header("Info Básica del Hechizo")]
    public string nombreHechizo;
    [TextArea]
    public string descripcion;
    public Sprite icono;
    public AudioClip sonidoLanzamiento;

    [Header("Costos")]
    public float costoMana; // Aún no lo usamos, pero es bueno tenerlo
    public float cooldown;  // Ídem

    // El método clave. Cada tipo de hechizo (proyectil, buff, etc.)
    // debe implementar su propia forma de "Lanzar".
    // Le pasamos el 'lanzador' para que el hechizo pueda acceder a
    // la cámara, el punto de lanzamiento, el audio source, etc.
    public abstract void Lanzar(LanzadorDeHechizos lanzador);
}