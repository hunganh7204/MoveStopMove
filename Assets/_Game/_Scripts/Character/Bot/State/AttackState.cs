using UnityEngine;

public class AttackState : IState
{
    public static readonly AttackState Instance = new AttackState();
    private AttackState() { }

    public void OnEnter(Bot bot)
    {
        bot.Move(Vector3.zero);
    }

    public void OnExecute(Bot bot)
    {
        bot.CleanUpTargets();

        if (!bot.HasTargets())
        {
            bot.ChangeState(IdleState.Instance);
            return;
        }

        bot.TryStartAttack(bot.GetFirstTarget());
    }

    public void OnExit(Bot bot)
    {
        bot.CancelAttack();
    }
}