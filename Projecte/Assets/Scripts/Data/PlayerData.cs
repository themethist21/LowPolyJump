using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength;
    public float fallGravityMult;

    [Space(10)]
    [Header("Jump")]
    public float jumpInputBufferTime;
    [HideInInspector] public float jumpForce;
    public float jumpHeight;
    public float jumpTimeToApex;

    [Space(10)]
    [Header("General")]
    public float playerSpeed;

    private void OnValidate()
    {
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
    }

}
