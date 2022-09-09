using UnityEngine;

public class EnemyMinion : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        Animator.SetBool("isWalk", true);
        Animator.Play("Enemy_Run", -1, Random.Range(0f, 1f));
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Freeze(float duration)
    {
        base.Freeze(duration);

        Animator.speed = 0;
    }

    protected override void Defreeze()
    {
        base.Defreeze();

        Animator.speed = 1;
    }

    public override void Death()
    {
        base.Death();

        Animator.SetBool("isDead", true);

        Destroy(gameObject, 2.5f);
    }

    public override void Failed()
    {
        base.Failed();

        Animator.SetBool("isWalk", false);
    }
}
