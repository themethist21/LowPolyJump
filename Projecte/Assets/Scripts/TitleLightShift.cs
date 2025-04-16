using UnityEngine;

public class TitleLightShift : MonoBehaviour
{
    public float rotateVelocity = 5.0f;
    void Update()
    {
        this.transform.Rotate(new Vector3(rotateVelocity * Time.deltaTime, 0, rotateVelocity * Time.deltaTime));       
    }
}
