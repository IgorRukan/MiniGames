using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, target.transform.position, ref velocity, smoothSpeed);
        smoothedPosition.z = -10;
        transform.position = smoothedPosition;
    }
    
}
