using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobChaseController : MonoBehaviour
{
    public SpriteRenderer attackRadiusSprite;
    public Material attackRadiusMaterial;
    private CharacterController characterController;
    private float gravity = -9.81f; // Добавить гравитацию, если объект находится на земле

    public Color attackStartColor = Color.clear;
    public Color attackEndColor = Color.red;
    private bool inAttackRadius = false;
    public float attackCooldown = 3f;
    private bool isAttacking = false;
    private bool isDead = false;
    public Transform playerTransform;
<<<<<<< Updated upstream
    public float speed = 10f;
=======
    public float speed = 5f;
    public float startChasingDistance = 50f;
>>>>>>> Stashed changes
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
    private Rigidbody rb;
    private float attackDelay = 1f;
    int groundLayerMask;

    private void Start()
    {
<<<<<<< Updated upstream
        playerController = FindObjectOfType<PlayerController>();
=======
        groundLayerMask = LayerMask.GetMask("Ground");
        characterController = GetComponent<CharacterController>();
>>>>>>> Stashed changes
        playerTransform = GameObject.FindWithTag("Player").transform;
        mobAnimator = GetComponent<Animator>();
        id = nextId++;
        mobControllers.Add(id, this);
        rb = GetComponent<Rigidbody>();
        playerController = playerTransform.GetComponent<PlayerController>(); // Добавлено
        StartCoroutine(StartChasing());
    }
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (!isAttacking && distance <= attackDistance)
        {
            StartCoroutine(PrepareAndAttack());
        }
    }

    private IEnumerator StartChasing()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= startChasingDistance && distance > attackDistance)
            {
                MoveTowards(playerTransform.position);
                mobAnimator.SetBool("IsRunning", true);
            }
            else
            {
                mobAnimator.SetBool("IsRunning", false);
            }
            yield return null;
        }
    }
    private IEnumerator PrepareAndAttack()
    {
        isAttacking = true;
        AttackPlayer();
        yield return new WaitForSeconds(attackCooldown);
        ResetAttackState();
    }


    private IEnumerator DisplayAttackRadius()
    {
        inAttackRadius = true;
        float progress = 0f;
        while (progress <= 1f)
        {
            attackRadiusMaterial.color = Color.Lerp(attackStartColor, attackEndColor, progress);
            progress += Time.deltaTime;
            yield return null;
        }
        inAttackRadius = false;
        attackRadiusMaterial.color = attackStartColor;
    }

    private void MoveTowards(Vector3 target)
    {
        if (isDead) return;
        Vector3 direction = (target - transform.position).normalized;

        // Обновить для использования CharacterController.Move вместо Rigidbody.MovePosition
        direction.y = gravity * Time.deltaTime; // Добавить гравитацию, если объект находится на земле
        characterController.Move(direction * speed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    void LateUpdate()
    {
        UpdateAttackRadiusPosition();
    }
    void UpdateAttackRadiusPosition()
    {
        Ray groundRay = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(groundRay, out hit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 radiusPosition = attackRadiusSprite.transform.position;
            radiusPosition.y = hit.point.y;
            attackRadiusSprite.transform.position = radiusPosition;
        }
    }
    public void AttackPlayer()
    {
        if (isDead) return;
        if (Vector3.Distance(transform.position, playerTransform.position) <= attackDistance)
        {
            StartCoroutine(DisplayAttackRadius());  // Добавлено
            mobAnimator.SetTrigger("Attack");
            playerTransform.GetComponent<PlayerController>().TakeDamage((int)(mobStrengthController.strength * 0.3f));
        }
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        ResetAttackState();
    }
    public void ResetAttackState()
    {
        isAttacking = false;
        mobAnimator.ResetTrigger("Attack");
    }
    public void TakeDamage(int damage)
    {
        if (isDead) {
            playerController.point += 10;
            mobControllers.Remove(id);
            Destroy(gameObject);        
            return; }
        mobStrengthController.DecreaseStrength(damage);

        if (mobStrengthController.strength <= 0)
        {
            mobAnimator.SetTrigger("Death");
            isDead = true;
            playerController.playerPower += StartStrength / 2;
           // DeathAndDisappear(1f);
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
}
