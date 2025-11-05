using UnityEngine;
using UnityEngine.UI; // ¡Importante para las barras!
using System.Collections; // Para Corrutinas
using System.Collections.Generic; // Para efectos visuales

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

    [Header("Efectos Visuales de Maná")]
    [Tooltip("Imagen de fill de la barra de maná (para cambio de color dinámico)")]
    public Image manaBarFillImage;
    [Tooltip("Imagen de efecto de brillo/pulso durante regeneración")]
    public Image manaBarGlowImage;
    [Tooltip("Velocidad de animación suave de la barra")]
    [Range(5f, 30f)]
    public float manaBarAnimationSpeed = 15f;
    [Tooltip("Duración del efecto flash al gastar maná")]
    [Range(0.1f, 0.5f)]
    public float manaFlashDuration = 0.2f;
    [Tooltip("Intensidad del pulso durante regeneración")]
    [Range(0.1f, 0.5f)]
    public float manaRegenGlowIntensity = 0.3f;
    [Tooltip("Color cuando hay suficiente maná")]
    public Color manaColorFull = new Color(0.2f, 0.6f, 1f, 1f); // Azul
    [Tooltip("Color cuando el maná está bajo")]
    public Color manaColorLow = new Color(1f, 0.3f, 0.3f, 1f); // Rojo
    [Tooltip("Umbral de maná bajo (0-1, porcentaje)")]
    [Range(0f, 0.5f)]
    public float manaLowThreshold = 0.25f;
    [Tooltip("Fuerza del shake cuando no hay suficiente maná")]
    [Range(0f, 50f)]
    public float manaShakeIntensity = 20f;
    [Tooltip("Duración del shake cuando no hay suficiente maná")]
    [Range(0.1f, 1f)]
    public float manaShakeDuration = 0.3f;

    // --- Referencias Internas ---
    private Coroutine regenCorutina;
    private Coroutine manaAnimationCoroutine;
    private Coroutine manaFlashCoroutine;
    private Coroutine manaShakeCoroutine;
    private float targetManaValue; // Valor objetivo para animación suave
    private Vector3 originalManaBarPosition; // Posición original para shake
    private bool isRegeneratingMana = false;

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
            targetManaValue = currentMana;
            
            // Guardar posición original para efectos de shake
            originalManaBarPosition = manaBar.transform.localPosition;
            
            // Obtener referencia a la imagen de fill si no está asignada
            if (manaBarFillImage == null && manaBar.fillRect != null)
            {
                manaBarFillImage = manaBar.fillRect.GetComponent<Image>();
            }
            
            // Configurar color inicial
            ActualizarColorMana();
        }
        
        // Configurar imagen de glow si existe
        if (manaBarGlowImage != null)
        {
            Color glowColor = manaBarGlowImage.color;
            glowColor.a = 0f;
            manaBarGlowImage.color = glowColor;
        }

        // Iniciar la regeneración de maná
        regenCorutina = StartCoroutine(RegenerarMana());
        
        // Iniciar animación suave de la barra
        manaAnimationCoroutine = StartCoroutine(AnimarBarraMana());
    }

    private IEnumerator RegenerarMana()
    {
        // Bucle infinito que se ejecuta en segundo plano
        while (true)
        {
            if (currentMana < maxMana)
            {
                isRegeneratingMana = true;
                currentMana += manaRegenRate * Time.deltaTime;
                currentMana = Mathf.Clamp(currentMana, 0, maxMana);
                targetManaValue = currentMana;
                ActualizarColorMana();
                ActualizarEfectoRegeneracion();
            }
            else
            {
                isRegeneratingMana = false;
                ActualizarEfectoRegeneracion();
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
            targetManaValue = currentMana;
            ActualizarColorMana();
            
            // Efecto visual de gasto (flash)
            if (manaFlashCoroutine != null)
            {
                StopCoroutine(manaFlashCoroutine);
            }
            manaFlashCoroutine = StartCoroutine(EfectoFlashGastoMana());
            
            return true; // ¡Éxito! Hechizo se puede lanzar.
        }
        else
        {
            Debug.Log("¡No hay suficiente Maná!");
            
            // Feedback visual cuando no hay suficiente maná
            if (manaShakeCoroutine != null)
            {
                StopCoroutine(manaShakeCoroutine);
            }
            manaShakeCoroutine = StartCoroutine(EfectoShakeManaInsuficiente());
            
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
        // Ya no se actualiza directamente, se usa targetManaValue para animación suave
        targetManaValue = currentMana;
    }

    // --- Efectos Visuales de Maná ---

    private IEnumerator AnimarBarraMana()
    {
        // Animación suave continua de la barra de maná
        while (true)
        {
            if (manaBar != null)
            {
                float currentValue = manaBar.value;
                float difference = Mathf.Abs(currentValue - targetManaValue);
                
                if (difference > 0.1f)
                {
                    // Interpolación suave hacia el valor objetivo
                    manaBar.value = Mathf.Lerp(currentValue, targetManaValue, 
                        manaBarAnimationSpeed * Time.deltaTime);
                }
                else
                {
                    // Si está muy cerca, asignar directamente para evitar jitter
                    manaBar.value = targetManaValue;
                }
            }
            yield return null;
        }
    }

    private void ActualizarColorMana()
    {
        if (manaBarFillImage != null)
        {
            float manaPercentage = currentMana / maxMana;
            
            // Interpolar entre color bajo y color completo según el porcentaje
            if (manaPercentage <= manaLowThreshold)
            {
                // Si está por debajo del umbral, usar color de advertencia
                float t = manaPercentage / manaLowThreshold;
                manaBarFillImage.color = Color.Lerp(manaColorLow, manaColorFull, t);
            }
            else
            {
                // Si está por encima del umbral, usar color normal
                manaBarFillImage.color = manaColorFull;
            }
        }
    }

    private void ActualizarEfectoRegeneracion()
    {
        if (manaBarGlowImage != null)
        {
            Color glowColor = manaBarGlowImage.color;
            
            if (isRegeneratingMana && currentMana < maxMana)
            {
                // Efecto de pulso sutil durante regeneración
                float pulse = (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f; // Oscila entre 0 y 1
                glowColor.a = pulse * manaRegenGlowIntensity;
            }
            else
            {
                // Desvanecer el efecto cuando no se regenera
                glowColor.a = Mathf.Lerp(glowColor.a, 0f, Time.deltaTime * 5f);
            }
            
            manaBarGlowImage.color = glowColor;
        }
    }

    private IEnumerator EfectoFlashGastoMana()
    {
        if (manaBarFillImage != null)
        {
            Color originalColor = manaBarFillImage.color;
            Color flashColor = new Color(1f, 1f, 1f, 1f); // Blanco brillante
            
            float elapsed = 0f;
            while (elapsed < manaFlashDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / manaFlashDuration;
                
                // Flash rápido: blanco -> original
                float flashIntensity = 1f - (t * t); // Deceleración suave
                manaBarFillImage.color = Color.Lerp(originalColor, flashColor, flashIntensity * 0.5f);
                
                yield return null;
            }
            
            // Asegurar que vuelva al color correcto
            ActualizarColorMana();
        }
    }

    private IEnumerator EfectoShakeManaInsuficiente()
    {
        if (manaBar != null)
        {
            float elapsed = 0f;
            Vector3 originalPosition = manaBar.transform.localPosition;
            
            while (elapsed < manaShakeDuration)
            {
                elapsed += Time.deltaTime;
                
                // Shake aleatorio con deceleración
                float intensity = manaShakeIntensity * (1f - (elapsed / manaShakeDuration));
                Vector3 shakeOffset = new Vector3(
                    Random.Range(-intensity, intensity),
                    Random.Range(-intensity, intensity),
                    0f
                );
                
                manaBar.transform.localPosition = originalPosition + shakeOffset;
                
                yield return null;
            }
            
            // Restaurar posición original
            manaBar.transform.localPosition = originalPosition;
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