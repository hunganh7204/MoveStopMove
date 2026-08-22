using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PantItem
{
    public string id;
    public string name;
    public Material pantMaterial;
    public int price;

    [Header("Stats Buff")]
    public float bonusMoveSpeed;
}

[CreateAssetMenu(fileName = "PantData", menuName = "ScriptableObjects/Equipment/PantData", order = 2)]
public class PantData : ScriptableObject
{
    public List<PantItem> items = new List<PantItem>();

    public PantItem GetItem(string id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id) return items[i];
        }
        return null;
    }
}