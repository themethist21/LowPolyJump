using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour
{

    public UnityEvent collectCoin;
    private Transform model;

    private float appearVelocity = 3.0f;
    private float expandVelocity = 5.0f;
    private float expandHeight = 0.1f;

    private bool makeVisible = false;
    private bool isVisible = false;

    private ObstacleStates state = ObstacleStates.Stay;

    private float scale;
    private float expandTime;

    private float offset;
    private float _angularVel = 2.0f;
    private float _degreesPerSec = 40.0f;

    private void Start()
    {
        collectCoin.AddListener(GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().IncrementScore);
        collectCoin.AddListener(GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>().UpdateScore);

        model = this.gameObject.transform.GetChild(0);
        offset = Random.Range(1, 10);
        
        model.transform.localScale = Vector3.zero;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFXCoin(0.2f); // Reproducir sonido de moneda
            collectCoin.Invoke();
            Destroy(gameObject);
        }
    }

    private void Update()
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
        model.SetPositionAndRotation(new Vector3(model.position.x, model.position.y + 0.0005f * Mathf.Sin(offset + _angularVel * Time.time), model.position.z), model.rotation);
        model.Rotate(Vector3.up * _degreesPerSec * Time.deltaTime);

        if (isVisible != makeVisible)
        {
            state = ObstacleStates.Move;
            scale = model.transform.localScale.x;
        }
    }

    private void MoveUpdate()
    {
        if (makeVisible)
        {
            if (model.transform.localScale.x <= 1)
            {
                scale += appearVelocity * Time.deltaTime;
                model.transform.localScale = Vector3.one * scale;
            }
            else
            {
                expandTime = Time.time;
                state = ObstacleStates.Expand;
            }
        }
        else
        {
            if (model.transform.localScale.x > 0)
            {
                scale -= appearVelocity * Time.deltaTime;
                model.transform.localScale = Vector3.one * scale;
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
        if (model.transform.localScale.x > 1)
        {
            scale = 1.0f + expandHeight * Mathf.Sin(expandVelocity * (Time.time - expandTime));
            model.transform.localScale = Vector3.one * scale;
        }
        else
        {
            model.transform.localScale = Vector3.one;
            isVisible = true;
            state = ObstacleStates.Stay;
        }
    }

    public void SetVisible(bool b)
    {
        makeVisible = b;
    }
}
