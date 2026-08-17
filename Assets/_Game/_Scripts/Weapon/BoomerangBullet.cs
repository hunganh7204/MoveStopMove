
using UnityEngine;

public class BoomerangBullet : BulletBase
{
    private bool isReturning = false;

    public override void OnInit(Character shooter, Vector3 direction, float attackRange)
    {
        base.OnInit(shooter, direction, attackRange);
        isReturning = false;
    }

    protected override void Move()
    {
        if (!isReturning)
        {
            TF.Translate(TF.forward * speed * Time.deltaTime, Space.World);
        }
        else
        {
            if (shooter != null && !shooter.IsDead())
            {
                Vector3 targetPos = shooter.TF.position;
                targetPos.y = TF.position.y;

                Vector3 returnDir = (targetPos - TF.position).normalized;
                TF.Translate(returnDir * speed * Time.deltaTime, Space.World);
            }
            else
            {
                OnDespawn();
            }
        }
    }

    protected override void RotateVisual()
    {
        if (visual != null)
        {
            visual.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.Self);
        }
    }

    protected override void CheckDespawn()
    {
        if (!isReturning)
        {
            Vector3 currentFlatPos = new Vector3(TF.position.x, 0f, TF.position.z);
            Vector3 startFlatPos = new Vector3(startPos.x, 0f, startPos.z);

            if ((currentFlatPos - startFlatPos).sqrMagnitude > maxRange * maxRange)
            {
                isReturning = true;
            }
        }
        else
        {
            if (shooter != null)
            {
                Vector3 myFlatPos = new Vector3(TF.position.x, 0f, TF.position.z);
                Vector3 shooterFlatPos = new Vector3(shooter.TF.position.x, 0f, shooter.TF.position.z);
                if ((myFlatPos - shooterFlatPos).sqrMagnitude < 4f)
                {
                    OnDespawn();
                }
            }
        }
    }
}
