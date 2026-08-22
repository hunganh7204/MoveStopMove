using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleScan : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Player player;
    [SerializeField] private Transform TF;

    private HashSet<Obstacle> obstacles = new HashSet<Obstacle>();
    private HashSet<Obstacle> currentFadeObstacles = new HashSet<Obstacle>();

    void Update()
    {
        currentFadeObstacles.Clear();
        CheckCamBlocking();
        CheckAttackRange();
        
        foreach(var obstacle in obstacles)
        {
            if (!currentFadeObstacles.Contains(obstacle))
            {
                obstacle.FadeIn();
            }
        }

        foreach(var obstacle in currentFadeObstacles)
        {
            if (!obstacles.Contains(obstacle)){
                obstacle.FadeOut();
            }
        }

        obstacles.Clear();
        foreach(var obstacle in currentFadeObstacles)
        {
            obstacles.Add(obstacle);
        }
    }

    private void CheckCamBlocking()
    {
        Vector3 dir = TF.position - cameraTransform.position;
        float distance = dir.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, dir.normalized, distance,obstacleLayer);
        foreach (RaycastHit hit in hits)
        {
            Obstacle obstacle = Cache.GetObstacle(hit.collider);
            if (obstacle != null)
            {
                currentFadeObstacles.Add(obstacle);
            }
        }
    }
    private void CheckAttackRange()
    {
        Collider[] cols = Physics.OverlapSphere(TF.position, player.GetAttackRange(), obstacleLayer);
        for(int i=0; i<cols.Length; i++)
        {
            Obstacle obstacle = Cache.GetObstacle(cols[i]);
            if (obstacle != null)
            {
                currentFadeObstacles.Add(obstacle);
            }
        }
    }
}
