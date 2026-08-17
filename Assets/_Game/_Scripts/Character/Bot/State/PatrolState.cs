using UnityEngine;

public class PatrolState : IState
{
    public static readonly PatrolState Instance = new PatrolState();
    private PatrolState() { }

    public void OnEnter(Bot bot)
    {
        if (bot.GetNavMesh().TryGetRandomPoint(bot.transform.position, bot.GetPatrolRadius(), out Vector3 dest))
        {
            bot.GetNavMesh().SetDestination(dest);
        }
        else
        {
            bot.ChangeState(IdleState.Instance);
        }
    }

    public void OnExecute(Bot bot)
    {
        Vector3 dir = bot.GetNavMesh().GetSteeringDirection(bot.transform.position);
        bot.Move(dir);
        if (bot.GetNavMesh().IsAtDestination(bot.transform.position) || dir == Vector3.zero)
        {
            bot.ChangeState(IdleState.Instance);
        }
    }

    public void OnExit(Bot bot)
    {
        bot.GetNavMesh().StopCalculating();
    }
}