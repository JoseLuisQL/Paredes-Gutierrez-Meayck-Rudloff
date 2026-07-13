using UnityEngine;

public class MovingRamp : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;
    private float t = 0f;
    private bool movingToB = true;

    void Start()
    {
        // If points are not set, make a default up/down movement relative to starting position
        if (pointA == Vector3.zero && pointB == Vector3.zero)
        {
            pointA = transform.position;
            pointB = transform.position + new Vector3(0, 5, 0);
        }
        transform.position = pointA;
    }

    void Update()
    {
        t += Time.deltaTime * speed;
        if (movingToB)
        {
            transform.position = Vector3.Lerp(pointA, pointB, t);
            if (t >= 1f)
            {
                t = 0f;
                movingToB = false;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(pointB, pointA, t);
            if (t >= 1f)
            {
                t = 0f;
                movingToB = true;
            }
        }
    }
    
    // So the player moves with the ramp
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}