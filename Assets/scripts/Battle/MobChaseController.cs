using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobChaseController : MonoBehaviour
{
    public SpriteRenderer attackRadiusSprite;
    public Material attackRadiusMaterial;
    public Color attackStartColor = Color.clear;
    public Color attackEndColor = Color.red;
    public LayerMask groundLayerMask;

    enum MobState { Idle, Chasing, PreparingAttack, Attacking, Waiting }
    private MobState currentState = MobState.Idle;
    private bool inAttackRadius = false;
    public float attackCooldown = 2f;

    public List<UltimateAbility> ultimateAbilities;
    private float ultimateAbilityCooldownTimer = 0f;

    private bool isDead = false;
    public Transform playerTransform;
    public float speed = 10f;
    public float chaseDistance = 5f;
    public float attackDistance = 2f;
    public bool isAlly = false;
    public MobStrengthController mobStrengthController;
    public Animator mobAnimator;
    public float idleDistanceThreshold = 2f;
    public float rotationSpeed = 2f;
    private PlayerController playerController;
    public static Dictionary<int, MobChaseController> mobControllers = new Dictionary<int, MobChaseController>();
    private int id;
    private static int nextId = 0;
    private static int StartStrength = 0;
    private CharacterController characterController;
    private Coroutine currentMoveCoroutine = null;


    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        mobAnimator = GetComponent<Animator>();
        id = nextId++;
        mobControllers.Add(id, this);
        characterController = GetComponent<CharacterController>();
        attackRadiusMaterial.color = new Color(attackRadiusMaterial.color.r, attackRadiusMaterial.color.g, attackRadiusMaterial.color.b, 0.5f);
    }

    private void Update()
    {
        if (currentState == MobState.Chasing)
        {
            attackRadiusSprite.enabled = false;
        }
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        inAttackRadius = distance <= attackDistance;
        if (isDead) return;

        if (ultimateAbilityCooldownTimer <= 0 && currentState == MobState.Attacking)
        {
            ultimateAbilityCooldownTimer = 12f;
            int abilityIndex = Random.Range(0, ultimateAbilities.Count);
            StartCoroutine(ultimateAbilities[abilityIndex].UseAbility(this));
        }
        else
        {
            ultimateAbilityCooldownTimer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case MobState.Idle:
                if (distance <= chaseDistance)
                {
                    currentState = MobState.Chasing;
                }
                break;

            case MobState.Chasing:
                if (distance <= attackDistance)
                {
                    if (currentMoveCoroutine != null)
                    {
                        StopCoroutine(currentMoveCoroutine);
                        currentMoveCoroutine = null;
                    }
                    currentState = MobState.PreparingAttack;
                    StartCoroutine(AttackWithDelay());
                }
                else
                {
                    if (currentMoveCoroutine == null)
                    {
                        currentMoveCoroutine = StartCoroutine(MoveTowardsCoroutine(playerTransform.position));
                    }
                }
                break;

            case MobState.PreparingAttack:
                mobAnimator.SetBool("IsRunning", false);
                mobAnimator.SetBool("IsIdle", false);
                if (distance > attackDistance)
                {
                    currentState = MobState.Chasing;
                }
                break;

            case MobState.Attacking:
                mobAnimator.SetBool("IsRunning", false);
                mobAnimator.SetBool("IsIdle", false);
                if (distance > attackDistance)
                {
                    currentState = MobState.Chasing;
                }
                break;

            case MobState.Waiting:
                mobAnimator.SetBool("IsIdle", true);
                if (distance > attackDistance)
                {
                    currentState = MobState.Chasing;
                }
                break;
        }
    }
    private IEnumerator MoveTowardsCoroutine(Vector3 target)
    {
        mobAnimator.SetBool("IsRunning", true);

        while (Vector3.Distance(transform.position, target) > attackDistance)
        {
            if (currentState != MobState.Chasing)
            {
                mobAnimator.SetBool("IsRunning", false);
                yield break;
            }

            Vector3 direction = (target - transform.position).normalized;
            Vector3 velocity = direction * speed * Time.deltaTime;

            if (Physics.Raycast(transform.position, -transform.up, 1.5f, groundLayerMask))
            {
                velocity.y = 0;
            }

            characterController.Move(velocity);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        mobAnimator.SetBool("IsRunning", false);
    }
    public void AttackPlayer()
    {
        if (currentState == MobState.Chasing || currentState == MobState.Attacking)
        {
            StartCoroutine(AttackWithDelay());
        }
    }

    public void AttackAnimation()
    {
        if (currentState == MobState.Chasing || currentState == MobState.Attacking)
        {
            StartCoroutine(AttackWithDelay());
            mobAnimator.SetTrigger("Attack");
        }
    }

    private IEnumerator PostAttackDelay(float delay)
    {
        currentState = MobState.Waiting;
        yield return new WaitForSeconds(delay);
        if (currentState == MobState.Waiting)
        {
            currentState = MobState.Idle;
        }
    }


    public abstract class UltimateAbility : ScriptableObject
    {
        public string abilityName;
        public string description;
        public Sprite abilityIcon;
        public float cooldown = 12f;
        public float radius;
        public float damage;

        public abstract IEnumerator UseAbility(MobChaseController mobController);
    }

    private IEnumerator AttackWithDelay()
    {
        mobAnimator.SetTrigger("Attack");
        attackRadiusSprite.enabled = true;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 2; // Ќа это уйдут 2 секунды
            attackRadiusMaterial.color = Color.Lerp(attackStartColor, attackEndColor, t);
            yield return null;

            if (!inAttackRadius) // провер€ем, в зоне ли атаки игрок
            {
                attackRadiusSprite.enabled = false;
                attackRadiusMaterial.color = attackStartColor;
                currentState = MobState.Chasing;
                yield break;
            }
        }

        // ѕосле 2 секунд происходит атака игрока
        if (inAttackRadius) // атакуем, только если игрок все еще в радиусе атаки
        {
            int damageToPlayer = mobStrengthController.strength / 3; // вычисл€ем урон игроку
            playerController.TakeDamage(damageToPlayer); // вызываем метод TakeDamage у игрока

            // Set state to attacking
            currentState = MobState.Attacking;
        }

        // после атаки ждем остальную часть кулдауна атаки
        yield return new WaitForSeconds(attackCooldown); // ¬ычитаем 2 секунды, потому что уже ждали 2 секунды

        // после кулдауна атаки ожидаем следующую атаку
        StartCoroutine(WaitForNextAttack(attackCooldown));

        attackRadiusSprite.enabled = false;
        attackRadiusMaterial.color = attackStartColor;
    }


    private IEnumerator WaitForNextAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentState == MobState.Waiting)
        {
            currentState = MobState.Idle;
        }
    }

    private void MoveTowards(Vector3 target)
    {
        if (isDead) return;

        Vector3 direction = (target - transform.position).normalized;
        Vector3 velocity = direction * speed * Time.deltaTime;

        if (Physics.Raycast(transform.position, -transform.up, 1.5f, groundLayerMask))
        {
            velocity.y = 0;
        }

        characterController.Move(velocity);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            playerController.point += 10;
            playerController.RefreshPoints();
            mobControllers.Remove(id);
            Destroy(gameObject);
        }
        else
        {
            mobStrengthController.strength -= damage;
            if (mobStrengthController.strength <= 0)
            {
                isDead = true;
            }
        }
        if (mobStrengthController.strength <= 0)
        {
            isDead = true;
            playerController.point += 10;
            playerController.RefreshPoints();
            mobControllers.Remove(id);
            Destroy(gameObject);
        }
    }
}
