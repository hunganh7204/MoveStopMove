using UnityEngine;

public class BulletBase : GameUnit
{
    [Header("Bullet settings")]
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected Transform visual;
    [SerializeField] protected float rotateSpeed = 720f;

    protected Character shooter;
    protected float maxRange;
    protected Vector3 startPos;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider col;


    public virtual void OnInit(Character shooter, Vector3 direction, float attackRange)
    {
        this.shooter = shooter;
        this.maxRange = attackRange +1f;
        this.startPos = TF.position;
        TF.rotation = Quaternion.LookRotation(direction);
    }

    protected virtual void Update()
    {
        Move();
        RotateVisual();
        CheckDespawn();
    }
    protected virtual void Move()
    {
        TF.Translate(TF.forward * speed * Time.deltaTime, Space.World);
    }

    protected virtual void RotateVisual()
    {

    }

    protected virtual void CheckDespawn()
    {
        Vector3 currentFlatPos = new Vector3(TF.position.x, 0f, TF.position.z);
        Vector3 startFlatPos = new Vector3(startPos.x, 0f, startPos.z);

        if ((currentFlatPos - startFlatPos).sqrMagnitude > maxRange * maxRange)
        {
            OnDespawn();
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=yellow>[TEST] Đạn chạm vào Component có tên là: {other.gameObject.name}</color>");
        if (shooter == null) return;
        Character hitTarget = other.GetComponent<Character>();
        if(hitTarget != null && hitTarget != shooter && !hitTarget.IsDead())
        {
            Debug.Log($"{shooter.gameObject.name} hit {hitTarget.gameObject.name}");
            hitTarget.OnHit(shooter);
            OnDespawn();
        }
    }
    public virtual void OnDespawn()
    {
        SimplePool.Despawn(this);
    }
}
