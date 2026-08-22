using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HatItem
{
    public string id;          
    public string name;
    public GameObject visualPrefab; 
    public int price;

    [Header("Stats Buff")]
    public float bonusAttackRange; 
}

[CreateAssetMenu(fileName = "HatData", menuName = "ScriptableObjects/Equipment/HatData", order = 1)]
public class HatData : ScriptableObject
{
    public List<HatItem> items = new List<HatItem>();

    public HatItem GetItem(string id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id) return items[i];
        }
        return null;
    }
}