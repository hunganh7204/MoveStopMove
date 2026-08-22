using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Data & References")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private Player player;

    public int CurrentLevelIndex { get; private set; } = 1;

    private MapArea currentSpawnedMap;
    private LevelItem currentLevelItem;

    private int botsSpawnedCount;
    private int botsKilledCount;

    public void OnInit(int levelIndex)
    {
        CurrentLevelIndex = levelIndex;
        currentLevelItem = levelData.GetLevel(levelIndex);
        if (currentLevelItem == null) return;

        botsSpawnedCount = 0;
        botsKilledCount = 0;

        if (currentSpawnedMap != null)
        {
            Destroy(currentSpawnedMap.gameObject);
        }

        if (currentLevelItem.mapPrefab != null)
        {
            currentSpawnedMap = Instantiate(currentLevelItem.mapPrefab);

            if (currentSpawnedMap.playerSpawnPoint != null && player != null)
            {
                player.OnInit();
                player.transform.position = currentSpawnedMap.playerSpawnPoint.position;
                player.transform.rotation = currentSpawnedMap.playerSpawnPoint.rotation;
            }

        }

        BotManager.Instance.OnInit(currentSpawnedMap);
    }

    public bool CanSpawnMoreBots() => botsSpawnedCount < currentLevelItem.totalBots;
    public void RegisterBotSpawned() => botsSpawnedCount++;

    public void RegisterBotKilled()
    {
        botsKilledCount++;
        if (botsKilledCount >= currentLevelItem.totalBots)
        {
            GameManager.Instance.ChangeState(GameState.Win);
        }
    }
}