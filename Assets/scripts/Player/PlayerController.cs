using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public TextMeshProUGUI deathMessage; // новое поле дл€ отображени€ сообщени€ о смерти
    private bool isDead = false; // нова€ переменна€ дл€ отслеживани€ смерти персонажа
    bool IsRoll = false;
    public Button buttonRoll;
    public Transform playerTransform;
    public CharacterController characterController;
    public float speed = 3.0f;
    public DynamicJoystick joystick;
    public float runSpeed = 6.0f;
    public ControllCamera controllCamera;
    private float currentRunDuration = 0.0f;
    private float currentRunCooldown = 0.0f;
    public Animator playerAnimator;
    public int playerPower;
    private bool isAtacket = true;
    private bool isRunning;
    private float coutAttack = 1;
    private MobChaseController chaseController;
    public List<MobChaseController> alliedMobs = new List<MobChaseController>();
    public float rotationSpeed = 10.0f;
    private Vector3 moveDirection = Vector3.zero;
    public float attackRange = 2.0f;  // ƒобавлено


    public Button runButton;
    public Button attackButton;
    private void Start()
    {
        joystick.DeadZone = 0.1f; // ”меньшить Dead Zone
        runButton.onClick.AddListener(ToggleRun);
        attackButton.onClick.AddListener(Attack);
        buttonRoll.onClick.AddListener(() => Rilling());
    }
    private void Rilling()
    {
        
        StartCoroutine(RollCooldown(0.8f));
        
    }
    private IEnumerator RollCooldown(float duration)
    {
        playerAnimator.SetBool("Roll", true);
        yield return new WaitForSeconds(duration);
        playerAnimator.SetBool("Roll", false);
        
    }
    public void Update()
    {
        if (isDead) return;

        float moveHorizontal = joystick.Horizontal;
        float moveVertical = joystick.Vertical;

        float threshold = 0.1f; // ¬ы можете подобрать значение порога экспериментальным путем.
        if (Mathf.Abs(moveHorizontal) < threshold && Mathf.Abs(moveVertical) < threshold)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            moveDirection = new Vector3(moveHorizontal, 0.0f, moveVertical);
        }

        float currentSpeed = isRunning ? runSpeed : speed;

        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
      
        playerAnimator.SetBool("IsRunning", isRunning);
       
        playerAnimator.SetFloat("Speed", moveDirection.magnitude > 0 ? 1 : 0);

        if (moveDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentRunDuration, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
        }

        controllCamera.enabled = true;
    }


    public void SetRunning(bool running)
    {
        isRunning = running;
    }

    public int powerRet()
    {
        return playerPower;
    }

    public void TakeDamage(int damage)
    {
        playerPower -= damage;
        if (playerPower <= 0 && !isDead)
        {
            playerAnimator.SetTrigger("Death");
            isDead = true;
            // Implement game over logic here
            deathMessage.text = "Your character has died.";
            // Implement game over logic here
        }
    }
    public void AddAlliedMob(MobChaseController mob)
    {
        if (!alliedMobs.Contains(mob))
        {
            alliedMobs.Add(mob);
        }
    }
    private void ToggleRun()
    {
        isRunning = !isRunning;
    }
    public void Attack()
    {
        if (isDead) return;

        if (isAtacket == true)
        {
            if (coutAttack == 4)
            {
                coutAttack = 1;
            }
            if (playerAnimator.isActiveAndEnabled)
            {
                AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.normalizedTime >= 0.5f)
                {
                    // ѕрерываем анимацию и сбрасываем состо€ние
                    playerAnimator.StopPlayback();
                    playerAnimator.Play(stateInfo.fullPathHash, -1, 0f);
                }
            }
            playerAnimator.SetFloat("AttackCout", coutAttack);
            coutAttack++;
            playerAnimator.SetBool("IsIdle", false);
            playerAnimator.SetTrigger("Attack");

            // Check if there is a mob in range to attack
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
            foreach (var hitCollider in hitColliders)
            {
                MobChaseController mob = hitCollider.GetComponent<MobChaseController>();
                if (mob != null && !mob.isAlly)
                {
                    
                    mob.TakeDamage((int)(playerPower * 0.3f)); // обновлено
                    
                    StartCoroutine(AttackCooldown(0.8f));
                    return;
                }
            }

            // ≈сли ни одного врага не было найдено, все равно переходим в состо€ние Idle
            StartCoroutine(AttackCooldown(0.8f));
        }
    }

    private IEnumerator AttackCooldown(float duration)
    {
        isAtacket = false;
        yield return new WaitForSeconds(duration);
        playerAnimator.ResetTrigger("Attack");
        playerAnimator.SetBool("IsIdle", true);
        isAtacket = true;
        
    }
}