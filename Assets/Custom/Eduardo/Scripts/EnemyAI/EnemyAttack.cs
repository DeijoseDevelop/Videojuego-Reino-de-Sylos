using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float attackCooldown = 2f;  // Tiempo en segundos entre ataques
    [SerializeField] private float attackPower = 5f;     // Fuerza del empujón

    [Header("Referencias")]
    [SerializeField] private Transform playerTransform; // Referencia al jugador


    private float lastAttackTime = 0f;

    void Start()
    {
        // Buscar el jugador automáticamente si no se ha asignado
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
    }

    /// Ataca al jugador empujándolo. Solo funciona si el cooldown ha terminado.
    public void AttackPlayer()
    {
        // Verificar si el cooldown ha terminado
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return;
        }

        // Verificar que tengamos referencia al jugador
        if (playerTransform == null)
        {
            Debug.LogWarning("No se encontró referencia al jugador");
            return;
        }

        // Calcular la dirección del empujón
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        // Buscar el script de knockback del jugador primero
        PlayerKnockback playerKnockback = playerTransform.GetComponent<PlayerKnockback>();
        if (playerKnockback != null)
        {
            playerKnockback.ApplyKnockback(direction * attackPower);
            Debug.Log("Empujando con PlayerKnockback");
        }
        // Intentar empujar con CharacterController
        else
        {
            CharacterController playerCC = playerTransform.GetComponent<CharacterController>();
            if (playerCC != null)
            {
                // Aplicar múltiples movimientos para un efecto más visible
                StartCoroutine(KnockbackCharacterController(playerCC, direction * attackPower));
                Debug.Log("Empujando con CharacterController");
            }
            // Si no tiene CharacterController, intentar con Rigidbody
            else
            {
                Rigidbody playerRigidbody = playerTransform.GetComponent<Rigidbody>();
                if (playerRigidbody != null)
                {
                    playerRigidbody.AddForce(direction * attackPower, ForceMode.Impulse);
                    Debug.Log("Empujando con Rigidbody");
                }
                else
                {
                    Debug.LogWarning("El jugador no tiene CharacterController ni Rigidbody. Considera añadir el script PlayerKnockback al jugador.");
                }
            }
        }

        // Actualizar el tiempo del último ataque
        lastAttackTime = Time.time;
    }

    private System.Collections.IEnumerator KnockbackCharacterController(CharacterController cc, Vector3 knockbackForce)
    {
        float elapsedTime = 0f;
        float knockbackDuration = 0.3f; // Duración del empujón

        while (elapsedTime < knockbackDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / knockbackDuration;
            float forceMultiplier = Mathf.Lerp(1f, 0f, normalizedTime); // Suavizado del empujón

            cc.Move(knockbackForce * forceMultiplier * Time.deltaTime);
            yield return null;
        }
    }

    /// Comprueba si el ataque está disponible (cooldown terminado)
    public bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }
}
