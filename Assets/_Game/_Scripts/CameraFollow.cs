using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Speed")]
    [SerializeField] private Transform target;     
    [SerializeField] private float smoothSpeed = 5f; 
    [Header("Offsets")]
    [SerializeField] private Vector3 baseOffset = new Vector3(0, 15, -15); 

    private Vector3 currentOffset;

    private void Awake()
    {
        currentOffset = baseOffset;
    }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
    public void UpdateZoom(float playerScale)
    {
        currentOffset = baseOffset * playerScale;
    }
}