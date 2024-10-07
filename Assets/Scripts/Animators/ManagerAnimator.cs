using UnityEngine;

public class ManagerAnimator : MonoBehaviour
{
    Animator animator;
    public Transform lookAt;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = GameManager.Singleton.GetAnimation(AnimState.ANGY);
        LookAtPlayer();
    }

    public void PlayAngy()
    {
        animator.runtimeAnimatorController = GameManager.Singleton.GetAnimation(AnimState.ANGY);
    }

    public void PlayIdle()
    {
        animator.runtimeAnimatorController = GameManager.Singleton.GetAnimation(AnimState.IDLE);
    }

    public void LookAtPlayer()
    {
        transform.LookAt(lookAt);
    }
}
