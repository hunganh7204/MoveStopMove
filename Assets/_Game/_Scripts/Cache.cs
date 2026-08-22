using UnityEngine;
using System.Collections.Generic;

public static class Cache
{
    private static Dictionary<Collider, Obstacle> dictObstacle = new Dictionary<Collider, Obstacle>();

    public static Obstacle GetObstacle(Collider col)
    {
        if (!dictObstacle.ContainsKey(col))
        {
            Obstacle obstacle = col.GetComponent<Obstacle>();
            dictObstacle.Add(col, obstacle);
        }

        return dictObstacle[col];
    }
}
