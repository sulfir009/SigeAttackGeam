
using System.Collections;
using UnityEngine;

public class MobBattleController : MonoBehaviour
{
    public MobStrengthController mobStrengthController;
    public Animator mobAnimator;

    public void AttackAnimation()
    {
        mobAnimator.SetTrigger("Attack");
    }
}
