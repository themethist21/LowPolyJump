using UnityEngine;

public class PlayerShowcase : MonoBehaviour
{
    public float rotateVel = 1.0f;

    private Transform model;

    private void Start()
    {
        model = this.gameObject.transform.GetChild(0);
    }


    void Update()
    {
        model.Rotate(Vector3.up * rotateVel * Time.deltaTime);
    }
}
