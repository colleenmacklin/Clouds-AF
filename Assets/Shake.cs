using UnityEngine;

public class Shake : MonoBehaviour
{
    Vector2 startingPos;
    public float _speed = 1.0f;
    public float amount = 1.0f;


    void Awake()
    {
        startingPos.x = transform.position.x;
        startingPos.y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3((startingPos.x + Mathf.Sin(Time.time * _speed) * amount), (startingPos.y + (Mathf.Sin(Time.time * _speed) * amount)), 0);
    }
}