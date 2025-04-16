using UnityEngine;

public class Decoration : MonoBehaviour
{
    private GameObject renderedObject;

    private float appearVelocity = 3.0f;
    private float expandVelocity = 5.0f;
    private float expandHeight = 0.1f;

    private bool makeVisible = false;
    private bool isVisible = false;

    private ObstacleStates state = ObstacleStates.Stay;

    private float scale;
    private float expandTime;

    void Start()
    {
        renderedObject = gameObject;
        renderedObject.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case ObstacleStates.Stay:
                StayUpdate();
                break;
            case ObstacleStates.Move:
                MoveUpdate();
                break;
            case ObstacleStates.Expand:
                ExpandUpdate();
                break;
        }

    }

    private void StayUpdate()
    {
        if (isVisible != makeVisible)
        {
            state = ObstacleStates.Move;
            scale = renderedObject.transform.localScale.x;
        }
    }

    private void MoveUpdate()
    {
        if (makeVisible)
        {
            if (renderedObject.transform.localScale.x <= 1)
            {
                scale += appearVelocity * Time.deltaTime;
                renderedObject.transform.localScale = Vector3.one * scale;
            }
            else
            {
                expandTime = Time.time;
                state = ObstacleStates.Expand;
            }
        }
        else
        {
            if (renderedObject.transform.localScale.x > 0)
            {
                scale -= appearVelocity * Time.deltaTime;
                renderedObject.transform.localScale = Vector3.one * scale;
            }
            else
            {
                state = ObstacleStates.Stay;
                isVisible = false;
            }
        }
    }

    private void ExpandUpdate()
    {
        if (renderedObject.transform.localScale.x > 1)
        {
            scale = 1.0f + expandHeight * Mathf.Sin(expandVelocity * (Time.time - expandTime));
            renderedObject.transform.localScale = Vector3.one * scale;
        }
        else
        {
            renderedObject.transform.localScale = Vector3.one;
            isVisible = true;
            state = ObstacleStates.Stay;
        }
    }

    public void SetVisible(bool b)
    {
        makeVisible = b;
    }
}
