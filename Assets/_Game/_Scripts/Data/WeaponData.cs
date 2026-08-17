using System;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Knife = 0,
    Hammer = 1,
    Boomerang = 2,
}

[Serializable]
public class WeaponItem
{
    public string name;
    public WeaponType type;
    public BulletBase bulletPrefab;
    public GameObject visual;
    public int price;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public List<WeaponItem> weapons = new List<WeaponItem>();
    public WeaponItem GetWeapon(WeaponType type)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].type == type)
            {
                return weapons[i];
            }
        }
        return null;
    }
}
