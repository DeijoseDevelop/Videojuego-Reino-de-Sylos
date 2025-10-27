using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    [Header("Configuración de Knockback")]
    [SerializeField] private float knockbackDuration = 0.3f; // Duración del empujón
    [SerializeField] private float knockbackDamping = 0.5f; // Amortiguación del empujón
    
    private CharacterController characterController;
    private Vector3 knockbackVelocity;
    private bool isKnockedBack = false;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("PlayerKnockback requiere un CharacterController");
        }
    }
    
    void Update()
    {
        // Aplicar el knockback cada frame si está activo
        if (isKnockedBack)
        {
            ApplyKnockbackMotion();
        }
    }
    
    /// <summary>
    /// Aplica un knockback al jugador
    /// </summary>
    public void ApplyKnockback(Vector3 force)
    {
        knockbackVelocity = force;
        isKnockedBack = true;
    }
    
    /// <summary>
    /// Aplica el movimiento de knockback
    /// </summary>
    private void ApplyKnockbackMotion()
    {
        if (characterController != null)
        {
            // Mover el CharacterController con el knockback
            characterController.Move(knockbackVelocity * Time.deltaTime);
            
            // Reducir gradualmente la velocidad del knockback
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDamping * Time.deltaTime);
            
            // Detener el knockback cuando sea insignificante
            if (knockbackVelocity.magnitude < 0.1f)
            {
                isKnockedBack = false;
                knockbackVelocity = Vector3.zero;
            }
        }
    }
    
    /// <summary>
    /// Obtiene si el jugador está siendo empujado
    /// </summary>
    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }
    
    /// <summary>
    /// Cancela el knockback actual
    /// </summary>
    public void CancelKnockback()
    {
        isKnockedBack = false;
        knockbackVelocity = Vector3.zero;
    }
}

