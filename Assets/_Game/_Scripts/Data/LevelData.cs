using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelItem
{
    public int levelIndex;
    public MapArea mapPrefab;
    public int totalBots;  
}

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public List<LevelItem> levels = new List<LevelItem>();

    public LevelItem GetLevel(int index)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].levelIndex == index) return levels[i];
        }
        return null;
    }
}