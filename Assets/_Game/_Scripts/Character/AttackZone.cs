using UnityEngine;

public class AttackZone : MonoBehaviour
{
    [SerializeField] private Character owner;

    private void OnTriggerEnter(Collider other)
    {
        Character target = other.GetComponent<Character>();

        if (target != null && target != owner && !target.IsDead())
        {
            owner.AddTarget(target);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    Character otherCharacter = other.GetComponent<Character>();
    //    if (otherCharacter != null && otherCharacter != character)
    //    {
    //        character.RemoveTarget(otherCharacter);
    //    }
    //}
}
