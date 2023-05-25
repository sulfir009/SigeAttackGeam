using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobChaseController : MonoBehaviour
{
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
    private PlayerController playerController;
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
            if (distance <= chaseDistance)
            {
                if (distance <= attackDistance)
                {
                    mobAnimator.SetTrigger("Attack");
                    mobAnimator.SetBool("IsIdle", false);
                }
                else
                {
                    MoveTowards(playerTransform.position);
                    mobAnimator.SetBool("IsRunning", true);
                    mobAnimator.SetBool("IsIdle", false);
                }
            }
            else
            {
                mobAnimator.SetBool("IsRunning", false);
            }
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

    public void AttackPlayer()
    {
        if (isDead) return;
        if (Vector3.Distance(transform.position, playerTransform.position) <= attackDistance)
        {
            playerTransform.GetComponent<PlayerController>().TakeDamage((int)(mobStrengthController.strength * 0.3f));
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        mobStrengthController.DecreaseStrength(damage);

        if (mobStrengthController.strength <= 0)
        {
            mobAnimator.SetTrigger("Death");
            isDead = true;
            playerController.playerPower += StartStrength / 2;
            StartCoroutine(DeathAndDisappear(3f));
        }
    }

    private IEnumerator DeathAndDisappear(float delay)
    {
        yield return new WaitForSeconds(delay);
        mobControllers.Remove(id);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = other.transform;
            StartCoroutine(BattleCooldown(10f));
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
