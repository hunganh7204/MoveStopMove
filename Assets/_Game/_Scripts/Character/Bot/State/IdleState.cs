using UnityEngine;

public class IdleState : IState
{
    public static readonly IdleState Instance = new IdleState();
    private IdleState() { }

    public void OnEnter(Bot bot)
    {
        bot.Move(Vector3.zero);
        bot.ResetIdleTimer();
    }

    public void OnExecute(Bot bot)
    {
        bot.CleanUpTargets();

        if (bot.HasTargets())
        {
            bot.ChangeState(AttackState.Instance);
            return;
        }

        bot.DecreaseStateTimer(Time.fixedDeltaTime);
        if (bot.IsStateTimerFinished())
        {
            bot.ChangeState(PatrolState.Instance);
        }
    }

    public void OnExit(Bot bot) { }
}