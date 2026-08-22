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
        navMesh.EnableAgent();
        ChangeState(IdleState.Instance);
    }

    private void FixedUpdate()
    {
        if (IsDead())
        {
            return;
        }
        navMesh.SyncPosition(TF.position);
        currentState?.OnExecute(this);
    }

    public void ChangeState(IState newState)
    {
        currentState?.OnExit(this);
        currentState = newState;
        currentState?.OnEnter(this);
    }

    public override void OnDeath()
    {
        navMesh.DisableAgent();
        base.OnDeath();
    }

    public void SetLevel(int targetLevel)
    {
        level = targetLevel;
        expToNextLevel = level * 10f;
        float scaleMultiplier = 1f + (level - 1) * 0.1f;
        TF.localScale = Vector3.one * scaleMultiplier;
        attackRange = initialAttackRange * scaleMultiplier;
        moveSpeed = initialMoveSpeed * (1f + (level - 1) * 0.05f);
    }

    public void EquipRandomItems()
    {
        if (hatData != null && hatData.items.Count > 0)
        {
            int randIndex = Random.Range(0, hatData.items.Count);
            ChangeHat(hatData.items[randIndex].id);
        }

        if (pantData != null && pantData.items.Count > 0)
        {
            int randIndex = Random.Range(0, pantData.items.Count);
            ChangePant(pantData.items[randIndex].id);
        }

        int randomWeapon = Random.Range(0, 3);
        ChangeWeapon((WeaponType)randomWeapon);
    }

    public BotNavMesh GetNavMesh() => navMesh;
    public float GetPatrolRadius() => patrolRadius;

    public void ResetIdleTimer() => stateTimer = idleDuration;
    public void DecreaseStateTimer(float deltaTime) => stateTimer -= deltaTime;
    public bool IsStateTimerFinished() => stateTimer <= 0;
}