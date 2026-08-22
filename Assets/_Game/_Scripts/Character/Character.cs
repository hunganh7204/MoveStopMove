using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Equipment")]
    [SerializeField] protected HatData hatData;
    [SerializeField] protected PantData pantData;
    [SerializeField] protected AccessoryData accessoryData;

    [Header("Equipment Pos")]
    [SerializeField] protected Transform hatPos;
    [SerializeField] protected Transform accessoryPos;
    [SerializeField] protected Renderer pantRend;
    
    [Header("References")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider col;
    [SerializeField] protected Renderer rend;

    [Header("Level System")]
    public int level = 1;
    protected float currentExp = 0f;
    protected float expToNextLevel = 1f;

    [Header("Test Equipment")]
    [SerializeField] protected string currentHatId;
    [SerializeField] protected string currentPantId;
    [SerializeField] protected string currentAccessoryId;

    protected float initialAttackRange;
    protected float initialMoveSpeed;

    protected GameObject currentHatVisual;
    protected GameObject currentAccessoryVisual;

    protected bool isMoving;
    protected bool isAttacking;
    protected float lastAttackTime;
    protected List<Character> targets = new List<Character>();
    protected Coroutine attackCoroutine;
    protected WeaponItem currentWeapon;       
    protected GameObject currentVisualWeapon;

    private bool isDead;
    private string currentAnim = Constants.ANIM_IDLE;

    protected virtual void Awake()
    {
        initialAttackRange = attackRange;
        initialMoveSpeed = moveSpeed;
    }

    public virtual void OnInit()
    {
        SetDead(false);
        isMoving = false;
        isAttacking = false;
        targets.Clear();

        level = 1;
        currentExp = 0f;
        expToNextLevel = 1f;
        attackRange = initialAttackRange;
        moveSpeed = initialMoveSpeed;
        TF.localScale = Vector3.one;

        IdleAnim();
        ChangeWeapon(currentWeaponType);
        EquipTestItems();
    }

    [ContextMenu("Test Change Equipment")]
    public void EquipTestItems()
    {
        ChangeHat(currentHatId);
        ChangePant(currentPantId);
        ChangeAccessory(currentAccessoryId);
    }

    public void AddExp(float amount)
    {
        currentExp += amount;
        while(currentExp > expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    protected virtual void LevelUp()
    {
        level++;
        expToNextLevel = level * 2f;
        float scaleMultiplier = 1f + (level - 1) * 0.1f;
        TF.localScale = Vector3.one * scaleMultiplier;
        attackRange = initialAttackRange * scaleMultiplier;
        moveSpeed = initialMoveSpeed * (1f + (level - 1) * 0.05f);
    }

    public virtual void OnHit(Character attacker)
    {
        if (IsDead()) return;
        SetDead(true);
        if (col != null) col.enabled = false;
        isMoving = false;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            isAttacking = false;
        }
        targets.Clear();
        if(attacker != null && !attacker.IsDead())
        {
            float grantedExp = 1f + (this.level * 1.5f);
            attacker.AddExp(grantedExp);
        }
        StartCoroutine(DeathRoutine());
    }
    private IEnumerator DeathRoutine()
    {
        DeadAnim();
        yield return new WaitForSeconds(1.5f);

        float fadeDuration = 1f; 
        float timer = 0f;
        

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            
            yield return null; 
        }
        OnDeath();
    }
    public virtual void OnDeath()
    {
        if (currentVisualWeapon != null)
        {
            currentVisualWeapon.SetActive(false);
        }
        SimplePool.Despawn(this);
    }

    public bool IsDead() => isDead;
    protected void SetDead(bool dead) => isDead = dead;
    public float ColliderRadius => colliderRadius;

    public float GetAttackRange()
    {
        return attackRange;
    }

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

    public void ChangeHat(string id)
    {
        if(hatData == null) return;
        HatItem item = hatData.GetItem(id);
        if (item == null) return;
        if(currentHatVisual != null)
        {
            Destroy(currentHatVisual);
        }
        currentHatVisual = Instantiate(item.visualPrefab, hatPos);
        currentHatVisual.transform.localPosition = Vector3.zero;
        currentHatVisual.transform.localRotation = Quaternion.identity;
    }

    public void ChangeAccessory(string id)
    {
        if(accessoryData == null) return;
        AccessoryItem item = accessoryData.GetItem(id);
        if(item == null) return;
        if (currentAccessoryVisual != null)
        {
            Destroy(currentAccessoryVisual);
        }
        currentAccessoryVisual = Instantiate(item.visualPrefab, accessoryPos);
        currentAccessoryVisual.transform.localPosition = Vector3.zero;
        currentAccessoryVisual.transform.localRotation = Quaternion.identity;
    }

    public void ChangePant(string id)
    {
        if(pantData == null) return;
        PantItem item = pantData.GetItem(id);
        if(item == null) return;
        pantRend.material = item.pantMaterial;
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