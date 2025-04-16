using UnityEngine;

public enum TerrainStates
{
    Stay,
    Move,
    Bounce,
}
public class Terrain : MonoBehaviour
{
    private GameObject renderedObject;

    private float moveVelocity = 7.0f;
    public float downVelocity = 7.0f;
    private float downHeight = -8.0f;
    private float bounceVelocity = 5.0f;
    private float bounceHeight = 0.5f;

    private bool makeVisible = false;
    private bool isVisible = false;

    private float bounceStart;
    private float bounceTime;

    private TerrainStates state = TerrainStates.Stay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderedObject = this.transform.GetChild(0).gameObject;
        renderedObject.transform.Translate(Vector3.up * (downHeight  - this.transform.position.y));
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case TerrainStates.Stay:
                StayUpdate();
                break;
            case TerrainStates.Move:
                MoveUpdate();
                break;
            case TerrainStates.Bounce:
                BounceUpdate();
                break;
        }
 
    }

    private void StayUpdate()
    {
        if (isVisible != makeVisible)
        {
            state = TerrainStates.Move;
        }
    }

    private void MoveUpdate()
    {
        if (makeVisible)
        {
            if (renderedObject.transform.position.y < this.transform.position.y)
            {
                renderedObject.transform.Translate(moveVelocity * Time.deltaTime * Vector3.up);
            }
            else
            {
                state = TerrainStates.Bounce;
                bounceStart = this.transform.position.y;
                bounceTime = Time.time;
            }
        }
        else
        {
            if (renderedObject.transform.position.y > downHeight)
            {
                renderedObject.transform.Translate(downVelocity * Time.deltaTime * Vector3.down);
            }
            else
            {
                state = TerrainStates.Stay;
                isVisible = false;
            }
        }
    }
    
    private void BounceUpdate()
    {
        if (renderedObject.transform.position.y > this.transform.position.y)
        {
            float bouncePos = bounceStart + bounceHeight * Mathf.Sin(bounceVelocity * (Time.time - bounceTime));
            renderedObject.transform.SetPositionAndRotation(new Vector3(renderedObject.transform.position.x, bouncePos, renderedObject.transform.position.z), renderedObject.transform.rotation);
        }
        else
        {
            renderedObject.transform.SetPositionAndRotation(new Vector3(renderedObject.transform.position.x, this.transform.position.y, renderedObject.transform.position.z), renderedObject.transform.rotation);
            isVisible = true;
            state = TerrainStates.Stay;
        }
    }

    public void SetVisible(bool b)
    {
        makeVisible = b;
    }

    public void SetHide()
    {
        makeVisible = false;
    }
}
