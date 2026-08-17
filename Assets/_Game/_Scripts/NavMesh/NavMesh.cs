using UnityEngine;
using UnityEngine.AI;

public class BotNavMesh : MonoBehaviour
{
    private NavMeshAgent agent;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }
    public void SyncPosition(Vector3 currentPos)
    {
        agent.nextPosition = currentPos;
    }

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public Vector3 GetSteeringDirection(Vector3 currentPos)
    {
        if (agent.pathPending || agent.path.corners.Length == 0) return Vector3.zero;

        Vector3 target = agent.steeringTarget;
        target.y = currentPos.y;

        Vector3 dir = target - currentPos;
        if (dir.sqrMagnitude < 0.05f) return Vector3.zero;

        return dir.normalized;
    }

    public void StopCalculating()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }

    public bool IsAtDestination(Vector3 currentPos)
    {
        if (agent.pathPending) return false;

        Vector3 dest = agent.destination;
        dest.y = currentPos.y;
        return Vector3.Distance(currentPos, dest) <= agent.stoppingDistance + 0.1f;
    }

    public bool TryGetRandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 rand2D = Random.insideUnitCircle * range;
            Vector3 randomDirection = center + new Vector3(rand2D.x, 0f, rand2D.y);

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, range, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    if (Vector3.Distance(center, hit.position) > 3f)
                    {
                        result = hit.position;
                        return true;
                    }
                }
            }
        }
        result = Vector3.zero;
        return false;
    }
}
