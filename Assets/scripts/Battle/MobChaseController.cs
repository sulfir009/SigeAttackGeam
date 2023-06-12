using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobChaseController : MonoBehaviour
{
    public SpriteRenderer attackRadiusSprite;
    public Material attackRadiusMaterial;
    public Color attackStartColor = Color.clear;
    public Color attackEndColor = Color.red;

    enum MobState { Idle, Chasing, Attacking, Waiting }
    private MobState currentState = MobState.Idle;
    private bool inAttackRadius = false;
    public float attackCooldown = 1f;

    private bool isDead = false;
    public Transform playerTransform;
    public float speed = 5f;
    public float chaseDistance = 5f;
    public float attackDistance = 2f;
    public bool isAlly = false;
    public MobStrengthController mobStrengthController;
    public Animator mobAnimator;
    public float idleDistanceThreshold = 2f;
    public float rotationSpeed = 2f;
    public PlayerController playerController;
    public static Dictionary<int, MobChaseController> mobControllers = new Dictionary<int, MobChaseController>();
    private int id;
    private static int nextId = 0;
    private static int StartStrength = 0;
    private CharacterController characterController;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        mobAnimator = GetComponent<Animator>();
        id = nextId++;
        mobControllers.Add(id, this);
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (isDead) return;

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
        StartCoroutine(AttackWithDelay());
    }

    private IEnumerator AttackWithDelay()
    {
        attackRadiusSprite.enabled = true;

        float elapsedTime = 0f;

        while (elapsedTime < 3f)
        {
            float progress = elapsedTime / 3f;
            attackRadiusMaterial.color = Color.Lerp(attackStartColor, attackEndColor, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        attackRadiusSprite.enabled = false;
        if (inAttackRadius)
        {
            mobAnimator.SetTrigger("Attack");
            currentState = MobState.Waiting;
            StartCoroutine(WaitForNextAttack(attackCooldown));
        }
        else
        {
            currentState = MobState.Chasing;
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
        transform.position += velocity;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) {

            playerController.point += 10;
            return; }
        mobStrengthController.DecreaseStrength(damage);

        if (mobStrengthController.strength <= 0)
        {
            mobAnimator.SetTrigger("Death");
            isDead = true;
            playerController.playerPower += StartStrength / 2;
            DeathAndDisappear(1f);
        }
    }

    private void DeathAndDisappear(float delay)
    {
        while (delay > 0) { 
        delay -= Time.deltaTime;
        };
        mobControllers.Remove(id);
        Destroy(gameObject);
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
