using UnityEngine;

[RequireComponent(typeof(BotNavMesh))]
public class Bot : Character
{
    [Header("Bot Stats")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float idleDuration = 2f;

    [SerializeField] private BotNavMesh navMesh;
    private IState currentState;
    private float stateTimer;

    private void Start() => OnInit();

    public override void OnInit()
    {
        base.OnInit();
        currentState = null;
        ChangeState(IdleState.Instance);
    }

    private void FixedUpdate()
    {
        if (IsDead()) return;

        navMesh.SyncPosition(TF.position);

        currentState?.OnExecute(this);
    }

    public void ChangeState(IState newState)
    {
        currentState?.OnExit(this);
        currentState = newState;
        currentState?.OnEnter(this);
    }

    public BotNavMesh GetNavMesh() => navMesh;
    public float GetPatrolRadius() => patrolRadius;

    public void ResetIdleTimer() => stateTimer = idleDuration;
    public void DecreaseStateTimer(float deltaTime) => stateTimer -= deltaTime;
    public bool IsStateTimerFinished() => stateTimer <= 0;
}