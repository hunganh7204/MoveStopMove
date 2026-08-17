using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class Constants
{
    public const string ANIM_IDLE = "idle";
    public const string ANIM_RUN = "run";
    public const string ANIM_ATTACK = "attack";
    public const string ANIM_DEAD = "dead";
}
public class Character : GameUnit
{
    [Header("Base Stats")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotationSpeed = 15f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float attackRange = 5f;
    [SerializeField] protected float colliderRadius = 0.5f;

    [Header("Weapon")]
    [SerializeField] protected WeaponData weaponData;        
    [SerializeField] protected WeaponType currentWeaponType; 
    [SerializeField] protected Transform firePoint;          
    [SerializeField] protected Transform visualPoint;


    [Header("References")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody rb;

    protected bool isMoving;
    protected bool isAttacking;
    protected float lastAttackTime;
    protected List<Character> targets = new List<Character>();
    protected Coroutine attackCoroutine;
    protected WeaponItem currentWeapon;       
    protected GameObject currentVisualWeapon;

    private bool isDead;
    private string currentAnim = Constants.ANIM_IDLE;

    public virtual void OnInit()
    {
        SetDead(false);
        isMoving = false;
        isAttacking = false;
        targets.Clear();
        IdleAnim();
        ChangeWeapon(currentWeaponType);
    }

    public bool IsDead() => isDead;
    protected void SetDead(bool dead) => isDead = dead;
    public float ColliderRadius => colliderRadius;

    public virtual void Move(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            if (isAttacking)
            {
                CancelAttack();
            }
            if (!isMoving)
            {
                isMoving = true;
                RunAnim();
            }

            PerformTranslation(direction);
            PerformRotation(direction);
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
                IdleAnim();
            }
        }
    }

    private void PerformTranslation(Vector3 direction)
    {
        Vector3 targetPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }

    private void PerformRotation(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    public void AddTarget(Character target)
    {
        if (!targets.Contains(target)) targets.Add(target);
    }

    public void CleanUpTargets()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i] == null || !targets[i].gameObject.activeInHierarchy || targets[i].IsDead())
            {
                targets.RemoveAt(i);
                continue;
            }

            Vector3 myPos = new Vector3(TF.position.x, 0f, TF.position.z);
            Vector3 targetPos = new Vector3(targets[i].TF.position.x, 0f, targets[i].TF.position.z);

            float maxRange = attackRange + targets[i].ColliderRadius;

            if ((myPos - targetPos).sqrMagnitude > maxRange * maxRange)
            {
                targets.RemoveAt(i);
            }
        }
    }

    public bool HasTargets() => targets.Count > 0;
    public Character GetFirstTarget() => targets[0];

    public void TryStartAttack(Character target)
    {
        if (isAttacking || Time.time - lastAttackTime < attackCooldown) return;
        attackCoroutine = StartCoroutine(AttackRoutine(target));
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        if (weaponData == null) return;

        currentWeapon = weaponData.GetWeapon(weaponType);
        if (currentWeapon == null) return;

        currentWeaponType = weaponType;

        if (currentVisualWeapon != null)
        {
            Destroy(currentVisualWeapon);
        }

        if (currentWeapon.visual != null && visualPoint != null)
        {
            currentVisualWeapon = Instantiate(currentWeapon.visual, visualPoint);
            currentVisualWeapon.transform.localPosition = Vector3.zero;
            currentVisualWeapon.transform.localRotation = Quaternion.identity;
        }
    }

    protected IEnumerator AttackRoutine(Character target)
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        Vector3 dir = (target.TF.position - TF.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero) TF.rotation = Quaternion.LookRotation(dir);

        AttackAnim();

        yield return new WaitForSeconds(0.4f);


        if (target != null && !target.IsDead() && target.gameObject.activeInHierarchy && currentWeapon != null)
        {
            Vector3 throwDir = (target.TF.position - TF.position).normalized;
            throwDir.y = 0;

            Vector3 spawnPos = firePoint != null ? firePoint.position : TF.position;
            spawnPos.y = TF.position.y + 1f;

            BulletBase bullet = SimplePool.Spawn<BulletBase>(currentWeapon.bulletPrefab.PoolType, spawnPos, Quaternion.identity);

            if (bullet != null)
            {
                bullet.OnInit(this, throwDir, attackRange);
            }
            if (currentVisualWeapon != null)
            {
                currentVisualWeapon.SetActive(false);
            }
        }
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
        IdleAnim();
        if (currentVisualWeapon != null)
        {
            currentVisualWeapon.SetActive(true);
        }
    }

    public void CancelAttack()
    {
        if (isAttacking)
        {
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            isAttacking = false;
            IdleAnim();
            if (currentVisualWeapon != null)
            {
                currentVisualWeapon.SetActive(true);
            }
        }
    }

    public virtual void ChangeAnim(string animName)
    {
        animator.ResetTrigger(currentAnim);
        currentAnim = animName;
        animator.SetTrigger(currentAnim);
    }

    protected void IdleAnim() => ChangeAnim(Constants.ANIM_IDLE);
    protected void RunAnim() => ChangeAnim(Constants.ANIM_RUN);
    protected void AttackAnim() => ChangeAnim(Constants.ANIM_ATTACK);
    protected void DeadAnim() => ChangeAnim(Constants.ANIM_DEAD);


}