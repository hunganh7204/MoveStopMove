using UnityEngine;

public class PoolControl : MonoBehaviour
{
    [SerializeField] PoolAmount[] poolAmounts;
    void Awake()
    {
        //load tu resources folder
        //GameUnit[] gameUnits = Resources.LoadAll<GameUnit>("Pool/");

        //for (int i = 0; i < gameUnits.Length; i++)
        //{
        //    SimplePool.Preload(gameUnits[i], 0, new GameObject(gameUnits[i].name).transform);
        //}
        //load tu list
        for (int i = 0; i < poolAmounts.Length; i++)
        {
            SimplePool.Preload(poolAmounts[i].prefab, poolAmounts[i].amount, poolAmounts[i].parent);
        }
    }
}

[System.Serializable]
public class PoolAmount
{
    public GameUnit prefab;
    public Transform parent;
    public int amount;
}

public enum PoolType
{
    Bot = 0,
    Player = 1,
    Knife = 2,
    Hammer = 3,
    Boomerang = 4,
}

