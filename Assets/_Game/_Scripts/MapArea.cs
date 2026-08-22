using UnityEngine;

public class MapArea : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private Vector2 size = new Vector2(40f, 40f);
    [SerializeField] private Vector3 centerOffset = Vector3.zero;

    [Header("Spawn Points")]
    public Transform playerSpawnPoint;

    public Vector2 GetMinBounds()
    {
        float minX = transform.position.x + centerOffset.x - size.x / 2f;
        float minZ = transform.position.z + centerOffset.z - size.y / 2f;
        return new Vector2(minX, minZ);
    }

    public Vector2 GetMaxBounds()
    {
        float maxX = transform.position.x + centerOffset.x + size.x / 2f;
        float maxZ = transform.position.z + centerOffset.z + size.y / 2f;
        return new Vector2(maxX, maxZ);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position + centerOffset;
        Vector3 boxSize = new Vector3(size.x, 0.1f, size.y);
        Gizmos.DrawWireCube(center, boxSize);

        if (playerSpawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(playerSpawnPoint.position, 0.5f);
        }
    }
}