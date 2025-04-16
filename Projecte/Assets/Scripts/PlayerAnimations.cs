using UnityEngine;
public class PlayerAnimations : MonoBehaviour
{
    Animator animator;

    PlayerMovementHorizontal parentComponent;

    private float jumpInputBufferTime = 0.2f; // Tiempo máximo para guardar el salto en el buffer
    private float jumpBufferTimer = 0.0f; // Temporizador para el buffer de entrada

    void Start()
    {
        parentComponent = GetComponentInParent<PlayerMovementHorizontal>();

        animator = GetComponent<Animator>();

        // Ajusta la velocidad para que la animación dure 2 segundos
        float originalDuration = 3.160f; // Duración original de la animación (en segundos)
        float desiredDuration = 1f;  // Duración deseada de la animación
        animator.SetFloat("Multiplier", originalDuration / desiredDuration);
    }

    void Update()
    {
        // Guardar la acción de salto en el buffer si se presiona la tecla
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpInputBufferTime;
        }

        // Reducir el buffer con el tiempo
        if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // Reproducir la animación si se cumple la condición
        if (jumpBufferTimer > 0 && parentComponent.state == PlayerStates.Grounded)
        {
            animator.SetTrigger("JumpTrigger");
            jumpBufferTimer = 0; // Restablecer el buffer después de activar la animación
        }
    }
}
