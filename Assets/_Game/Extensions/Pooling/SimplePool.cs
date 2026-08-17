using UnityEngine;
using System.Collections.Generic;

public static class SimplePool
{
    private static Dictionary<PoolType, Pool> poolIstance = new Dictionary<PoolType, Pool>();
    //khoi tao pool moi
    public static void Preload(GameUnit prefab, int amount, Transform parent)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is null");
            return;
        }

        if (!poolIstance.ContainsKey(prefab.PoolType) || poolIstance[prefab.PoolType] == null)
        {
            Pool p = new Pool();
            p.Preload(prefab, amount, parent);
            poolIstance[prefab.PoolType] = p;
        }
    }

    //lay phan tu ra tu pool
    public static T Spawn<T>(PoolType poolType, Vector3 pos, Quaternion rot) where T : GameUnit
    {
        if (!poolIstance.ContainsKey(poolType))
        {
            Debug.LogError(poolType + "is not preload");
            return null;
        }
        else
        {
            return poolIstance[poolType].Spawn(pos, rot) as T;
        }
    }

    //tra phan tu ve pool
    public static void Despawn(GameUnit unit)
    {
        if (!poolIstance.ContainsKey(unit.PoolType))
        {
            Debug.LogError("Pool not found for type: " + unit.PoolType);
        }
        poolIstance[unit.PoolType].Despawn(unit);
    }

    //thu thap phan tu
    public static void Collect(PoolType poolType)
    {
        if (!poolIstance.ContainsKey(poolType))
        {
            Debug.LogError("Pool not found for type: " + poolType);

        }
        poolIstance[poolType].Collect();
    }

    //thu thap tat ca cac phan tu
    public static void CollectAll()
    {
        foreach (var item in poolIstance.Values)
        {
            item.Collect();
        }
    }

    //destroy 1 pool
    public static void Release(PoolType poolType)
    {
        if (!poolIstance.ContainsKey(poolType))
        {
            Debug.LogError("Pool not found for type: " + poolType);

        }
        poolIstance[poolType].Release();
    }
    //destroy tat ca cac pool
    public static void ReleaseAll()
    {
        foreach (var item in poolIstance.Values)
        {
            item.Release();
        }
    }
}

public class Pool
{
    Transform parent;
    GameUnit prefab;
    //list chua cac unit dang o trong pool
    Queue<GameUnit> inactives = new Queue<GameUnit>();
    //list chua cac unit dang duoc su dung
    List<GameUnit> actives = new List<GameUnit>();

    //khoi tao pool voi 1 prefab va so luong ban dau
    public void Preload(GameUnit prefab, int amount, Transform parent)
    {
        this.parent = parent;
        this.prefab = prefab;
        for (int i = 0; i < amount; i++)
        {
            Despawn(GameObject.Instantiate(prefab, parent));
        }
    }

    //lay phan tu ra tu pool
    public GameUnit Spawn(Vector3 pos, Quaternion rot)
    {
        GameUnit unit;
        if (inactives.Count <= 0)
        {
            unit = GameObject.Instantiate(prefab, parent);
        }
        else
        {
            unit = inactives.Dequeue();
        }
        unit.TF.SetPositionAndRotation(pos, rot);
        actives.Add(unit);
        unit.gameObject.SetActive(true);
        return unit;
    }

    //tra phan tu ve pool
    public void Despawn(GameUnit unit)
    {
        if (unit != null && unit.gameObject.activeSelf)
        {
            actives.Remove(unit);
            inactives.Enqueue(unit);
            unit.gameObject.SetActive(false);
            unit.TF.SetParent(this.parent);
        }
    }

    //thu thap tat ca cac phan tu dang dung ve pool
    public void Collect()
    {
        while (actives.Count > 0)
        {
            Despawn(actives[0]);
        }
    }
    //destroy tat ca cac phan tu trong pool
    public void Release()
    {
        Collect();
        while (inactives.Count > 0)
        {
            GameObject.Destroy(inactives.Dequeue().gameObject);
        }
        inactives.Clear();
    }
}