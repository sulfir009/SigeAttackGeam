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

    enum MobState { Idle, Chasing, Attacking, Waiting }
    private MobState currentState = MobState.Idle;
    private bool inAttackRadius = false;
    public float attackCooldown = 1f;

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

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        mobAnimator = GetComponent<Animator>();
        id = nextId++;
        mobControllers.Add(id, this);
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (currentState == MobState.Chasing)
        {
            attackRadiusSprite.enabled = false;
        }
        float distance = Vector3.Distance(transform.position, playerTransform.position);
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

        if (isAlly)
        {
        }
        else
        {
            if (distance <= chaseDistance && currentState == MobState.Idle)
            {
                currentState = MobState.Chasing;
            }
            if (currentState == MobState.Chasing)
            {
                if (distance <= attackDistance)
                {
                    inAttackRadius = true;
                    StartCoroutine(AttackWithDelay());
                    currentState = MobState.Attacking;
                }
                else
                {
                    MoveTowards(playerTransform.position);
                    mobAnimator.SetBool("IsRunning", true);
                }
            }
            if (currentState == MobState.Attacking)
            {
                if (distance > attackDistance)
                {
                    inAttackRadius = false;
                }
                mobAnimator.SetBool("IsRunning", false);
                mobAnimator.SetBool("IsIdle", true);
            }
            if (currentState == MobState.Waiting)
            {
                if (distance > attackDistance)
                {
                    inAttackRadius = false;
                    currentState = MobState.Chasing;
                }
                else
                {
                    mobAnimator.SetBool("IsIdle", true);
                }
            }
        }
    }
    public void AttackPlayer()
    {
        if (currentState != MobState.Chasing && currentState != MobState.Attacking)
            return;

        StartCoroutine(AttackWithDelay());
    }
    public void AttackAnimation()
    {
        StartCoroutine(AttackWithDelay());
        mobAnimator.SetTrigger("Attack");
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
    private IEnumerator PostAttackDelay(float delay)
    {
        currentState = MobState.Waiting;
        yield return new WaitForSeconds(delay);
        if (currentState == MobState.Waiting)
        {
            currentState = MobState.Chasing;
        }
    }
    private IEnumerator AttackWithDelay()
    {
        if (inAttackRadius)
        {
            mobAnimator.SetTrigger("Attack");
            currentState = MobState.Waiting;
            attackRadiusSprite.enabled = true;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / attackCooldown;
                attackRadiusMaterial.color = Color.Lerp(attackStartColor, attackEndColor, t);
                yield return null;
            }

            yield return new WaitForSeconds(attackCooldown);
            if (currentState == MobState.Waiting)
            {
                currentState = MobState.Chasing;
            }
            // Дальше ваш код...
        }
    }





    private IEnumerator WaitForNextAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentState == MobState.Waiting)
        {
            currentState = MobState.Chasing;
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
            mobControllers.Remove(id);
            Destroy(gameObject);
            return;
        }
        mobStrengthController.DecreaseStrength(damage);

        if (mobStrengthController.strength <= 0)
        {
            mobAnimator.SetTrigger("Death");
            isDead = true;
            playerController.playerPower += StartStrength / 2;
            StartCoroutine(DeathAndDisappear(1f));
        }
    }

    private IEnumerator DeathAndDisappear(float delay)
    {
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        };
        mobControllers.Remove(id);
        Destroy(gameObject);
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = other.transform;
            StartCoroutine(BattleCooldown(3f));
        }
    }

    private IEnumerator BattleCooldown(float duration)
    {
        StartStrength = mobStrengthController.strength;

        yield return new WaitForSeconds(duration);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isDead) return;
        if (hit.gameObject.CompareTag("Player"))
        {
            playerTransform = hit.transform;
            AttackPlayer();
        }
    }
}
