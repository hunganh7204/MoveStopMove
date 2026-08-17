using UnityEngine;

public class GameUnit : MonoBehaviour
{
    public PoolType PoolType;
    private Transform tf;
    //cache tranform toi uu hieu nang -> tu Unity 2022 da toi uu -> co the khong can thiet
    public Transform TF
    {
        get
        {
            if (tf == null)
            {
                tf = transform;
            }
            return tf;
        }
    }
}
