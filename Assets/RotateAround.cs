using UnityEngine;

public class RotateAround : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;

    private void Update()
    {
        transform.RotateAround(target.transform.position, Vector3.up, Time.deltaTime * speed);
        transform.LookAt(target, Vector3.up);
    }
}