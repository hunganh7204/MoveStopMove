using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AccessoryItem
{
    public string id;
    public string name;
    public GameObject visualPrefab;
    public int price;

}

[CreateAssetMenu(fileName = "AccessoryData", menuName = "ScriptableObjects/Equipment/AccessoryData", order = 3)]
public class AccessoryData : ScriptableObject
{
    public List<AccessoryItem> items = new List<AccessoryItem>();

    public AccessoryItem GetItem(string id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == id) return items[i];
        }
        return null;
    }
}