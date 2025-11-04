using UnityEngine;
using UnityEngine.UI; // ¡Importante para las barras!
using System.Collections; // Para Corrutinas

public class PlayerStats : MonoBehaviour
{
    [Header("Estadísticas de Salud")]
    public float maxSalud = 100f;
    public float currentSalud;

    [Header("Estadísticas de Escudo")]
    public float currentEscudo = 0f;
    public GameObject vfxEscudo; // Movimos la referencia aquí

    [Header("Estadísticas de Maná")]
    public float maxMana = 100f;
    public float currentMana;
    public float manaRegenRate = 5f; // Maná por segundo

    [Header("Referencias de UI (HUD)")]
    public Slider healthBar;
    public Slider manaBar;
    // Opcional: public Slider shieldBar;

    // --- Referencias Internas ---
    private Coroutine regenCorutina;

    void Start()
    {
        // Inicializar todas las estadísticas
        currentSalud = maxSalud;
        currentMana = maxMana;
        currentEscudo = 0f;

        if (vfxEscudo != null)
        {
            vfxEscudo.SetActive(false);
        }

        // Configurar las barras de UI al inicio
        if (healthBar != null)
        {
            healthBar.maxValue = maxSalud;
            healthBar.value = currentSalud;
        }
        if (manaBar != null)
        {
            manaBar.maxValue = maxMana;
            manaBar.value = currentMana;
        }

        // Iniciar la regeneración de maná
        regenCorutina = StartCoroutine(RegenerarMana());
    }

    private IEnumerator RegenerarMana()
    {
        // Bucle infinito que se ejecuta en segundo plano
        while (true)
        {
            if (currentMana < maxMana)
            {
                currentMana += manaRegenRate * Time.deltaTime;
                currentMana = Mathf.Clamp(currentMana, 0, maxMana);
                ActualizarBarraMana();
            }
            // Espera al siguiente frame
            yield return null;
        }
    }

    // --- API PÚBLICA PARA HECHIZOS ---

    public bool GastarMana(float costo)
    {
        if (currentMana >= costo)
        {
            currentMana -= costo;
            ActualizarBarraMana();
            return true; // ¡Éxito! Hechizo se puede lanzar.
        }
        else
        {
            Debug.Log("¡No hay suficiente Maná!");
            // Aquí podrías reproducir un sonido de "fizzle" o "error"
            return false; // ¡Fallo! No se lanza el hechizo.
        }
    }

    // --- API PÚBLICA PARA DAÑO Y BUFFS ---

    public void RecibirDaño(float cantidad)
    {
        Debug.Log($"Jugador recibe {cantidad} de daño.");

        if (currentEscudo > 0)
        {
            float dañoAbsorbido = Mathf.Min(currentEscudo, cantidad);
            currentEscudo -= dañoAbsorbido;
            cantidad -= dañoAbsorbido;
            Debug.Log($"Escudo absorbió {dañoAbsorbido}. Escudo restante: {currentEscudo}");

            if (currentEscudo <= 0)
            {
                if (vfxEscudo != null) vfxEscudo.SetActive(false);
                Debug.Log("¡Escudo roto!");
            }
        }

        if (cantidad > 0)
        {
            currentSalud -= cantidad;
            ActualizarBarraSalud();
            Debug.Log($"Salud golpeada. Salud restante: {currentSalud}");
        }

        if (currentSalud <= 0)
        {
            currentSalud = 0; // Clamp
            Debug.Log("¡El jugador ha muerto!");
            // Aquí va la lógica de muerte
        }
    }

    public void AplicarEscudo(float cantidad)
    {
        currentEscudo = cantidad;
        if (vfxEscudo != null)
        {
            vfxEscudo.SetActive(true);
        }
        // Aquí podrías iniciar una corrutina para que el escudo dure X segundos
    }

    // --- Helpers Internos de UI ---

    private void ActualizarBarraMana()
    {
        if (manaBar != null)
        {
            manaBar.value = currentMana;
        }
    }

    private void ActualizarBarraSalud()
    {
        if (healthBar != null)
        {
            healthBar.value = currentSalud;
        }
    }
}