using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotManager : Singleton<BotManager>
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Camera mainCam;

    [Header("Spawn Settings")]
    public int maxBotsOnMap = 10;
    [SerializeField] private float minDistanceBetweenBots = 3f;

    private MapArea currentMapArea;
    private List<Bot> activeBots = new List<Bot>();

    public void OnInit(MapArea mapArea)
    {
        if (mainCam == null) mainCam = Camera.main;
        currentMapArea = mapArea;
        ClearAllBots();
    }

    private void Update()
    {
        if (GameManager.Instance.GetGameState() != GameState.Playing) return;

        for (int i = activeBots.Count - 1; i >= 0; i--)
        {
            if (activeBots[i] == null || activeBots[i].IsDead())
            {
                activeBots.RemoveAt(i);
                LevelManager.Instance.RegisterBotKilled();
            }
        }
        if (activeBots.Count < maxBotsOnMap && LevelManager.Instance.CanSpawnMoreBots())
        {
            SpawnBot();
        }
    }

    private void SpawnBot()
    {
        if (currentMapArea == null) return;

        Vector3 spawnPos;
        if (!TryGetValidSpawnPosition(out spawnPos)) return;

        Bot bot = SimplePool.Spawn<Bot>(PoolType.Bot, spawnPos, Quaternion.identity);
        if (bot != null)
        {
            bot.OnInit();
            int randomOffset = Random.Range(-2, 3);
            int newBotLevel = Mathf.Max(1, player.level + randomOffset);
            bot.SetLevel(newBotLevel);
            bot.EquipRandomItems();

            activeBots.Add(bot);
            LevelManager.Instance.RegisterBotSpawned();
        }
    }

    private bool TryGetValidSpawnPosition(out Vector3 resultPos)
    {
        resultPos = Vector3.zero;
        int attempts = 0;
        Vector2 minBounds = currentMapArea.GetMinBounds();
        Vector2 maxBounds = currentMapArea.GetMaxBounds();

        while (attempts < 30)
        {
            attempts++;
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            float randomZ = Random.Range(minBounds.y, maxBounds.y);
            Vector3 testPos = new Vector3(randomX, currentMapArea.transform.position.y, randomZ);

            Vector3 viewportPos = mainCam.WorldToViewportPoint(testPos);
            bool isOffScreen = viewportPos.x < -0.1f || viewportPos.x > 1.1f ||
                               viewportPos.y < -0.1f || viewportPos.y > 1.1f || viewportPos.z < 0;
            if (!isOffScreen) continue;

            bool isTooClose = false;
            foreach (var bot in activeBots)
            {
                if ((bot.TF.position - testPos).sqrMagnitude < (minDistanceBetweenBots * minDistanceBetweenBots))
                {
                    isTooClose = true;
                    break;
                }
            }
            if (isTooClose) continue;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(testPos, out hit, 2f, NavMesh.AllAreas))
            {
                resultPos = hit.position;
                return true;
            }
        }
        return false;
    }

    public void ClearAllBots()
    {
        foreach (var bot in activeBots)
        {
            if (bot != null && bot.gameObject.activeInHierarchy) bot.OnDeath();
        }
        activeBots.Clear();
    }
}