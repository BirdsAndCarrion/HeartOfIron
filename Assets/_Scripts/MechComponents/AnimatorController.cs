using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;

    private readonly string IsMovingParam = "IsMoving";
    private readonly string LeftArmActiveParam = "LeftArmActive";
    private readonly string RightArmActiveParam = "RightArmActive";
    private readonly string LeftArmBlendParam = "LeftArmBlend";
    private readonly string RightArmBlendParam = "RightArmBlend";
    private readonly string MeleeLParam = "MeleeL";
    private readonly string MeleeRParam = "MeleeR";
    private readonly string MeleeLeftHandedParam = "MeleeLeftHanded";
    private readonly string MeleeRightHandedParam = "MeleeRightHanded";
    private readonly string MeleeTwoHandedParam = "MeleeTwoHanded";

    public void Init(Animator animator)
    {
        this.animator = animator;
    }

    public void SetIsMoving(bool isMoving)
    {
        animator.SetBool(IsMovingParam, isMoving);
    }

    public void SetOneHandedLeft(bool isActive, bool isEquipped = false, bool isMelee = false)
    {
        animator.SetBool(LeftArmActiveParam, isActive);
        animator.SetBool(MeleeLParam, isMelee);
        animator.SetFloat(LeftArmBlendParam, isActive && isEquipped ? 0.5f : 0f);
    }

    public void SetOneHandedRight(bool isActive, bool isEquipped = false, bool isMelee = false)
    {
        animator.SetBool(RightArmActiveParam, isActive);
        animator.SetBool(MeleeRParam, isMelee);
        animator.SetFloat(RightArmBlendParam, isActive && isEquipped ? 0.5f : 0f);
    }

    public void SetTwoHanded(bool isActive, bool isMelee = false)
    {
        animator.SetBool(LeftArmActiveParam, isActive);
        animator.SetBool(RightArmActiveParam, isActive);
        animator.SetBool(MeleeLParam, isMelee);
        animator.SetBool(MeleeRParam, isMelee);
        animator.SetFloat(LeftArmBlendParam, isActive ? 1f : 0f);
        animator.SetFloat(RightArmBlendParam, isActive ? 1f : 0f);
    }

    public void PlayMeleeRight()
    {
        animator.SetTrigger(MeleeRightHandedParam);
    }

    public void PlayMeleeLeft()
    {
        animator.SetTrigger(MeleeLeftHandedParam);
    }

    public void PlayMeleeTwoHanded()
    {
        animator.SetTrigger(MeleeTwoHandedParam);
    }
}
